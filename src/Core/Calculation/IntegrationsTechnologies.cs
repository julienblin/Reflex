// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsTechnologies.cs" company="CGI">
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
    public class IntegrationsTechnologies : BaseTechnologiesCalculation
    {
        public override string Name
        {
            get { return CoreResources.AppIntegrationTechnoCalculation; }
        }

        public override int? Calculate(Application application)
        {
            var technos = application.GetIntegrations().SelectMany(i => i.TechnologyLinks).Select(i => i.Technology).Distinct();
            return CalculateTechnologiesScore(technos);
        }
    }
}
