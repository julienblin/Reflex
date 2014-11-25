// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesList.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Areas.Servers.Models.Technologies
{
    public class TechnologiesList
    {
        public string ServerName { get; set; }

        public int ServerId { get; set; }

        public IEnumerable<Technology> ServerTechnologies { get; set; }

        public IEnumerable<Technology> ApplicationTechnologies { get; set; }

        public IEnumerable<Technology> DbInstanceTechnologies { get; set; }
    }
}