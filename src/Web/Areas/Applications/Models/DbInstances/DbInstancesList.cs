// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesList.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.DbInstances
{
    public class DbInstancesList
    {
        public string AppName { get; set; }

        public int AppId { get; set; }

        public IList<DbInstance> DbInstances { get; set; }
    }
}