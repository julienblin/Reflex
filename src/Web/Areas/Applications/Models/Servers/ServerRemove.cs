// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerRemove.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.Servers
{
    public class ServerRemove
    {
        public ServerRemove()
        {
            DbInstancesToRemove = new List<DbInstance>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string ServerName { get; set; }

        public string ApplicationName { get; set; }

        public int ApplicationId { get; set; }

        public IList<DbInstance> DbInstancesToRemove { get; set; }
    }
}