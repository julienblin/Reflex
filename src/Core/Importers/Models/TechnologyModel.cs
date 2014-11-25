// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyModel.cs" company="CGI">
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
using CGI.Reflex.Core.Queries;
using ClosedXML.Excel;
using NHibernate;

namespace CGI.Reflex.Core.Importers.Models
{
    // ReSharper disable RedundantAssignment
    internal class TechnologyModel : BaseImporterModel<Technology>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string FullName { get; set; }

        public DateTime? EndOfSupport { get; set; }

        public string TechnologyType { get; set; }

        public string Description { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Technology");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("EndOfSupport");
            ws.Cell(3, ci++).SetValue("TechnologyType");
            ws.Cell(3, ci++).SetValue("Description");
            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 75;
            ws.Column(ci++).Width = 15;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDataValidation(ws, 3, "TechnologyType");
            ws.Range(4, 2, 10000, 2).Style.DateFormat.SetFormat(DateFormat);
            ws.Range(4, 4, RowLimit, 4).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(FullName);
            row.Cell(ci++).Value = EndOfSupport.HasValue ? EndOfSupport : null;
            row.Cell(ci++).SetValue(TechnologyType ?? string.Empty);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            FullName = row.Cell(ci++).GetString();
            if (!string.IsNullOrEmpty(row.Cell(ci).GetString()))
                EndOfSupport = row.Cell(ci).GetDateTime();
            ci++;
            TechnologyType = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Technology entity)
        {
            entity.Name = FullName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last().Replace("_", " ");
            var parentFullName = FullName.Trim().Substring(0, FullName.Trim().Length - entity.Name.Length).Trim();
            if (!string.IsNullOrEmpty(parentFullName))
            {
                entity.Parent = new TechnologyByEscapedFullNameQuery { EscapedFullName = parentFullName }.Execute(session);
                if (entity.Parent == null)
                    throw new ReferenceNotFoundException("Name", parentFullName);
            }

            entity.EndOfSupport = EndOfSupport;
            entity.TechnologyType = GetDomainValueReference(session, DomainValueCategory.TechnologyType, "TechnologyType", TechnologyType);
            entity.Description = Description;
            return this;
        }

        public override IImporterModel FromEntity(Technology entity)
        {
            FullName = entity.GetEscapedFullName();
            EndOfSupport = entity.EndOfSupport;
            TechnologyType = entity.TechnologyType == null ? string.Empty : entity.TechnologyType.Name;
            Description = entity.Description;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
