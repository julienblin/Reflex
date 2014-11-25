// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorsAndContactsFileImporter.cs" company="CGI">
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
    internal class SectorsAndContactsFileImporter : BaseFileImporter
    {
        public SectorsAndContactsFileImporter()
            : base("SectorsAndContacts", 2)
        {
        }

        protected override FileImporterResult GetTemplateImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.ContactType);

                var sectorModel = new SectorModel();
                var ws = workbook.Worksheets.Add("Sectors");
                sectorModel.Prepare(ws, "Sectors");

                var contactModel = new ContactModel();
                ws = workbook.Worksheets.Add("Contacts");
                contactModel.Prepare(ws, "Contacts");

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = "SectorsAndContacts-template.xlsx"
            };
        }

        protected override FileImporterResult ExportImpl(ISession session, Stream stream)
        {
            using (var workbook = new XLWorkbook())
            {
                CreateDomainValuesReferences(session, workbook, DomainValueCategory.ContactType);

                var sectorModel = new SectorModel();
                var ws = workbook.Worksheets.Add("Sectors");
                sectorModel.Prepare(ws, "Sectors");

                var sectors = new SectorHierarchicalQuery().List(session);
                var row = ws.Row(4);
                foreach (var sector in sectors.OrderBy(s => s.Name))
                {
                    sectorModel = new SectorModel();
                    sectorModel.FromEntity(sector).ToRow(row);
                    row = row.RowBelow();
                    row = ExportChilds<Sector, SectorModel>(sector, row);
                }

                var contactModel = new ContactModel();
                ws = workbook.Worksheets.Add("Contacts");
                contactModel.Prepare(ws, "Contacts");

                var contacts = session.QueryOver<Contact>()
                                      .Fetch(c => c.Type).Eager
                                      .Fetch(c => c.Sector).Eager
                                      .OrderBy(c => c.FirstName).Asc
                                      .ThenBy(c => c.LastName).Asc
                                      .ThenBy(c => c.Company).Asc
                                      .List();
                row = ws.Row(4);
                foreach (var contact in contacts)
                {
                    contactModel = new ContactModel();
                    contactModel.FromEntity(contact).ToRow(row);
                    row = row.RowBelow();
                }

                workbook.SaveAs(stream);
            }

            return new FileImporterResult
            {
                Stream = stream,
                ContentType = XlsxContentType,
                SuggestedFileName = string.Format("SectorsAndContacts-export-{0:yyyy-MM-dd}.xlsx", DateTime.Now)
            };
        }

        protected override IEnumerable<ImportOperationLineResult> ImportImpl(ISession session, Stream inputStream)
        {
            var result = new List<ImportOperationLineResult>();
            try
            {
                using (var workbook = new XLWorkbook(inputStream))
                {
                    var ws = workbook.Worksheet("Sectors");
                    if (ws != null)
                        Import<Sector, SectorModel>(session, result, ws, (sess, model) => sess.QueryOver<Sector>().Where(s => s.Name == model.Name).SingleOrDefault());

                    ws = workbook.Worksheet("Contacts");
                    if (ws != null)
                        Import<Contact, ContactModel>(
                            session,
                            result,
                            ws,
                            (sess, model) => session.QueryOver<Contact>().Where(c => c.FirstName == model.FirstName)
                                                 .And(c => c.LastName == model.LastName)
                                                 .And(c => c.Company == model.Company)
                                                 .SingleOrDefault());
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
