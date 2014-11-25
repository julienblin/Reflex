// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusAppLineCriteriaTest.cs" company="CGI">
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
    public class StatusAppLineCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_work_with_line_only_and_all_applications()
        {
            var appStatus2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = -1; });
            var appStatus1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = -2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = appStatus1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = appStatus2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus" }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appStatus1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appStatus2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(9);
            result.GetGrandTotal().Should().Be(9);
        }

        [Test]
        public void It_should_work_with_line_only_and_only_active_applications()
        {
            var appStatus2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = -1; });
            var appStatus1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationStatus; dv.DisplayOrder = -2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = appStatus1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = appStatus2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(4);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(appStatus1.Name);
            result.Lines[0].GetCount(0).Should().Be(0);
            result.Lines[0].Total.Should().Be(0);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(appStatus2.Name);
            result.Lines[1].GetCount(0).Should().Be(0);
            result.Lines[1].Total.Should().Be(0);

            result.Lines[2].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.Lines[3].GetCriteria<DomainValue>().Name.Should().Be(NonActiveDomainValue.Name);
            result.Lines[3].GetCount(0).Should().Be(0);
            result.Lines[3].Total.Should().Be(0);

            result.GetTotalCount(0).Should().Be(1);
            result.GetGrandTotal().Should().Be(1);
        }

        [Test]
        public void It_should_work_with_columns_and_all_applications()
        {
            var appCrit2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationCriticity; dv.DisplayOrder = 2; });
            var appCrit1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationCriticity; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Criticity = appCrit1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Criticity = appCrit1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Criticity = null; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Criticity = appCrit2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Criticity = appCrit2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Criticity = null; });

            CleanOtherStatuses();

            var allCrit = new DomainValueQuery { Category = DomainValueCategory.ApplicationCriticity }.List();
            foreach (var crit in allCrit.Where(crit => crit != appCrit1 && crit != appCrit2))
                NHSession.Delete(crit);

            NHSession.Flush();

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus", ColumnCriteria = "ApplicationCriticity" }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(4);
            result.Lines[0].GetCount(2).Should().Be(0);
            result.Lines[0].Total.Should().Be(11);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(NonActiveDomainValue.Name);
            result.Lines[1].GetCount(0).Should().Be(6);
            result.Lines[1].GetCount(1).Should().Be(3);
            result.Lines[1].GetCount(2).Should().Be(0);
            result.Lines[1].Total.Should().Be(9);

            result.GetTotalCount(0).Should().Be(13);
            result.GetTotalCount(1).Should().Be(7);
            result.GetTotalCount(2).Should().Be(0);
            result.GetGrandTotal().Should().Be(20);
        }

        [Test]
        public void It_should_work_with_columns_and_only_active_applications()
        {
            var appCrit2 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationCriticity; dv.DisplayOrder = 2; });
            var appCrit1 = Factories.DomainValue.Save(dv => { dv.Category = DomainValueCategory.ApplicationCriticity; dv.DisplayOrder = 1; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Criticity = appCrit1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Criticity = appCrit1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Criticity = null; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Criticity = appCrit2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Criticity = appCrit2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Criticity = null; });

            CleanOtherStatuses();

            var allCrit = new DomainValueQuery { Category = DomainValueCategory.ApplicationCriticity }.List();
            foreach (var crit in allCrit.Where(crit => crit != appCrit1 && crit != appCrit2))
                NHSession.Delete(crit);

            NHSession.Flush();

            var result = new ApplicationSeries { LineCriteria = "ApplicationStatus", ColumnCriteria = "ApplicationCriticity", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<DomainValue>().Name.Should().Be(ActiveDomainValue.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(4);
            result.Lines[0].GetCount(2).Should().Be(0);
            result.Lines[0].Total.Should().Be(11);

            result.Lines[1].GetCriteria<DomainValue>().Name.Should().Be(NonActiveDomainValue.Name);
            result.Lines[1].GetCount(0).Should().Be(0);
            result.Lines[1].GetCount(1).Should().Be(0);
            result.Lines[1].GetCount(2).Should().Be(0);
            result.Lines[1].Total.Should().Be(0);

            result.GetTotalCount(0).Should().Be(7);
            result.GetTotalCount(1).Should().Be(4);
            result.GetTotalCount(2).Should().Be(0);
            result.GetGrandTotal().Should().Be(11);
        }
    }
}
