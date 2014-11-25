// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeModel.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    // ReSharper disable RedundantAssignment
    internal class LandscapeModel : BaseImporterModel<Landscape>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Landscape");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Description");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDynamicRange(ws, "LanscapesNamesDynamic", "A");
            ws.Range(4, 2, RowLimit, 2).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Name = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Landscape entity)
        {
            entity.Name = Name;
            entity.Description = Description;
            return this;
        }

        public override IImporterModel FromEntity(Landscape entity)
        {
            Name = entity.Name ?? string.Empty;
            Description = entity.Description ?? string.Empty;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
