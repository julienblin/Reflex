// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesModalOptions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class DbInstancesModalOptions
    {
        public string PostUrl { get; set; }

        public bool HideWithServer { get; set; }

        public List<int> HideInstanceIds { get; set; }
    }
}