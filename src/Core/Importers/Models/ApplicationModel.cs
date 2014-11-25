// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationModel.cs" company="CGI">
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
    internal class ApplicationModel : BaseImporterModel<Application>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Code { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string ApplicationType { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Criticity { get; set; }

        public string UserRange { get; set; }

        public string Category { get; set; }

        public string Certification { get; set; }

        public string Sector { get; set; }

        public string MaintenanceWindow { get; set; }

        public string Notes { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Application");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Code");
            ws.Cell(3, ci++).SetValue("ApplicationType");
            ws.Cell(3, ci++).SetValue("Description");
            ws.Cell(3, ci++).SetValue("Status");
            ws.Cell(3, ci++).SetValue("Criticity");
            ws.Cell(3, ci++).SetValue("UserRange");
            ws.Cell(3, ci++).SetValue("Category");
            ws.Cell(3, ci++).SetValue("Certification");
            ws.Cell(3, ci++).SetValue("Sector");
            ws.Cell(3, ci++).SetValue("MaintenanceWindow");
            ws.Cell(3, ci++).SetValue("Notes");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 17;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 50;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 50;
            ws.Column(ci++).Width = 100;

            SetColumnDynamicRange(ws, "ApplicationsNamesDynamic", "A");

            SetColumnDataValidation(ws, 3, "ApplicationType");
            SetColumnDataValidation(ws, 5, "ApplicationStatus");
            SetColumnDataValidation(ws, 6, "ApplicationCriticity");
            SetColumnDataValidation(ws, 7, "ApplicationUserRange");
            SetColumnDataValidation(ws, 8, "ApplicationCategory");
            SetColumnDataValidation(ws, 9, "ApplicationCertification");
            SetColumnDataValidation(ws, 10, "Sectors");

            ws.Range(4, 4, RowLimit, 4).Style.Alignment.SetWrapText();
            ws.Range(4, 11, RowLimit, 11).Style.Alignment.SetWrapText();
            ws.Range(4, 12, RowLimit, 12).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Code ?? string.Empty);
            row.Cell(ci++).SetValue(ApplicationType ?? string.Empty);
            row.Cell(ci++).SetValue(Description ?? string.Empty);
            row.Cell(ci++).SetValue(Status ?? string.Empty);
            row.Cell(ci++).SetValue(Criticity ?? string.Empty);
            row.Cell(ci++).SetValue(UserRange ?? string.Empty);
            row.Cell(ci++).SetValue(Category ?? string.Empty);
            row.Cell(ci++).SetValue(Certification ?? string.Empty);
            row.Cell(ci++).SetValue(Sector ?? string.Empty);
            row.Cell(ci++).SetValue(MaintenanceWindow ?? string.Empty);
            row.Cell(ci++).SetValue(Notes ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            Name = row.Cell(ci++).GetString();
            Code = row.Cell(ci++).GetString();
            ApplicationType = row.Cell(ci++).GetString();
            Description = row.Cell(ci++).GetString();
            Status = row.Cell(ci++).GetString();
            Criticity = row.Cell(ci++).GetString();
            UserRange = row.Cell(ci++).GetString();
            Category = row.Cell(ci++).GetString();
            Certification = row.Cell(ci++).GetString();
            Sector = row.Cell(ci++).GetString();
            MaintenanceWindow = row.Cell(ci++).GetString();
            Notes = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Application entity)
        {
            entity.Name = Name;
            entity.Code = Code;
            entity.ApplicationType = GetDomainValueReference(session, DomainValueCategory.ApplicationType, "ApplicationType", ApplicationType);
            entity.AppInfo.Description = Description;
            if (!string.IsNullOrEmpty(Status))
                entity.AppInfo.Status = GetDomainValueReference(session, DomainValueCategory.ApplicationStatus, "Status", Status);
            if (!string.IsNullOrEmpty(Criticity))
                entity.AppInfo.Criticity = GetDomainValueReference(session, DomainValueCategory.ApplicationCriticity, "Criticity", Criticity);
            if (!string.IsNullOrEmpty(UserRange))
                entity.AppInfo.UserRange = GetDomainValueReference(session, DomainValueCategory.ApplicationUserRange, "UserRange", UserRange);
            if (!string.IsNullOrEmpty(Category))
                entity.AppInfo.Category = GetDomainValueReference(session, DomainValueCategory.ApplicationCategory, "Category", Category);
            if (!string.IsNullOrEmpty(Certification))
                entity.AppInfo.Certification = GetDomainValueReference(session, DomainValueCategory.ApplicationCertification, "Certification", Certification);
            if (!string.IsNullOrEmpty(Sector))
            {
                entity.AppInfo.Sector = session.QueryOver<Sector>().Where(s => s.Name == Sector).SingleOrDefault();
                if (entity.AppInfo.Sector == null)
                    throw new ReferenceNotFoundException("Sector", Sector);
            }

            entity.AppInfo.MaintenanceWindow = MaintenanceWindow;
            entity.AppInfo.Notes = Notes;
            return this;
        }

        public override IImporterModel FromEntity(Application entity)
        {
            Name = entity.Name;
            Code = entity.Code;
            ApplicationType = entity.ApplicationType != null ? entity.ApplicationType.Name : string.Empty;
            Description = entity.AppInfo.Description;
            Status = entity.AppInfo.Status != null ? entity.AppInfo.Status.Name : string.Empty;
            Criticity = entity.AppInfo.Criticity != null ? entity.AppInfo.Criticity.Name : string.Empty;
            UserRange = entity.AppInfo.UserRange != null ? entity.AppInfo.UserRange.Name : string.Empty;
            Category = entity.AppInfo.Category != null ? entity.AppInfo.Category.Name : string.Empty;
            Certification = entity.AppInfo.Certification != null ? entity.AppInfo.Certification.Name : string.Empty;
            Sector = entity.AppInfo.Sector != null ? entity.AppInfo.Sector.Name : string.Empty;
            MaintenanceWindow = entity.AppInfo.MaintenanceWindow;
            Notes = entity.AppInfo.Notes;
            return this;
        }
        //// ReSharper restore RedundantAssignment
    }
}
