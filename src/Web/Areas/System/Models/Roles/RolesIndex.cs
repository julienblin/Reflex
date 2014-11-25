// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RolesIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.System.Models.Roles
{
    public class RolesIndex : AbstractSearchResultModel<Role>
    {
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "AllowedOperation")]
        public string AllowedOperation { get; set; }

        public IEnumerable<string> AllowedOperations { get; set; }

        public bool SearchDefined
        {
            get
            { 
                return !string.IsNullOrEmpty(Name)
                    || !string.IsNullOrEmpty(AllowedOperation);
            }
        }
    }
}