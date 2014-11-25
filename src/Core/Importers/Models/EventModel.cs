// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventModel.cs" company="CGI">
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
    internal class EventModel : BaseImporterModel<Event>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Application { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Source { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Type { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Contact");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Application").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Description");
            ws.Cell(3, ci++).SetValue("Source");
            ws.Cell(3, ci++).SetValue("Date");
            ws.Cell(3, ci++).SetValue("Type");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 17;
            ws.Column(ci++).Width = 25;

            SetColumnDataValidation(ws, 1, "ApplicationsNamesDynamic");
            SetColumnDataValidation(ws, 5, "EventType");
            ws.Range(4, 2, 10000, 2).Style.Alignment.SetWrapText();
            ws.Range(4, 4, 10000, 4).Style.DateFormat.SetFormat(DateFormat);
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Application ?? string.Empty);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            row.Cell(ci++).SetValue(Source ?? string.Empty);
            row.Cell(ci++).Value = Date.HasValue ? Date : null;
            row.Cell(ci++).SetValue(Type ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Application = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            Source = row.Cell(ci++).GetString();
            if (!string.IsNullOrEmpty(row.Cell(ci).GetString()))
                Date = row.Cell(ci).GetDateTime();
            ci++;
            Type = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Event entity)
        {
            entity.Application = session.QueryOver<Application>().Where(a => a.Name == Application).SingleOrDefault();
            if (entity.Application == null)
                throw new ReferenceNotFoundException("Application", Application);

            entity.Description = Description;
            entity.Source = Source;

            if (Date.HasValue)
                entity.Date = Date.Value;
            
            if (!string.IsNullOrEmpty(Type))
                entity.Type = GetDomainValueReference(session, DomainValueCategory.EventType, "Type", Type);
            return this;
        }

        public override IImporterModel FromEntity(Event entity)
        {
            Application = entity.Application == null ? string.Empty : entity.Application.Name;
            Description = entity.Description;
            Source = entity.Source;
            Date = entity.Date;
            Type = entity.Type == null ? string.Empty : entity.Type.Name;
            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
