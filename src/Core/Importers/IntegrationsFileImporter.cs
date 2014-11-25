// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsFileImporter.cs" company="CGI">
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
    internal class IntegrationsFileImporter : BaseFileImporter
    {
        public IntegrationsFileImporter()
            : base("Integrations", 6)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.IntegrationNature);
                CreateApplicationsReferences(session, workbook);
                CreateTechnologiesReferences(session, workbook);

                var integrationModel = new IntegrationModel();
                var ws = workbook.Worksheets.Add("Integrations");
                integrationModel.Prepare(ws, "Integrations");

                var intTechnoModel = new IntegrationTechnoModel();
                ws = workbook.Worksheets.Add("Integrations-Techno");
                intTechnoModel.Prepare(ws, "Integrations-Techno");

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "Integrations-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.IntegrationNature);
                CreateApplicationsReferences(session, workbook);
                CreateTechnologiesReferences(session, workbook);

                var integrationModel = new IntegrationModel();
                var ws = workbook.Worksheets.Add("Integrations");
                integrationModel.Prepare(ws, "Integrations");

                var integrations = session.QueryOver<Integration>()
                                          .Fetch(i => i.AppDest).Eager
                                          .JoinQueryOver(i => i.AppSource).OrderBy(a => a.Name).Asc
                                          .List();

                var row = ws.Row(4);
                foreach (var integration in integrations)
                {
                    integrationModel = new IntegrationModel();
                    integrationModel.FromEntity(integration).ToRow(row);
                    row = row.RowBelow();
                }

                var intTechnoModel = new IntegrationTechnoModel();
                ws = workbook.Worksheets.Add("Integrations-Techno");
                intTechnoModel.Prepare(ws, "Integrations-Techno");

                var intTechnoLinks = session.QueryOver<IntegrationTechnoLink>()
                                            .Fetch(itl => itl.Technology).Eager
                                            .JoinQueryOver(itl => itl.Integration).OrderBy(a => a.Name).Asc
                                            .List();

                row = ws.Row(4);
                foreach (var intTechnoLink in intTechnoLinks)
                {
                    intTechnoModel = new IntegrationTechnoModel();
                    intTechnoModel.FromEntity(intTechnoLink).ToRow(row);
                    row = row.RowBelow();
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("Integrations-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    var ws = workbook.Worksheet("Integrations");
                    if (ws != null)
                        Import<Integration, IntegrationModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var query = sess.QueryOver<Integration>();
                                query.JoinQueryOver(i => i.AppSource).Where(a => a.Name == model.AppSource);
                                query.JoinQueryOver(i => i.AppDest).Where(a => a.Name == model.AppDest);
                                query.Where(i => i.Name == model.Name);
                                return query.SingleOrDefault();
                            });

                    ws = workbook.Worksheet("Integrations-Techno");
                    if (ws != null)
                        Import<IntegrationTechnoLink, IntegrationTechnoModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var techno = new TechnologyByEscapedFullNameQuery { EscapedFullName = model.Technology }.Execute(sess);
                                if (techno == null) return null;

                                var itlQuery = sess.QueryOver<IntegrationTechnoLink>()
                                                   .Where(itl => itl.Technology.Id == techno.Id)
                                                   .JoinQueryOver(itl => itl.Integration)
                                                   .Where(i => i.Name == model.Name);
                                itlQuery.JoinQueryOver(i => i.AppSource).Where(a => a.Name == model.AppSource);
                                itlQuery.JoinQueryOver(i => i.AppDest).Where(a => a.Name == model.AppDest);

                                return itlQuery.SingleOrDefault();
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

        private void CreateApplicationsReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("ApplicationsReferences");

            var appNames = session.QueryOver<Application>()
                                  .OrderBy(s => s.Name).Asc
                                  .Select(s => s.Name)
                                  .List<string>();
            var row = ws.Row(1);
            foreach (var appName in appNames)
            {
                row.Cell(1).SetValue(appName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();

            ws.Workbook.NamedRanges.Add("Applications", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }
    }
}
