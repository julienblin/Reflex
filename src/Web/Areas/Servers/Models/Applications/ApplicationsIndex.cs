// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Servers.Models.Applications
{
    public class ApplicationsIndex : AbstractSearchResultModel<Application>
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Code")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Code { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationType")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationType)]
        public int? ApplicationType { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationStatus)]
        public int? Status { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Criticity")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationCriticity)]
        public int? Criticity { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        [UIHint("Contact")]
        public int? Contact { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Sector")]
        [UIHint("Sector")]
        public int? Sector { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        [UIHint("Technology")]
        public int? Technology { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Server")]
        [UIHint("Server")]
        public int? Server { get; set; }

        public int ServerId { get; set; }
    }
}