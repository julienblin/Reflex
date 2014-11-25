// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServerModel.cs" company="CGI">
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
    internal class AppServerModel : BaseImporterModel<AppServerLink>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Application { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Server { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Applications-Servers");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Application").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Server").Style.Font.SetBold();

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;

            SetColumnDataValidation(ws, 1, "ApplicationsNamesDynamic");
            SetColumnDataValidation(ws, 2, "Servers");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Application ?? string.Empty);
            row.Cell(ci++).SetValue(Server ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Application = row.Cell(ci++).GetString();
            Server = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, AppServerLink entity)
        {
            entity.Application = session.QueryOver<Application>().Where(a => a.Name == Application).SingleOrDefault();
            if (entity.Application == null)
                throw new ReferenceNotFoundException("Application", Application);

            entity.Server = session.QueryOver<Server>().Where(s => s.Name == Server).SingleOrDefault();
            if (entity.Server == null)
                throw new ReferenceNotFoundException("Server", Server);
            return this;
        }

        public override IImporterModel FromEntity(AppServerLink entity)
        {
            Application = entity.Application != null ? entity.Application.Name : string.Empty;
            Server = entity.Server != null ? entity.Server.Name : string.Empty;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
