// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsFileImporter.cs" company="CGI">
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
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Importers
{
    internal class ApplicationsFileImporter : BaseFileImporter
    {
        public ApplicationsFileImporter()
            : base("Applications", 5)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(
                    session,
                    workbook,
                    DomainValueCategory.ApplicationType,
                    DomainValueCategory.ApplicationStatus,
                    DomainValueCategory.ApplicationCriticity,
                    DomainValueCategory.ApplicationUserRange,
                    DomainValueCategory.ApplicationCategory,
                    DomainValueCategory.ApplicationCertification,
                    DomainValueCategory.EventType);
                CreateTechnologiesReferences(session, workbook);
                CreateSectorsReferences(session, workbook);
                CreateServersReferences(session, workbook);
                CreateDbInstancesReferences(session, workbook);
                CreateContactsReferences(session, workbook);

                var appModel = new ApplicationModel();
                var ws = workbook.Worksheets.Add("Applications");
                appModel.Prepare(ws, "Applications");

                var appTechnoModel = new AppTechnoModel();
                ws = workbook.Worksheets.Add("Applications-Techno");
                appTechnoModel.Prepare(ws, "Applications-Techno");

                var appServerModel = new AppServerModel();
                ws = workbook.Worksheets.Add("Applications-Servers");
                appServerModel.Prepare(ws, "Applications-Servers");

                var appDbInstanceModel = new AppDbInstanceModel();
                ws = workbook.Worksheets.Add("Applications-DbInstances");
                appDbInstanceModel.Prepare(ws, "Applications-DbInstances");

                var appContactModel = new AppContactModel();
                ws = workbook.Worksheets.Add("Applications-Contacts");
                appContactModel.Prepare(ws, "Applications-Contacts");

                var eventModel = new EventModel();
                ws = workbook.Worksheets.Add("Events");
                eventModel.Prepare(ws, "Events");

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "Applications-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(
                    session,
                    workbook,
                    DomainValueCategory.ApplicationType,
                    DomainValueCategory.ApplicationStatus,
                    DomainValueCategory.ApplicationCriticity,
                    DomainValueCategory.ApplicationUserRange,
                    DomainValueCategory.ApplicationCategory,
                    DomainValueCategory.ApplicationCertification,
                    DomainValueCategory.EventType);
                CreateTechnologiesReferences(session, workbook);
                CreateSectorsReferences(session, workbook);
                CreateServersReferences(session, workbook);
                CreateDbInstancesReferences(session, workbook);
                CreateContactsReferences(session, workbook);

                var appModel = new ApplicationModel();
                var ws = workbook.Worksheets.Add("Applications");
                appModel.Prepare(ws, "Applications");

                var applications = session.QueryOver<Application>()
                                          .Fetch(a => a.ApplicationType).Eager
                                          .Fetch(a => a.AppInfo).Eager
                                          .Fetch(a => a.AppInfo.Status).Eager
                                          .Fetch(a => a.AppInfo.Criticity).Eager
                                          .Fetch(a => a.AppInfo.UserRange).Eager
                                          .Fetch(a => a.AppInfo.Category).Eager
                                          .Fetch(a => a.AppInfo.Certification).Eager
                                          .Fetch(a => a.AppInfo.Sector).Eager
                                          .OrderBy(a => a.Name).Asc
                                          .List();
                var row = ws.Row(4);
                foreach (var app in applications)
                {
                    appModel = new ApplicationModel();
                    appModel.FromEntity(app).ToRow(row);
                    row = row.RowBelow();
                }

                var appTechnoModel = new AppTechnoModel();
                ws = workbook.Worksheets.Add("Applications-Techno");
                appTechnoModel.Prepare(ws, "Applications-Techno");

                var appTechnoLinks = session.QueryOver<AppTechnoLink>()
                                            .Fetch(atl => atl.Technology).Eager
                                            .JoinQueryOver(atl => atl.Application).OrderBy(a => a.Name).Asc
                                            .List();

                row = ws.Row(4);
                foreach (var appTechnoLink in appTechnoLinks)
                {
                    appTechnoModel = new AppTechnoModel();
                    appTechnoModel.FromEntity(appTechnoLink).ToRow(row);
                    row = row.RowBelow();
                }

                var appServerModel = new AppServerModel();
                ws = workbook.Worksheets.Add("Applications-Servers");
                appServerModel.Prepare(ws, "Applications-Servers");

                var appServerLinks = session.QueryOver<AppServerLink>()
                                            .Fetch(asl => asl.Server).Eager
                                            .JoinQueryOver(asl => asl.Application).OrderBy(a => a.Name).Asc
                                            .List();

                row = ws.Row(4);
                foreach (var appServerLink in appServerLinks)
                {
                    appServerModel = new AppServerModel();
                    appServerModel.FromEntity(appServerLink).ToRow(row);
                    row = row.RowBelow();
                }

                var appDbInstanceModel = new AppDbInstanceModel();
                ws = workbook.Worksheets.Add("Applications-DbInstances");
                appDbInstanceModel.Prepare(ws, "Applications-DbInstances");

                var appDbInstanceLinks = session.QueryOver<AppDbInstanceLink>()
                                            .Fetch(asl => asl.DbInstances).Eager
                                            .Fetch(asl => asl.DbInstances.Server).Eager
                                            .JoinQueryOver(asl => asl.Application).OrderBy(a => a.Name).Asc
                                            .List();

                row = ws.Row(4);
                foreach (var appDbInstanceLink in appDbInstanceLinks)
                {
                    appDbInstanceModel = new AppDbInstanceModel();
                    appDbInstanceModel.FromEntity(appDbInstanceLink).ToRow(row);
                    row = row.RowBelow();
                }

                var appContactModel = new AppContactModel();
                ws = workbook.Worksheets.Add("Applications-Contacts");
                appContactModel.Prepare(ws, "Applications-Contacts");

                var appContactLinks = session.QueryOver<AppContactLink>()
                                                  .Fetch(acl => acl.Contact).Eager
                                                  .JoinQueryOver(asl => asl.Application).OrderBy(a => a.Name).Asc
                                                  .List();

                row = ws.Row(4);
                foreach (var appContactLink in appContactLinks)
                {
                    appContactModel = new AppContactModel();
                    appContactModel.FromEntity(appContactLink).ToRow(row);
                    row = row.RowBelow();
                }

                var eventModel = new EventModel();
                ws = workbook.Worksheets.Add("Events");
                eventModel.Prepare(ws, "Events");

                var events = session.QueryOver<Event>()
                                    .Fetch(e => e.Application).Eager
                                    .Fetch(e => e.Type).Eager
                                    .List();

                row = ws.Row(4);
                foreach (var @event in events)
                {
                    eventModel = new EventModel();
                    eventModel.FromEntity(@event).ToRow(row);
                    row = row.RowBelow();
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("Applications-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    var ws = workbook.Worksheet("Applications");
                    if (ws != null)
                        Import<Application, ApplicationModel>(session, result, ws, (sess, model) => sess.QueryOver<Application>().Where(a => a.Name == model.Name).SingleOrDefault());

                    ws = workbook.Worksheet("Applications-Techno");
                    if (ws != null)
                        Import<AppTechnoLink, AppTechnoModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var techno = new TechnologyByEscapedFullNameQuery { EscapedFullName = model.Technology }.Execute(sess);
                                if (techno == null) return null;

                                return sess.QueryOver<AppTechnoLink>()
                                           .Where(atl => atl.Technology.Id == techno.Id)
                                           .JoinQueryOver(atl => atl.Application).Where(a => a.Name == model.Application)
                                           .SingleOrDefault();
                            });

                    ws = workbook.Worksheet("Applications-Servers");
                    if (ws != null)
                        Import<AppServerLink, AppServerModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var query = sess.QueryOver<AppServerLink>();
                                query.JoinQueryOver(asl => asl.Server).Where(s => s.Name == model.Server);
                                query.JoinQueryOver(asl => asl.Application).Where(a => a.Name == model.Application);
                                return query.SingleOrDefault();
                            });

                    ws = workbook.Worksheet("Applications-DbInstances");
                    if (ws != null)
                        Import<AppDbInstanceLink, AppDbInstanceModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var query = sess.QueryOver<AppDbInstanceLink>();
                                query.JoinQueryOver(adil => adil.DbInstances)
                                     .Where(dbi => dbi.Name == model.DbInstance)
                                     .JoinQueryOver(dbi => dbi.Server).Where(s => s.Name == model.Server);
                                query.JoinQueryOver(asl => asl.Application).Where(a => a.Name == model.Application);
                                return query.SingleOrDefault();
                            });

                    ws = workbook.Worksheet("Applications-Contacts");
                    if (ws != null)
                        Import<AppContactLink, AppContactModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var contact = new ContactByEscapedFullNameQuery { EscapedFullName = model.Contact }.Execute(sess);
                                if (contact == null) return null;

                                var query = sess.QueryOver<AppContactLink>();
                                query.Where(acl => acl.Contact.Id == contact.Id);
                                query.JoinQueryOver(acl => acl.Application).Where(a => a.Name == model.Application);
                                return query.SingleOrDefault();
                            });

                    ws = workbook.Worksheet("Events");
                    if (ws != null)
                        Import<Event, EventModel>(session, result, ws, (sess, model) => null);
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

        private void CreateSectorsReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("SectorsReferences");

            var sectorNames = session.QueryOver<Sector>()
                                     .OrderBy(s => s.Name).Asc
                                     .Select(s => s.Name)
                                     .List<string>();
            var row = ws.Row(1);
            foreach (var sectorName in sectorNames)
            {
                row.Cell(1).SetValue(sectorName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();

            ws.Workbook.NamedRanges.Add("Sectors", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }

        private void CreateServersReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("ServersReferences");

            var serverNames = session.QueryOver<Server>()
                                     .OrderBy(s => s.Name).Asc
                                     .Select(s => s.Name)
                                     .List<string>();
            var row = ws.Row(1);
            foreach (var serverName in serverNames)
            {
                row.Cell(1).SetValue(serverName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();

            ws.Workbook.NamedRanges.Add("Servers", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }

        private void CreateDbInstancesReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("DbInstancesReferences");

            var dbInstanceNames = session.QueryOver<DbInstance>()
                                          .OrderBy(dbi => dbi.Name).Asc
                                          .SelectList(x => x.Select(Projections.Distinct(Projections.Property<DbInstance>(dbi => dbi.Name))))
                                          .List<string>();
            var row = ws.Row(1);
            foreach (var dbInstanceName in dbInstanceNames)
            {
                row.Cell(1).SetValue(dbInstanceName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();

            ws.Workbook.NamedRanges.Add("DbInstances", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }

        private void CreateContactsReferences(ISession session, XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("ContactsReferences");

            var contactNames = session.QueryOver<Contact>()
                                     .OrderBy(c => c.FirstName).Asc
                                     .ThenBy(c => c.LastName).Asc
                                     .ThenBy(c => c.Company).Asc
                                     .List()
                                     .Select(c => c.GetEscapedFullName())
                                     .OrderBy(s => s);
            var row = ws.Row(1);
            foreach (var contactName in contactNames)
            {
                row.Cell(1).SetValue(contactName);
                row = row.RowBelow();
            }

            if (row.RowNumber() == 1)
                row = row.RowBelow();

            ws.Workbook.NamedRanges.Add("Contacts", ws.Range(1, 1, row.RowNumber() - 1, 1));
            ws.Hide();
        }
    }
}
