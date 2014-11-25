// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersModalOptions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class ServersModalOptions
    {
        public ServersModalOptions()
        {
            SelectionMode = SelectionMode.Multiple;
        }

        public string PostUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public bool HideWithLandscape { get; set; }

        public int? CurLandscapeId { get; set; }

        public string HideServersFromTarget { get; set; }
    }
}