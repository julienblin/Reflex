// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyHierarchy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Models.Pickers
{
    public class TechnologyHierarchy
    {
        public IEnumerable<Technology> RootTechnologies { get; set; }

        public string PostUrl { get; set; }

        public string AddFunctionName { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public IEnumerable<DomainValue> TechnologyTypes { get; set; }
    }
}