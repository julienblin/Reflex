// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationModel.cs" company="CGI">
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
    internal class IntegrationModel : BaseImporterModel<Integration>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string AppSource { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string AppDest { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Nature { get; set; }

        public string Description { get; set; }

        public string DataDescription { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Frequency { get; set; }

        public string Comments { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Integration");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("AppSource").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("AppDest").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Nature");
            ws.Cell(3, ci++).SetValue("Description");
            ws.Cell(3, ci++).SetValue("DataDescription");
            ws.Cell(3, ci++).SetValue("Frequency");
            ws.Cell(3, ci++).SetValue("Comments");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;
            ws.Column(ci++).Width = 100;
            ws.Column(ci++).Width = 50;
            ws.Column(ci++).Width = 100;

            SetColumnDataValidation(ws, 1, "Applications");
            SetColumnDataValidation(ws, 2, "Applications");
            SetColumnDataValidation(ws, 4, "IntegrationNature");

            ws.Range(4, 5, 10000, 5).Style.Alignment.SetWrapText();
            ws.Range(4, 6, 10000, 6).Style.Alignment.SetWrapText();
            ws.Range(4, 7, 10000, 7).Style.Alignment.SetWrapText();
            ws.Range(4, 8, 10000, 8).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(AppSource ?? string.Empty);
            row.Cell(ci++).SetValue(AppDest ?? string.Empty);
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Nature ?? string.Empty);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            row.Cell(ci++).SetValue(DataDescription ?? string.Empty);
            row.Cell(ci++).SetValue(Frequency ?? string.Empty);
            row.Cell(ci++).SetValue(Comments ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            AppSource = row.Cell(ci++).GetString();
            AppDest = row.Cell(ci++).GetString();
            Name = row.Cell(ci++).GetString();
            Nature = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            DataDescription = row.Cell(ci++).GetString();
            Frequency = row.Cell(ci++).GetString();
            Comments = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Integration entity)
        {
            entity.AppSource = session.QueryOver<Application>().Where(a => a.Name == AppSource).SingleOrDefault();
            if (entity.AppSource == null)
                throw new ReferenceNotFoundException("AppSource", AppSource);

            entity.AppDest = session.QueryOver<Application>().Where(a => a.Name == AppDest).SingleOrDefault();
            if (entity.AppDest == null)
                throw new ReferenceNotFoundException("AppDest", AppDest);

            entity.Name = Name;

            if (!string.IsNullOrEmpty(Nature))
                entity.Nature = GetDomainValueReference(session, DomainValueCategory.IntegrationNature, "Nature", Nature);

            entity.Description = Description;
            entity.DataDescription = DataDescription;
            entity.Frequency = Frequency;
            entity.Comments = Comments;
            return this;
        }

        public override IImporterModel FromEntity(Integration entity)
        {
            AppSource = entity.AppSource == null ? string.Empty : entity.AppSource.Name;
            AppDest = entity.AppDest == null ? string.Empty : entity.AppDest.Name;
            Name = entity.Name;
            Nature = entity.Nature == null ? string.Empty : entity.Nature.Name;
            Description = entity.Description;
            DataDescription = entity.DataDescription;
            Frequency = entity.Frequency;
            Comments = entity.Comments;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
