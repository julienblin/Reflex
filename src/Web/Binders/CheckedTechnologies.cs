// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedTechnologies.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace CGI.Reflex.Web.Binders
{
    public class CheckedTechnologies
    {
        public CheckedTechnologies()
        {
            TechnologyIds = Enumerable.Empty<int>();
        }

        public IEnumerable<int> TechnologyIds { get; set; }
    }
}