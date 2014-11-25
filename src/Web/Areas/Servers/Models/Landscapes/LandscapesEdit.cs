// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesEdit.cs" company="CGI">
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
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Servers.Models.Landscapes
{
    public class LandscapesEdit 
    {
       [HiddenInput(DisplayValue = false)]
       public int Id { get; set; }

       [HiddenInput(DisplayValue = false)]
       public int ConcurrencyVersion { get; set; }

       [Unique(typeof(Landscape), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
       [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
       [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
       [Display(ResourceType = typeof(CoreResources), Name = "Name")]
       public string Name { get; set; }

       [Display(ResourceType = typeof(CoreResources), Name = "Description")]
       [StringLength(100, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
       public string Description { get; set; }

       [Display(ResourceType = typeof(CoreResources), Name = "Server")]
       public int? ServerId { get; set; }

       public LandscapesServersDisplay LandscapeServers { get; set; }

       public string FormAction
       {
           get { return Id == 0 ? "Create" : "Edit"; }
       }
    }
}
