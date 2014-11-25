// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyDetails.cs" company="CGI">
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

namespace CGI.Reflex.Web.Models.Pickers
{
    public class TechnologyDetails
    {
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }

        public string ParentFullName { get; set; }

        public bool HasChildren { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "EndOfSupport")]
        public DateTime? EndOfSupport { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Contact")]
        [UIHint("Contact")]
        public int? ContactId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "TechnologyType")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.TechnologyType)]
        public virtual int? TechnologyTypeId { get; set; }
    }
}