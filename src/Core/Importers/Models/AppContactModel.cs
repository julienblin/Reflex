// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    // ReSharper disable RedundantAssignment
    internal class AppContactModel : BaseImporterModel<AppContactLink>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Application { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Contact { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Applications-Contacts");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Application").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Contact").Style.Font.SetBold();

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;

            SetColumnDataValidation(ws, 1, "ApplicationsNamesDynamic");
            SetColumnDataValidation(ws, 2, "Contacts");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Application ?? string.Empty);
            row.Cell(ci++).SetValue(Contact ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Application = row.Cell(ci++).GetString();
            Contact = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, AppContactLink entity)
        {
            entity.Application = session.QueryOver<Application>().Where(a => a.Name == Application).SingleOrDefault();
            if (entity.Application == null)
                throw new ReferenceNotFoundException("Application", Application);

            entity.Contact = new ContactByEscapedFullNameQuery { EscapedFullName = Contact }.Execute(session);
            if (entity.Contact == null)
                throw new ReferenceNotFoundException("Contact", Contact);
            return this;
        }

        public override IImporterModel FromEntity(AppContactLink entity)
        {
            Application = entity.Application != null ? entity.Application.Name : string.Empty;
            Contact = entity.Contact != null ? entity.Contact.GetEscapedFullName() : string.Empty;
            return this;
        }
        //// ReSharper restore RedundantAssignment
    }
}
