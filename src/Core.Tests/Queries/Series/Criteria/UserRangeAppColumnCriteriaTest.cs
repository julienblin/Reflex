﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRangeAppColumnCriteriaTest.cs" company="CGI">
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
    public class UserRangeAppColumnCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_count_columns()
        {
            var appUserRange2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 2; });
            var appUserRange1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = null; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = null; });

            var allrange = new DomainValueQuery { Category = DomainValueCategory.ApplicationUserRange }.List();
            foreach (var range in allrange.Where(range => range != appUserRange1 && range != appUserRange2))
                NHSession.Delete(range);

            NHSession.Flush();

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus", ColumnCriteria = "ApplicationUserRange" }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(4);
            result.Lines[0].GetCount(2).Should().Be(2);
            result.Lines[0].Total.Should().Be(13);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(NonActiveDomainValue.Name);
            result.Lines[1].GetCount(0).Should().Be(6);
            result.Lines[1].GetCount(1).Should().Be(3);
            result.Lines[1].GetCount(2).Should().Be(5);
            result.Lines[1].Total.Should().Be(14);

            result.GetTotalCount(0).Should().Be(13);
            result.GetTotalCount(1).Should().Be(7);
            result.GetTotalCount(2).Should().Be(7);
            result.GetGrandTotal().Should().Be(27);
        }
    }
}
