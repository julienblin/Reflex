// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorHierarchy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class SectorHierarchy
    {
        public IEnumerable<Sector> RootSectors { get; set; }

        public string TargetUpdateId { get; set; }
    }
}