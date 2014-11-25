// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorModel.cs" company="CGI">
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
    internal class SectorModel : BaseImporterModel<Sector>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        public string ParentName { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Sector");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("ParentName");
            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;

            SetColumnDynamicRange(ws, "SectorsNamesDynamic", "A");
            SetColumnDataValidation(ws, 2, "SectorsNamesDynamic");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Name);
            row.Cell(ci++).SetValue(ParentName ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Name = row.Cell(ci++).GetString();
            ParentName = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Sector entity)
        {
            entity.Name = Name;
            if (!string.IsNullOrEmpty(ParentName))
            {
                entity.Parent = session.QueryOver<Sector>().Where(s => s.Name == ParentName).SingleOrDefault();
                if (entity.Parent == null)
                    throw new ReferenceNotFoundException("ParentName", ParentName);
            }

            return this;
        }

        public override IImporterModel FromEntity(Sector entity)
        {
            Name = entity.Name;
            ParentName = entity.Parent == null ? string.Empty : entity.Parent.Name;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
