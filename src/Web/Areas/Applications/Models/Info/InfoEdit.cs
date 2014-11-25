// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Infra;

namespace CGI.Reflex.Web.Areas.Applications.Models.Info
{
    public class InfoEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [Unique(typeof(Application), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Code")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Code { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationType)]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public int? TypeId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationStatus)]
        public int? StatusId { get; set; }
        
        [Display(ResourceType = typeof(CoreResources), Name = "Criticity")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationCriticity)]
        public int? CriticityId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationDescription")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "UserRange")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationUserRange)]
        public int? UserRange { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationCategory")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationCategory)]
        public int? Category { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "CategoryDescription")]
        public string CategoryDescription { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Certification")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationCertification)]
        public int? Certification { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "MaintenanceWindow")]
        public string MaintenanceWindow { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        [UIHint("Sector")]
        public int? SectorId { get; set; }
    }
}