// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceModel.cs" company="CGI">
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
    internal class DbInstanceModel : BaseImporterModel<DbInstance>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Server { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        public string Technology { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "DbInstance");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Server").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Technology");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDataValidation(ws, 1, "ServersNamesDynamic");
            SetColumnDataValidation(ws, 3, "Technologies");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Server ?? string.Empty);
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Technology ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Server = row.Cell(ci++).GetString();
            Name = row.Cell(ci++).GetString();
            Technology = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, DbInstance entity)
        {
            if (!string.IsNullOrEmpty(Server))
            {
                entity.Server = session.QueryOver<Server>().Where(s => s.Name == Server).SingleOrDefault();
                if (entity.Server == null)
                    throw new ReferenceNotFoundException("Server", Server);
            }

            entity.Name = Name;

            if (!string.IsNullOrEmpty(Technology))
            {
                var techno = new TechnologyByEscapedFullNameQuery { EscapedFullName = Technology }.Execute(session);
                if (techno == null)
                    throw new ReferenceNotFoundException("Technology", Technology);

                entity.SetTechnologyLinks(new[] { techno });
            }

            return this;
        }

        public override IImporterModel FromEntity(DbInstance entity)
        {
            Server = entity.Server == null ? string.Empty : entity.Server.Name;
            Name = entity.Name;
            Technology = entity.TechnologyLinks.Any() ? entity.TechnologyLinks.First().Technology.GetEscapedFullName() : string.Empty;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
