// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesFileImporter.cs" company="CGI">
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

namespace CGI.Reflex.Core.Importers
{
    internal class DomainValuesFileImporter : BaseFileImporter
    {
        public DomainValuesFileImporter()
            : base("DomainValues", 1)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                var model = new DomainValueModel();
                foreach (var category in (DomainValueCategory[])Enum.GetValues(typeof(DomainValueCategory)))
                {
                    var ws = workbook.Worksheets.Add(category.ToString());
                    model.Prepare(ws, category.ToString());
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "DomainValues-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                var model = new DomainValueModel();
                var domainValues = session.QueryOver<DomainValue>().List().GroupBy(dv => dv.Category).ToDictionary(g => g.Key, h => h.ToList());

                foreach (var category in domainValues.Keys.OrderBy(k => k.ToString()))
                {
                    var ws = workbook.Worksheets.Add(category.ToString());
                    model.Prepare(ws, category.ToString());

                    var row = ws.Row(4);
                    foreach (var domainValue in domainValues[category].OrderBy(dv => dv.DisplayOrder))
                    {
                        model = new DomainValueModel();
                        model.FromEntity(domainValue).ToRow(row);
                        row = row.RowBelow();
                    }
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("DomainValues-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    foreach (var ws in workbook.Worksheets)
                    {
                        DomainValueCategory category;
                        if (!Enum.TryParse(ws.Name, true, out category))
                        {
                            result.Add(new ImportOperationLineResult
                            {
                                Section = ws.Name,
                                LineNumber = -1,
                                Status = LineResultStatus.Rejected,
                                Message = string.Format("Unrecognized category: {0}", ws.Name)
                            });
                            continue;
                        }

                        Import<DomainValue, DomainValueModel>(session, result, ws, (sess, model) => new DomainValueQuery { Category = category, Name = model.Name }.SingleOrDefault(sess));
                    }
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

        protected override void ComplementaryModelInfo(object model, IXLRow row, IXLWorksheet ws)
        {
            ((DomainValueModel)model).Category = (DomainValueCategory)Enum.Parse(typeof(DomainValueCategory), ws.Name);
        }
    }
}
