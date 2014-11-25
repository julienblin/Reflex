// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleEdit.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.System.Models.Roles
{
    public class RoleEdit
    {
        public RoleEdit()
        {
            AllowedOperations = Enumerable.Empty<string>();
            DeniedOperations = Enumerable.Empty<string>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [Unique(typeof(Role), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(20, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Description")]
        public string Description { get; set; }

        public IEnumerable<string> AvailableFunctions { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AllowedOperations")]
        public IEnumerable<string> AllowedOperations { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "DeniedOperations")]
        public IEnumerable<string> DeniedOperations { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}