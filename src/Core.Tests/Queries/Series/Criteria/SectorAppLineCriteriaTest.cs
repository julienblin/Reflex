// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorAppLineCriteriaTest.cs" company="CGI">
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
    public class SectorAppLineCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_work_with_line_only_and_all_applications()
        {
            var rootSector = Factories.Sector.Save();

            var parentSector1 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "aaa"; });
            var parentSector2 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "bbb"; });

            var sector1 = Factories.Sector.Save(s => { s.Parent = parentSector1; s.Name = "ccc"; });
            var sector2 = Factories.Sector.Save(s => { s.Parent = parentSector1; s.Name = "ddd"; });
            var sector3 = Factories.Sector.Save(s => { s.Parent = parentSector2; s.Name = "eee"; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector3; });

            NHSession.Flush();
            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Sector" }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<Sector>().Name.Should().Be(sector1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<Sector>().Name.Should().Be(sector2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<Sector>().Name.Should().Be(sector3.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(9);
            result.GetGrandTotal().Should().Be(9);

            // Verifying hierarchy
            result.AreLinesHierarchical.Should().BeTrue();

            var rootNodes = result.GetRootNodes();
            rootNodes.Should().HaveCount(1);
            var rootNode = rootNodes.First();
            rootNode.GetCriteria<Sector>().Id.Should().Be(rootSector.Id);
            rootNode.Total.Should().Be(9);
            rootNode.Children.Should().HaveCount(2);

            var parentNode1 = rootNode.Children.Where(node => node.GetCriteria<Sector>().Id == parentSector1.Id).First();
            parentNode1.Total.Should().Be(8);
            parentNode1.Children.Should().HaveCount(2);

            var node1 = parentNode1.Children.Where(node => node.GetCriteria<Sector>().Id == sector1.Id).First();
            node1.Total.Should().Be(3);
            node1.Children.Should().HaveCount(0);

            var node2 = parentNode1.Children.Where(node => node.GetCriteria<Sector>().Id == sector2.Id).First();
            node2.Total.Should().Be(5);
            node2.Children.Should().HaveCount(0);

            var parentNode2 = rootNode.Children.Where(node => node.GetCriteria<Sector>().Id == parentSector2.Id).First();
            parentNode2.Total.Should().Be(1);
            parentNode2.Children.Should().HaveCount(1);

            var node3 = parentNode2.Children.Where(node => node.GetCriteria<Sector>().Id == sector3.Id).First();
            node3.Total.Should().Be(1);
            node3.Children.Should().HaveCount(0);
        }

        [Test]
        public void It_should_work_with_line_only_and_only_active_applications()
        {
            var rootSector = Factories.Sector.Save();

            var parentSector1 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "aaa"; });
            var parentSector2 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "bbb"; });

            var sector1 = Factories.Sector.Save(s => { s.Parent = parentSector1; s.Name = "ccc"; });
            var sector2 = Factories.Sector.Save(s => { s.Parent = parentSector1; s.Name = "ddd"; });
            var sector3 = Factories.Sector.Save(s => { s.Parent = parentSector2; s.Name = "eee"; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 8; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 1; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector3; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector3; });

            NHSession.Flush();
            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Sector", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<Sector>().Name.Should().Be(sector1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<Sector>().Name.Should().Be(sector2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<Sector>().Name.Should().Be(sector3.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(9);
            result.GetGrandTotal().Should().Be(9);
        }

        [Test]
        public void It_should_work_with_columns_and_all_applications()
        {
            var rootSector = Factories.Sector.Save();

            var sector1 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "aaa"; });
            var sector2 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "bbb"; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Sector = sector2; });

            CleanOtherStatuses();

            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Sector", ColumnCriteria = "ApplicationStatus" }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<Sector>().Name.Should().Be(sector1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(6);
            result.Lines[0].GetCount(2).Should().Be(5);
            result.Lines[0].Total.Should().Be(18);

            result.Lines[1].GetCriteria<Sector>().Name.Should().Be(sector2.Name);
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
            var rootSector = Factories.Sector.Save();

            var sector1 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "aaa"; });
            var sector2 = Factories.Sector.Save(s => { s.Parent = rootSector; s.Name = "bbb"; });

            for (var i = 0; i < 7; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 6; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 5; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Sector = sector1; });

            for (var i = 0; i < 4; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 3; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; a.AppInfo.Sector = sector2; });

            for (var i = 0; i < 2; i++)
                Factories.Application.Save(a => { a.AppInfo.Status = null; a.AppInfo.Sector = sector2; });

            CleanOtherStatuses();
            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Sector", ColumnCriteria = "ApplicationStatus", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<Sector>().Name.Should().Be(sector1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(0);
            result.Lines[0].GetCount(2).Should().Be(0);
            result.Lines[0].Total.Should().Be(7);

            result.Lines[1].GetCriteria<Sector>().Name.Should().Be(sector2.Name);
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
