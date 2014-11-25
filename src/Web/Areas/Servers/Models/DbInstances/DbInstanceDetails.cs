// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceDetails.cs" company="CGI">
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
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Servers.Models.DbInstances
{
    public class DbInstanceDetails : AbstractSearchResultModel<DbInstance>
    {
        public string ServerName { get; set; }

        public int ServerId { get; set; }

        public string TechnologyName { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(CoreResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string DbInstanceName { get; set; }

        public bool SearchDefined
        {
            get { return !string.IsNullOrEmpty(DbInstanceName); }
        }
    }
}
