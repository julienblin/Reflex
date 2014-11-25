// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersDetails.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Applications.Models.Servers
{
    public class ServersDetails
    {
        public string AppName { get; set; }

        public int AppId { get; set; }

        public LandscapesServersDisplay Results { get; set; }
    }
}