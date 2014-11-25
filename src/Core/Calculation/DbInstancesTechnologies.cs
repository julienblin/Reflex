// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesTechnologies.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Calculation
{
    public class DbInstancesTechnologies : BaseTechnologiesCalculation
    {
        public override string Name { get { return CoreResources.AppDbInstancesCalculation; } }

        public override int? Calculate(Application application)
        {
            IEnumerable<Technology> technos = application.DbInstanceLinks.Select(i => i.DbInstances).SelectMany(i => i.TechnologyLinks).Select(t => t.Technology).Distinct();

            return CalculateTechnologiesScore(technos);
        }
    }
}
