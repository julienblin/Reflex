// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueModel.cs" company="CGI">
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
    internal class DomainValueModel : BaseImporterModel<DomainValue>
    {
        public DomainValueCategory Category { get; set; }
        
        public int DisplayOrder { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(25, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        public string Description { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "DomainValue");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("DisplayOrder");
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).Value = "Description";
            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 15;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            ws.Range(4, 1, RowLimit, 1).Style.NumberFormat.SetNumberFormatId(1);
            ws.Range(4, 3, RowLimit, 3).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(DisplayOrder);
            row.Cell(ci++).SetValue(Name);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            DisplayOrder = row.Cell(ci++).GetValue<int>();
            Name = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, DomainValue entity)
        {
            entity.Category = Category;
            entity.DisplayOrder = DisplayOrder;
            entity.Name = Name;
            entity.Description = Description;
            return this;
        }

        public override IImporterModel FromEntity(DomainValue entity)
        {
            Category = entity.Category;
            DisplayOrder = entity.DisplayOrder;
            Name = entity.Name;
            Description = entity.Description;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
