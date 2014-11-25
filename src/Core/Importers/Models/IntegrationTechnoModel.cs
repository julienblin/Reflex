// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTechnoModel.cs" company="CGI">
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
    internal class IntegrationTechnoModel : BaseImporterModel<IntegrationTechnoLink>
    {
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string AppSource { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string AppDest { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Technology { get; set; }

        public override void Prepare(IXLWorksheet ws, string title = null)
        {
            SetTitle(ws, title ?? "Integrations-Technologies");

            var ci = 1;
            ws.Cell(3, ci++).SetValue("AppSource").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("AppDest").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Name").Style.Font.SetBold();
            ws.Cell(3, ci++).SetValue("Technology").Style.Font.SetBold();

            SetAutoFilter(ws, ci - 1);

            ci = 1;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 25;
            ws.Column(ci++).Width = 100;

            SetColumnDataValidation(ws, 1, "Applications");
            SetColumnDataValidation(ws, 2, "Applications");
            SetColumnDataValidation(ws, 4, "Technologies");
        }

        public override IImporterModel ToRow(IXLRow row)
        {
            var ci = 1;
            row.Cell(ci++).SetValue(AppSource ?? string.Empty);
            row.Cell(ci++).SetValue(AppDest ?? string.Empty);
            row.Cell(ci++).SetValue(Name ?? string.Empty);
            row.Cell(ci++).SetValue(Technology ?? string.Empty);
            return this;
        }

        public override IImporterModel FromRow(IXLRow row)
        {
            var ci = 1;
            AppSource = row.Cell(ci++).GetString();
            AppDest = row.Cell(ci++).GetString();
            Name = row.Cell(ci++).GetString();
            Technology = row.Cell(ci++).GetString();
            return this;
        }

        public override IImporterModel ToEntity(ISession session, IntegrationTechnoLink entity)
        {
            var integrationQuery = session.QueryOver<Integration>().Where(i => i.Name == Name);
            integrationQuery.JoinQueryOver(i => i.AppSource).Where(a => a.Name == AppSource);
            integrationQuery.JoinQueryOver(i => i.AppDest).Where(a => a.Name == AppDest);

            entity.Integration = integrationQuery.SingleOrDefault();
            if (entity.Integration == null)
                throw new ReferenceNotFoundException("Integration", string.Format("{0} => {1} ({2})", AppSource, AppDest, Name));

            entity.Technology = new TechnologyByEscapedFullNameQuery { EscapedFullName = Technology }.Execute(session);
            if (entity.Technology == null)
                throw new ReferenceNotFoundException("Technology", Technology);
            return this;
        }

        public override IImporterModel FromEntity(IntegrationTechnoLink entity)
        {
            if (entity.Integration == null)
            {
                AppSource = string.Empty;
                AppDest = string.Empty;
                Name = string.Empty;
                Technology = string.Empty;
            }
            else
            {
                AppSource = entity.Integration.AppSource != null ? entity.Integration.AppSource.Name : string.Empty;
                AppDest = entity.Integration.AppDest != null ? entity.Integration.AppDest.Name : string.Empty;
                Name = entity.Integration.Name ?? string.Empty;
                Technology = entity.Technology != null ? entity.Technology.GetEscapedFullName() : string.Empty;
            }

            return this;
        }
    }
    //// ReSharper restore RedundantAssignment
}
