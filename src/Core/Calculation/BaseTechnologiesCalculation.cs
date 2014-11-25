// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTechnologiesCalculation.cs" company="CGI">
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
    public abstract class BaseTechnologiesCalculation : ICalculation
    {
        protected BaseTechnologiesCalculation()
        {
            Date = DateTime.Now;
        }

        public QuestionType Type
        {
            get { return QuestionType.TechnologyValue; }
        }

        public int MaxValue
        {
            get { return 100; }
        }

        public abstract string Name { get; }

        public DateTime Date { get; set; }

        public abstract int? Calculate(Application application);

        protected int? CalculateTechnologiesScore(IEnumerable<Technology> technologies)
        {
            var technoCount = technologies.GroupBy(t => t.GetEndOfSupportStatus(Date)).ToDictionary(f => f.Key, g => g.Count());

            int upToDateCount = technoCount.ContainsKey(Technology.EndOfSupportStatus.UpToDate) ? technoCount[Technology.EndOfSupportStatus.UpToDate] : 0;
            int almostOutdated = technoCount.ContainsKey(Technology.EndOfSupportStatus.AlmostOutdated) ? technoCount[Technology.EndOfSupportStatus.AlmostOutdated] : 0;
            int outDated = technoCount.ContainsKey(Technology.EndOfSupportStatus.Outdated) ? technoCount[Technology.EndOfSupportStatus.Outdated] : 0;

            // Score (Plus d'un an : 100% / Moins d'un an : 50% / Passé date : 0%)
            var scoreTotal = (upToDateCount * 100) + (almostOutdated * 50) + (outDated * 0);

            // upToDateCount *= 1; // Plus d'un an (poid = 1)
            almostOutdated *= 2; // Moins d'un an (poid = 2)
            outDated *= 3; // Désuette (poids = 3)

            var totalTechno = upToDateCount + almostOutdated + outDated;

            if (totalTechno == 0)
                return null;
            return scoreTotal / totalTechno;
        }
    }
}
