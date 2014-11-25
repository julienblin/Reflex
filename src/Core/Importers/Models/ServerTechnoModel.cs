// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTechnoModel.cs" company="CGI">
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
    internal class ServerTechnoModel : BaseImporterModel<ServerTechnoLink>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Server { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Technology { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Servers-Technologies");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Server").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Technology").Style.Font.SetBold();

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;
            
            SetColumnDataValidation(ws, 1, "ServersNamesDynamic");
            SetColumnDataValidation(ws, 2, "Technologies");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Server ?? string.Empty);
            row.Cell(ci++).SetValue(Technology ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Server = row.Cell(ci++).GetString();
            Technology = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, ServerTechnoLink entity)
        {
            entity.Server = session.QueryOver<Server>().Where(s => s.Name == Server).SingleOrDefault();
            if (entity.Server == null)
                throw new ReferenceNotFoundException("Server", Server);

            entity.Technology = new TechnologyByEscapedFullNameQuery { EscapedFullName = Technology }.Execute(session);
            if (entity.Technology == null)
                throw new ReferenceNotFoundException("Technology", Technology);
            return this;
        }

        public override IImporterModel FromEntity(ServerTechnoLink entity)
        {
            Server = entity.Server != null ? entity.Server.Name : string.Empty;
            Technology = entity.Technology != null ? entity.Technology.GetEscapedFullName() : string.Empty;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
