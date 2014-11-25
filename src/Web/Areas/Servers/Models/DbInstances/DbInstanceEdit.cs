// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceEdit.cs" company="CGI">
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

namespace CGI.Reflex.Web.Areas.Servers.Models.DbInstances
{
    public class DbInstanceEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        [UIHint("Technology")]
        public int? TechnologyId { get; set; }

        public string FormAction
        {
            get { return Id == 0 ? "Create" : "Edit"; }
        }
    }
}
