// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppHeader.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Infra;

namespace CGI.Reflex.Web.Areas.Applications.Models
{
    public class AppHeader
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

        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [UIHint("DomainValue")]
        [DomainValue(DomainValueCategory.ApplicationType)]
        [Display(ResourceType = typeof(CoreResources), Name = "ApplicationType")]
        public virtual int? ApplicationTypeId { get; set; }
    }
}