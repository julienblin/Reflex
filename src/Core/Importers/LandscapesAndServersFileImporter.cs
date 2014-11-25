// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesAndServersFileImporter.cs" company="CGI">
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
    internal class LandscapesAndServersFileImporter : BaseFileImporter
    {
        public LandscapesAndServersFileImporter()
            : base("LandscapesAndServers", 4)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.Environment, DomainValueCategory.ServerRole, DomainValueCategory.ServerStatus, DomainValueCategory.ServerType);
                CreateTechnologiesReferences(session, workbook);

                var lanscapeModel = new LandscapeModel();
                var ws = workbook.Worksheets.Add("Landscapes");
                lanscapeModel.Prepare(ws, "Landscapes");

                var serverModel = new ServerModel();
                ws = workbook.Worksheets.Add("Servers");
                serverModel.Prepare(ws, "Servers");

                var serverTechnoModel = new ServerTechnoModel();
                ws = workbook.Worksheets.Add("Servers-Techno");
                serverTechnoModel.Prepare(ws, "Servers-Techno");

                var dbInstanceModel = new DbInstanceModel();
                ws = workbook.Worksheets.Add("DbInstances");
                dbInstanceModel.Prepare(ws, "DbInstances");

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "LandscapesAndServers-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.Environment, DomainValueCategory.ServerRole, DomainValueCategory.ServerStatus, DomainValueCategory.ServerType);
                CreateTechnologiesReferences(session, workbook);

                var lanscapeModel = new LandscapeModel();
                var ws = workbook.Worksheets.Add("Landscapes");
                lanscapeModel.Prepare(ws, "Landscapes");

                var landscapes = session.QueryOver<Landscape>().OrderBy(l => l.Name).Asc.List();
                var row = ws.Row(4);
                foreach (var landscape in landscapes)
                {
                    lanscapeModel = new LandscapeModel();
                    lanscapeModel.FromEntity(landscape).ToRow(row);
                    row = row.RowBelow();
                }

                var serverModel = new ServerModel();
                ws = workbook.Worksheets.Add("Servers");
                serverModel.Prepare(ws, "Servers");

                var servers = session.QueryOver<Server>()
                                     .OrderBy(s => s.Name).Asc
                                     .Fetch(s => s.Environment).Eager
                                     .Fetch(s => s.Role).Eager
                                     .Fetch(s => s.Status).Eager
                                     .Fetch(s => s.Type).Eager
                                     .Fetch(s => s.Landscape).Eager
                                     .List();
                row = ws.Row(4);
                foreach (var server in servers)
                {
                    serverModel = new ServerModel();
                    serverModel.FromEntity(server).ToRow(row);
                    row = row.RowBelow();
                }

                var serverTechnoModel = new ServerTechnoModel();
                ws = workbook.Worksheets.Add("Servers-Techno");
                serverTechnoModel.Prepare(ws, "Servers-Techno");

                var serverTechnoLinks = session.QueryOver<ServerTechnoLink>()
                                               .Fetch(stl => stl.Technology).Eager
                                               .JoinQueryOver(stl => stl.Server).OrderBy(s => s.Name).Asc
                                               .List();
                row = ws.Row(4);
                foreach (var serverTechnoLink in serverTechnoLinks)
                {
                    serverTechnoModel = new ServerTechnoModel();
                    serverTechnoModel.FromEntity(serverTechnoLink).ToRow(row);
                    row = row.RowBelow();
                }

                var dbInstanceModel = new DbInstanceModel();
                ws = workbook.Worksheets.Add("DbInstances");
                dbInstanceModel.Prepare(ws, "DbInstances");

                var dbInstances = session.QueryOver<DbInstance>()
                                         .Fetch(db => db.TechnologyLinks).Eager
                                         .Fetch(db => db.TechnologyLinks.First().Technology).Eager
                                         .JoinQueryOver(db => db.Server).OrderBy(s => s.Name).Asc
                                         .List();

                row = ws.Row(4);
                foreach (var dbInstance in dbInstances)
                {
                    dbInstanceModel = new DbInstanceModel();
                    dbInstanceModel.FromEntity(dbInstance).ToRow(row);
                    row = row.RowBelow();
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("LandscapesAndServers-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    var ws = workbook.Worksheet("Landscapes");
                    if (ws != null)
                        Import<Landscape, LandscapeModel>(session, result, ws, (sess, model) => sess.QueryOver<Landscape>().Where(s => s.Name == model.Name).SingleOrDefault());

                    ws = workbook.Worksheet("Servers");
                    if (ws != null)
                        Import<Server, ServerModel>(session, result, ws, (sess, model) => sess.QueryOver<Server>().Where(s => s.Name == model.Name).SingleOrDefault());

                    ws = workbook.Worksheet("Servers-Techno");
                    if (ws != null)
                        Import<ServerTechnoLink, ServerTechnoModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var techno = new TechnologyByEscapedFullNameQuery { EscapedFullName = model.Technology }.Execute(session);
                                if (techno == null) return null;

                                return sess.QueryOver<ServerTechnoLink>()
                                           .Where(stl => stl.Technology.Id == techno.Id)
                                           .JoinQueryOver(stl => stl.Server).Where(s => s.Name == model.Server)
                                           .SingleOrDefault();
                            });

                    ws = workbook.Worksheet("DbInstances");
                    if (ws != null)
                        Import<DbInstance, DbInstanceModel>(
                            session,
                            result,
                            ws,
                            (sess, model) =>
                            {
                                var query = sess.QueryOver<DbInstance>();
                                query.JoinQueryOver(db => db.Server).Where(s => s.Name == model.Server);
                                query.Where(db => db.Name == model.Name);
                                return query.SingleOrDefault();
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
