// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCriteriaTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries.Series.Criteria
{
    public abstract class BaseCriteriaTest : BaseDbTest
    {
        protected DomainValue ActiveDomainValue { get; set; }

        protected DomainValue NonActiveDomainValue { get; set; }

        [SetUp]
        public void SetUpActiveDV()
        {
            ActiveDomainValue = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = 0; });
            NonActiveDomainValue = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = 1; });

            var config = ReflexConfiguration.GetCurrent();
            NHSession.SetReadOnly(config, false);
            config.SetActiveAppStatusDVIds(new[] { ActiveDomainValue.Id });
        }

        protected void CleanOtherStatuses()
        {
            var allStatuses = new DomainValueQuery { Category = DomainValueCategory.ApplicationStatus }.List();
            foreach (var stat in allStatuses.Where(stat => stat != ActiveDomainValue && stat != NonActiveDomainValue))
                NHSession.Delete(stat);

            NHSession.Flush();
        }
    }
}
