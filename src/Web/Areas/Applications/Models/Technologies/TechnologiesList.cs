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

namespace CGI.Reflex.Web.Areas.Applications.Models.Technologies
{
    public class TechnologiesList
    {
        public string AppName { get; set; }

        public int AppId { get; set; }

        public IEnumerable<Technology> ApplicationTechnologies { get; set; }

        public IEnumerable<Technology> DatabaseTechnologies { get; set; }

        public IEnumerable<Technology> IntegrationTechnologies { get; set; }

        public IEnumerable<Technology> ServerTechnologies { get; set; }

        public IEnumerable<Technology> DbInstanceTechnologies { get; set; }
    }
}