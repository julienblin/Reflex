// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    // ReSharper disable RedundantAssignment
    internal class ServerModel : BaseImporterModel<Server>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Alias { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Environment { get; set; }

        public DateTime? EvergreenDate { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Landscape { get; set; }

        public string Comments { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Landscape");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Alias");
            ws.Cell(3, ci++).SetValue("Environment");
            ws.Cell(3, ci++).SetValue("EvergreenDate");
            ws.Cell(3, ci++).SetValue("Role");
            ws.Cell(3, ci++).SetValue("Status");
            ws.Cell(3, ci++).SetValue("Type");
            ws.Cell(3, ci++).SetValue("Landscape");
            ws.Cell(3, ci++).SetValue("Comments");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 17;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDynamicRange(ws, "ServersNamesDynamic", "A");

            SetColumnDataValidation(ws, 3, "Environment");
            SetColumnDataValidation(ws, 5, "ServerRole");
            SetColumnDataValidation(ws, 6, "ServerStatus");
            SetColumnDataValidation(ws, 7, "ServerType");
            SetColumnDataValidation(ws, 8, "LanscapesNamesDynamic");

            ws.Range(4, 4, 10000, 4).Style.DateFormat.SetFormat(DateFormat);
            ws.Range(4, 9, RowLimit, 9).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Alias ?? string.Empty);
            row.Cell(ci++).SetValue(Environment ?? string.Empty);
            row.Cell(ci++).Value = EvergreenDate.HasValue ? EvergreenDate : null;
            row.Cell(ci++).SetValue(Role ?? string.Empty);
            row.Cell(ci++).SetValue(Status ?? string.Empty);
            row.Cell(ci++).SetValue(Type ?? string.Empty);
            row.Cell(ci++).SetValue(Landscape ?? string.Empty);
            row.Cell(ci++).SetValue(Comments ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Name = row.Cell(ci++).GetString();
            Alias = row.Cell(ci++).GetString();
            Environment = row.Cell(ci++).GetString();
            if (!string.IsNullOrEmpty(row.Cell(ci).GetString()))
                EvergreenDate = row.Cell(ci).GetDateTime();
            ci++;
            Role = row.Cell(ci++).GetString();
            Status = row.Cell(ci++).GetString();
            Type = row.Cell(ci++).GetString();
            Landscape = row.Cell(ci++).GetString();
            Comments = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Server entity)
        {
            entity.Name = Name;
            entity.Alias = Alias;
            entity.Environment = GetDomainValueReference(session, DomainValueCategory.Environment, "Environment", Environment);
            entity.EvergreenDate = EvergreenDate;
            if (!string.IsNullOrEmpty(Role))
                entity.Role = GetDomainValueReference(session, DomainValueCategory.ServerRole, "Role", Role);
            if (!string.IsNullOrEmpty(Status))
                entity.Status = GetDomainValueReference(session, DomainValueCategory.ServerStatus, "Status", Status);
            if (!string.IsNullOrEmpty(Type))
                entity.Type = GetDomainValueReference(session, DomainValueCategory.ServerType, "Type", Type);
            if (!string.IsNullOrEmpty(Landscape))
            {
                entity.Landscape = session.QueryOver<Landscape>().Where(l => l.Name == Landscape).SingleOrDefault();
                if (entity.Landscape == null)
                    throw new ReferenceNotFoundException("Landscape", Landscape);
            }

            entity.Comments = Comments;
            return this;
        }

        public override IImporterModel FromEntity(Server entity)
        {
            Name = entity.Name ?? string.Empty;
            Alias = entity.Alias ?? string.Empty;
            Environment = entity.Environment == null ? string.Empty : entity.Environment.Name;
            EvergreenDate = entity.EvergreenDate;
            Role = entity.Role == null ? string.Empty : entity.Role.Name;
            Status = entity.Status == null ? string.Empty : entity.Status.Name;
            Type = entity.Type == null ? string.Empty : entity.Type.Name;
            Landscape = entity.Landscape == null ? string.Empty : entity.Landscape.Name;
            Comments = entity.Comments ?? string.Empty;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
