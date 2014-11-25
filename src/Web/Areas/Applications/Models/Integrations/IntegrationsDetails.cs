// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsDetails.cs" company="CGI">
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

namespace CGI.Reflex.Web.Areas.Applications.Models.Integrations
{
    public class IntegrationsDetails : AbstractSearchResultModel<Integration>
    {
        public string AppName { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AppSource")]
        [UIHint("Application")]
        public int? AppSourceId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AppDest")]
        [UIHint("Application")]
        public int? AppDestId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Nature")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.IntegrationNature)]
        public int? NatureId { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string IntegrationName { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        [UIHint("Technology")]
        public int? Technology { get; set; }

        public bool SearchDefined
        {
            get { return !string.IsNullOrEmpty(IntegrationName) || NatureId.HasValue || AppDestId.HasValue || AppSourceId.HasValue || Technology.HasValue; }
        }
    }
}