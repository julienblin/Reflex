// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.System.Models.DbConsole
{
    public enum DbQueryAction
    {
        Execute,
        Update
    }

    public class DbQuery
    {
        public string Query { get; set; }

        public DbQueryAction DbQueryAction { get; set; }

        public IList Result { get; set; }
    }
}