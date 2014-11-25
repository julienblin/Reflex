// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyHierarchy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Technologies.Models
{
    public class TechnologyHierarchy
    {
        public IEnumerable<Technology> RootTechnologies { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        [DomainValue(DomainValueCategory.TechnologyType)]
        [UIHint("DomainValue")]
        public int? TechnologyType { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "DateFrom")]
        public DateTime? DateFrom { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "DateTo")]
        public DateTime? DateTo { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        [UIHint("Contact")]
        public int? ContactId { get; set; }
    }
}