// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRangeAppLineCriteriaTest.cs" company="CGI">
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
    public class UserRangeAppLineCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_work_with_line_only_and_all_applications()
        {
            var appUserRange3 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 3; });
            var appUserRange2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 2; });
            var appUserRange1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 1; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange3; });

            var result = new ApplicationSeries { LineCriteria = "ApplicationUserRange" }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appUserRange1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appUserRange2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<DomainValue>().Name.Should().Be(appUserRange3.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(9);
            result.GetGrandTotal().Should().Be(9);
        }

        [Test]
        public void It_should_work_with_line_only_and_only_active_applications()
        {
            var appUserRange3 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 3; });
            var appUserRange2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 2; });
            var appUserRange1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 1; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 8; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange3; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange3; });

            var result = new ApplicationSeries { LineCriteria = "ApplicationUserRange", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appUserRange1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appUserRange2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<DomainValue>().Name.Should().Be(appUserRange3.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(9);
            result.GetGrandTotal().Should().Be(9);
        }

        [Test]
        public void It_should_work_with_columns_and_all_applications()
        {
            var appUserRange2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 2; });
            var appUserRange1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.UserRange = appUserRange2; });

            CleanOtherStatuses();

            var result = new ApplicationSeries { LineCriteria = "ApplicationUserRange", ColumnCriteria = "ApplicationStatus" }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appUserRange1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(6);
            result.Lines[0].GetCount(2).Should().Be(5);
            result.Lines[0].Total.Should().Be(18);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appUserRange2.Name);
            result.Lines[1].GetCount(0).Should().Be(4);
            result.Lines[1].GetCount(1).Should().Be(3);
            result.Lines[1].GetCount(2).Should().Be(2);
            result.Lines[1].Total.Should().Be(9);

            result.GetTotalCount(0).Should().Be(11);
            result.GetTotalCount(1).Should().Be(9);
            result.GetTotalCount(2).Should().Be(7);
            result.GetGrandTotal().Should().Be(27);
        }

        [Test]
        public void It_should_work_with_columns_and_only_active_applications()
        {
            var appUserRange2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 2; });
            var appUserRange1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationUserRange; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.UserRange = appUserRange1; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.UserRange = appUserRange2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.UserRange = appUserRange2; });

            CleanOtherStatuses();

            var result = new ApplicationSeries { LineCriteria = "ApplicationUserRange", ColumnCriteria = "ApplicationStatus", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appUserRange1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(0);
            result.Lines[0].GetCount(2).Should().Be(0);
            result.Lines[0].Total.Should().Be(7);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appUserRange2.Name);
            result.Lines[1].GetCount(0).Should().Be(4);
            result.Lines[1].GetCount(1).Should().Be(0);
            result.Lines[1].GetCount(2).Should().Be(0);
            result.Lines[1].Total.Should().Be(4);

            result.GetTotalCount(0).Should().Be(11);
            result.GetTotalCount(1).Should().Be(0);
            result.GetTotalCount(2).Should().Be(0);
            result.GetGrandTotal().Should().Be(11);
        }
    }
}
