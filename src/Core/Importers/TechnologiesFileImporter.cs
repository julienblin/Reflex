// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesFileImporter.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Importers.Models;
using CGI.Reflex.Core.Queries;
using ClosedXML.Excel;
using NHibernate;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Importers
{
    internal class TechnologiesFileImporter : BaseFileImporter
    {
        public TechnologiesFileImporter()
            : base("Technologies", 3)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.TechnologyType);

                var technoModel = new TechnologyModel();
                var ws = workbook.Worksheets.Add("Technologies");
                technoModel.Prepare(ws, "Technologies");

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "Technologies-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.TechnologyType);

                var technoModel = new TechnologyModel();
                var ws = workbook.Worksheets.Add("Technologies");
                technoModel.Prepare(ws, "Technologies");

                var technologies = session.QueryOver<Technology>();
                for (var i = 0; i < 8; i++)
                    technologies = technologies.Left.JoinQueryOver(t => t.Children);
                
                technologies.TransformUsing(Transformers.DistinctRootEntity);

                var row = ws.Row(4);
                foreach (var technology in technologies.List().OrderBy(t => t.FullName))
                {
                    if (technology.HasChildren())
                        continue;

                    technoModel = new TechnologyModel();
                    technoModel.FromEntity(technology).ToRow(row);
                    row = row.RowBelow();
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("Technologies-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    var ws = workbook.Worksheet("Technologies");
                    if (ws != null)
                        Import<Technology, TechnologyModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var entity = new TechnologyByEscapedFullNameQuery { EscapedFullName = model.FullName }.Execute(sess);
                                if (entity != null)
                                    return entity;

                                // We must construct full hierarchy
                                var tokens = model.FullName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (tokens.Count > 0)
                                {
                                    tokens = tokens.Take(tokens.Count - 1).ToList();
                                    Technology currentTechno = null;
                                    foreach (var token in tokens)
                                    {
                                        var potentialFullName = (currentTechno == null
                                                                     ? string.Empty
                                                                     : (currentTechno.GetEscapedFullName() + " ")) + token;
                                        var found =
                                            new TechnologyByEscapedFullNameQuery { EscapedFullName = potentialFullName }.Execute(sess);
                                        if (found == null)
                                        {
                                            found = new Technology { Name = token.Replace("_", " "), Parent = currentTechno };
                                            session.Save(found);
                                        }

                                        currentTechno = found;
                                    }
                                }

                                session.Flush();
                                return null;
                            });
                }
            }
            catch (Exception ex)
            {
                result.Add(new ImportOperationLineResult
                {
                    LineNumber = -1,
                    Status = LineResultStatus.Error,
                    Exception = ex,
                    Message = ex.Message
                });
            }

            return result;
        }
    }
}
