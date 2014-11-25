// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersIndex.cs" company="CGI">
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

namespace CGI.Reflex.Web.Areas.Servers.Models.Servers
{
    public class ServersIndex : AbstractSearchResultModel<Server>
    {
        public ServersIndex()
        {
            Environments = Enumerable.Empty<int>();
            Roles = Enumerable.Empty<int>();
            Statuses = Enumerable.Empty<int>();
            Types = Enumerable.Empty<int>();
        }

        [Display(ResourceType = typeof(WebResources), Name = "QuickNameOrAlias")]
        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string QuickNameOrAlias { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Alias")]
        [StringLength(30, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        public string Alias { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "ShortEvergreenDate")]
        [DataType(DataType.DateTime)]
        public DateTime? EvergreenDate { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Environment")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.Environment)]
        public IEnumerable<int> Environments { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Role")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ServerRole)]
        public IEnumerable<int> Roles { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Status")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ServerStatus)]
        public IEnumerable<int> Statuses { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Type")]
        [UIHint("DomainValues")]
        [DomainValue(DomainValueCategory.ServerType)]
        public IEnumerable<int> Types { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Technology")]
        [UIHint("Technology")]
        public int? Technology { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Landscape")]
        public int? LandscapeId { get; set; }

        public string LandscapeName { get; set; }

        public bool SearchDefined
        {
            get
            { 
                return !string.IsNullOrEmpty(Name)
                       || !string.IsNullOrEmpty(Alias)
                       || ((Environments != null) && Environments.Any())
                       || ((Roles != null) && Roles.Any())
                       || ((Statuses != null) && Statuses.Any())
                       || ((Types != null) && Types.Any())
                       || EvergreenDate.HasValue
                       || Technology.HasValue;
            }
        }
    }
}