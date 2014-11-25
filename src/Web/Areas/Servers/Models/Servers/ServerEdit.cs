// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEdit.cs" company="CGI">
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

namespace CGI.Reflex.Web.Areas.Servers.Models.Servers
{
    public class ServerEdit 
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ConcurrencyVersion { get; set; }

        [Unique(typeof(Server), ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Unique")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Alias")]
        public string Alias { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(CoreResources), Name = "Comments")]
        public string Comments { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "EvergreenDate")]
        [DataType(DataType.DateTime)]
        public DateTime? EvergreenDate { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Environment")]
        [DomainValue(DomainValueCategory.Environment)]
        [UIHint("DomainValue")]
        [Required(ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "Required")]
        public int? EnvironmentId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        [DomainValue(DomainValueCategory.ServerRole)]
        [UIHint("DomainValue")]
        public int? RoleId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [DomainValue(DomainValueCategory.ServerStatus)]
        [UIHint("DomainValue")]
        public int? StatusId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        [DomainValue(DomainValueCategory.ServerType)]
        [UIHint("DomainValue")]
        public int? TypeId { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Landscape")]
        public int? LandscapeId { get; set; }

        public string LandscapeName { get; set; }
    }
}
