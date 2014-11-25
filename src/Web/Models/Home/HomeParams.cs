// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeParams.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Home
{
    public class HomeParams
    {
        public string LineCriteria { get; set; }

        public IEnumerable<KeyValuePair<string, string>> LineCriteriaList { get; set; }

        public int NumberOfActiveApplications { get; set; }
    }
}