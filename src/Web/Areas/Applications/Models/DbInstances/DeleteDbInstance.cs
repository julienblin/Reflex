// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteDbInstance.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Applications.Models.DbInstances
{
    public class DeleteDbInstance
    {
        public DbInstance DbInstances { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
    }
}