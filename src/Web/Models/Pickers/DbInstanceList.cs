// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceList.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class DbInstanceList
    {
        public IEnumerable<DbInstance> DbInstances { get; set; }

        public IEnumerable<Server> ServersWithInstances { get; set; }

        public string PostUrl { get; set; }

        public bool HideWithServer { get; set; }
    }
}
