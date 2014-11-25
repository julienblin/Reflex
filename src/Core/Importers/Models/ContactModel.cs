// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactModel.cs" company="CGI">
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
    internal class ContactModel : BaseImporterModel<Contact>, IValidatableObject
    {
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Company { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Email { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string PhoneNumber { get; set; }

        public string SectorName { get; set; }

        public string ContactType { get; set; }

        public virtual string Notes { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Contact");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("FirstName").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("LastName").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Company").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Email");
            ws.Cell(3, ci++).SetValue("PhoneNumber");
            ws.Cell(3, ci++).SetValue("SectorName");
            ws.Cell(3, ci++).SetValue("ContactType");
            ws.Cell(3, ci++).SetValue("Notes");

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 50;
            ws.Column(ci++).Width = 17;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDataValidation(ws, 6, "SectorsNamesDynamic");
            SetColumnDataValidation(ws, 7, "ContactType");
            ws.Range(4, 8, 10000, 8).Style.Alignment.SetWrapText();
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(FirstName ?? string.Empty);
            row.Cell(ci++).SetValue(LastName ?? string.Empty);
            row.Cell(ci++).SetValue(Company ?? string.Empty);
            row.Cell(ci++).SetValue(Email ?? string.Empty);
            row.Cell(ci++).SetValue(PhoneNumber ?? string.Empty);
            row.Cell(ci++).SetValue(SectorName ?? string.Empty);
            row.Cell(ci++).SetValue(ContactType ?? string.Empty);
            row.Cell(ci++).SetValue(Notes ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            FirstName = row.Cell(ci++).GetString();
            LastName = row.Cell(ci++).GetString();
            Company = row.Cell(ci++).GetString();
            Email = row.Cell(ci++).GetString();
            PhoneNumber = row.Cell(ci++).GetString();
            SectorName = row.Cell(ci++).GetString();
            ContactType = row.Cell(ci++).GetString();
            Notes = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, Contact entity)
        {
            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.Company = Company;
            entity.Email = Email;
            entity.PhoneNumber = PhoneNumber;
            if (!string.IsNullOrEmpty(SectorName))
            {
                entity.Sector = session.QueryOver<Sector>().Where(s => s.Name == SectorName).SingleOrDefault();
                if (entity.Sector == null)
                    throw new ReferenceNotFoundException("SectorName", SectorName);
            }

            entity.Type = GetDomainValueReference(session, DomainValueCategory.ContactType, "ContactType", ContactType);
            entity.Notes = Notes;
            return this;
        }

        public override IImporterModel FromEntity(Contact entity)
        {
            FirstName = entity.FirstName ?? string.Empty;
            LastName = entity.LastName ?? string.Empty;
            Company = entity.Company ?? string.Empty;
            Email = entity.Email ?? string.Empty;
            PhoneNumber = entity.PhoneNumber ?? string.Empty;
            SectorName = entity.Sector == null ? string.Empty : entity.Sector.Name;
            ContactType = entity.Type == null ? string.Empty : entity.Type.Name;
            Notes = entity.Notes ?? string.Empty;
            return this;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(Company))
            {
                result.Add(new ValidationResult("FirstName, LastName or Company must have a value."));
            }

            return result;
        }
    }
    //// ReSharper restore RedundantAssignment
}
