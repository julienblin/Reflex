// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTypeAppColumnCriteriaTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Core.Queries.Series;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries.Series.Criteria
{
    public class ApplicationTypeAppColumnCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_count_columns()
        {
            var appType2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationType; dv.DisplayOrder = 2; });
            var appType1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationType; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.ApplicationType = appType1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.ApplicationType = appType1; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.ApplicationType = appType2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.ApplicationType = appType2; });

            var allTypes = new DomainValueQuery { Category = DomainValueCategory.ApplicationType }.List();
            foreach (var t in allTypes.Where(t => t != appType1 && t != appType2))
                NHSession.Delete(t);

            NHSession.Flush();

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus", ColumnCriteria = "ApplicationType" }.Execute();

            result.Columns.Should().HaveCount(2);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(4);
            result.Lines[0].Total.Should().Be(11);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(NonActiveDomainValue.Name);
            result.Lines[1].GetCount(0).Should().Be(6);
            result.Lines[1].GetCount(1).Should().Be(3);
            result.Lines[1].Total.Should().Be(9);

            result.GetTotalCount(0).Should().Be(13);
            result.GetTotalCount(1).Should().Be(7);
            result.GetGrandTotal().Should().Be(20);
        }
    }
}
