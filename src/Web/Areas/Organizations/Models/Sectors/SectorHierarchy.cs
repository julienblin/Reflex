// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorHierarchy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Organizations.Models.Sectors
{
    public class SectorHierarchy
    {
        public IEnumerable<Sector> RootSectors { get; set; }

        [Display(ResourceType = typeof(CoreResources), Name = "Name")]
        public string Name { get; set; }
    }
}