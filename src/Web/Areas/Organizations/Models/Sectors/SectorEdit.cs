// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorEdit.cs" company="CGI">
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
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models.Sectors
{
    public class SectorEdit
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ParentId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [StringLength(50, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }
    }
}