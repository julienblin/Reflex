// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersList.cs" company="CGI">
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
    public class ServersList
    {
        public IEnumerable<Server> Servers { get; set; }

        public string PostUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public bool HideWithLandscape { get; set; }

        public string HideServersFromTarget { get; set; }
    }
}