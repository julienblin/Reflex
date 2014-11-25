// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technologies.cs" company="CGI">
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
    public class Technologies : BaseTechnologiesCalculation
    {
        public override string Name { get { return CoreResources.AppTechnoCalculation; } }

        public override int? Calculate(Application application)
        {
            IEnumerable<Technology> appTechnos = application.TechnologyLinks.Select(s => s.Technology);

            return CalculateTechnologiesScore(appTechnos);
        }
    }
}
