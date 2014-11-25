// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersTechnologies.cs" company="CGI">
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
    public class ServersTechnologies : BaseTechnologiesCalculation
    {
        public override string Name { get { return CoreResources.AppServerTechnoCalculation; } }

        public override int? Calculate(Application application)
        {
            var technos = application.ServerLinks.SelectMany(a => a.Server.TechnologyLinks).Select(tl => tl.Technology).Distinct();

            return CalculateTechnologiesScore(technos);
        }
    }
}
