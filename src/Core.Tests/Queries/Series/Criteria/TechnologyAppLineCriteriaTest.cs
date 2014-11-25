// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyAppLineCriteriaTest.cs" company="CGI">
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
    public class TechnologyAppLineCriteriaTest : BaseCriteriaTest
    {
        [Test]
        public void It_should_work_with_line_only_and_all_applications()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => t.Parent = rootTechno);
            var parentTechno2 = Factories.Technology.Save(t => t.Parent = rootTechno);

            var appTechno1 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno2 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno3 = Factories.Technology.Save(t => t.Parent = parentTechno2);

            for (var i = 0; i < 3; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
                app.AddTechnologyLink(appTechno3);
            }

            for (var i = 0; i < 5; i++)
            { 
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 1; i++)
            { 
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno3);
            }

            NHSession.Flush();
            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Technology" }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<Technology>().Name.Should().Be(appTechno1.Name);
            result.Lines[0].GetCount(0).Should().Be(3);
            result.Lines[0].Total.Should().Be(3);

            result.Lines[1].GetCriteria<Technology>().Name.Should().Be(appTechno2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<Technology>().Name.Should().Be(appTechno3.Name);
            result.Lines[2].GetCount(0).Should().Be(4);
            result.Lines[2].Total.Should().Be(4);

            result.GetTotalCount(0).Should().Be(12);
            result.GetGrandTotal().Should().Be(12);

            // Verifying hierarchy
            result.AreLinesHierarchical.Should().BeTrue();

            var rootNodes = result.GetRootNodes();
            rootNodes.Should().HaveCount(1);
            var rootNode = rootNodes.First();
            rootNode.GetCriteria<Technology>().Id.Should().Be(rootTechno.Id);
            rootNode.Total.Should().Be(12);
            rootNode.Children.Should().HaveCount(2);

            var parentNode1 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno1.Id).First();
            parentNode1.Total.Should().Be(8);
            parentNode1.Children.Should().HaveCount(2);

            var node1 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno1.Id).First();
            node1.Total.Should().Be(3);
            node1.Children.Should().HaveCount(0);

            var node2 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno2.Id).First();
            node2.Total.Should().Be(5);
            node2.Children.Should().HaveCount(0);

            var parentNode2 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno2.Id).First();
            parentNode2.Total.Should().Be(4);
            parentNode2.Children.Should().HaveCount(1);

            var node3 = parentNode2.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno3.Id).First();
            node3.Total.Should().Be(4);
            node3.Children.Should().HaveCount(0);
        }

        [Test]
        public void It_should_work_with_line_only_and_only_active_applications()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => t.Parent = rootTechno);
            var parentTechno2 = Factories.Technology.Save(t => t.Parent = rootTechno);

            var appTechno1 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno2 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno3 = Factories.Technology.Save(t => t.Parent = parentTechno2);

            for (var i = 0; i < 3; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
                app.AddTechnologyLink(appTechno3);
            }

            for (var i = 0; i < 5; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 1; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno3);
            }

            NHSession.Flush();
            NHSession.Clear();

            var result = new ApplicationSeries { LineCriteria = "Technology", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(0);
            result.Lines.Should().HaveCount(3);

            result.Lines[0].GetCriteria<Technology>().Name.Should().Be(appTechno1.Name);
            result.Lines[0].GetCount(0).Should().Be(0);
            result.Lines[0].Total.Should().Be(0);

            result.Lines[1].GetCriteria<Technology>().Name.Should().Be(appTechno2.Name);
            result.Lines[1].GetCount(0).Should().Be(5);
            result.Lines[1].Total.Should().Be(5);

            result.Lines[2].GetCriteria<Technology>().Name.Should().Be(appTechno3.Name);
            result.Lines[2].GetCount(0).Should().Be(1);
            result.Lines[2].Total.Should().Be(1);

            result.GetTotalCount(0).Should().Be(6);
            result.GetGrandTotal().Should().Be(6);

            // Verifying hierarchy
            result.AreLinesHierarchical.Should().BeTrue();

            var rootNodes = result.GetRootNodes();
            rootNodes.Should().HaveCount(1);
            var rootNode = rootNodes.First();
            rootNode.GetCriteria<Technology>().Id.Should().Be(rootTechno.Id);
            rootNode.Total.Should().Be(6);
            rootNode.Children.Should().HaveCount(2);

            var parentNode1 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno1.Id).First();
            parentNode1.Total.Should().Be(5);
            parentNode1.Children.Should().HaveCount(2);

            var node1 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno1.Id).First();
            node1.Total.Should().Be(0);
            node1.Children.Should().HaveCount(0);

            var node2 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno2.Id).First();
            node2.Total.Should().Be(5);
            node2.Children.Should().HaveCount(0);

            var parentNode2 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno2.Id).First();
            parentNode2.Total.Should().Be(1);
            parentNode2.Children.Should().HaveCount(1);

            var node3 = parentNode2.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno3.Id).First();
            node3.Total.Should().Be(1);
            node3.Children.Should().HaveCount(0);
        }

        [Test]
        public void It_should_work_with_columns_and_all_applications()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => t.Parent = rootTechno);

            var appTechno1 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno2 = Factories.Technology.Save(t => t.Parent = parentTechno1);

            for (var i = 0; i < 7; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 6; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 5; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = null; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 4; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 3; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 2; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = null; });
                app.AddTechnologyLink(appTechno2);
            }

            CleanOtherStatuses();

            var result = new ApplicationSeries { LineCriteria = "Technology", ColumnCriteria = "ApplicationStatus" }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<Technology>().Name.Should().Be(appTechno1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(6);
            result.Lines[0].GetCount(2).Should().Be(5);
            result.Lines[0].Total.Should().Be(18);

            result.Lines[1].GetCriteria<Technology>().Name.Should().Be(appTechno2.Name);
            result.Lines[1].GetCount(0).Should().Be(4);
            result.Lines[1].GetCount(1).Should().Be(3);
            result.Lines[1].GetCount(2).Should().Be(2);
            result.Lines[1].Total.Should().Be(9);

            result.GetTotalCount(0).Should().Be(11);
            result.GetTotalCount(1).Should().Be(9);
            result.GetTotalCount(2).Should().Be(7);
            result.GetGrandTotal().Should().Be(27);

            // Verifying hierarchy
            result.AreLinesHierarchical.Should().BeTrue();

            var rootNodes = result.GetRootNodes();
            rootNodes.Should().HaveCount(1);
            var rootNode = rootNodes.First();
            rootNode.GetCriteria<Technology>().Id.Should().Be(rootTechno.Id);
            rootNode.Total.Should().Be(27);
            rootNode.Children.Should().HaveCount(1);

            var parentNode1 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno1.Id).First();
            parentNode1.Total.Should().Be(27);
            parentNode1.Children.Should().HaveCount(2);

            var node1 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno1.Id).First();
            node1.Total.Should().Be(18);
            node1.Children.Should().HaveCount(0);

            var node2 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno2.Id).First();
            node2.Total.Should().Be(9);
            node2.Children.Should().HaveCount(0);
        }

        [Test]
        public void It_should_work_with_columns_and_only_active_applications()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => t.Parent = rootTechno);

            var appTechno1 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno2 = Factories.Technology.Save(t => t.Parent = parentTechno1);

            for (var i = 0; i < 7; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 6; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 5; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = null; });
                app.AddTechnologyLink(appTechno1);
            }

            for (var i = 0; i < 4; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = ActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 3; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = NonActiveDomainValue; });
                app.AddTechnologyLink(appTechno2);
            }

            for (var i = 0; i < 2; i++)
            {
                var app = Factories.Application.Save(a => { a.AppInfo.Status = null; });
                app.AddTechnologyLink(appTechno2);
            }

            CleanOtherStatuses();

            var result = new ApplicationSeries { LineCriteria = "Technology", ColumnCriteria = "ApplicationStatus", OnlyActiveApplications = true }.Execute();

            result.Columns.Should().HaveCount(3);
            result.Lines.Should().HaveCount(2);

            result.Lines[0].GetCriteria<Technology>().Name.Should().Be(appTechno1.Name);
            result.Lines[0].GetCount(0).Should().Be(7);
            result.Lines[0].GetCount(1).Should().Be(0);
            result.Lines[0].GetCount(2).Should().Be(0);
            result.Lines[0].Total.Should().Be(7);

            result.Lines[1].GetCriteria<Technology>().Name.Should().Be(appTechno2.Name);
            result.Lines[1].GetCount(0).Should().Be(4);
            result.Lines[1].GetCount(1).Should().Be(0);
            result.Lines[1].GetCount(2).Should().Be(0);
            result.Lines[1].Total.Should().Be(4);

            result.GetTotalCount(0).Should().Be(11);
            result.GetTotalCount(1).Should().Be(0);
            result.GetTotalCount(2).Should().Be(0);
            result.GetGrandTotal().Should().Be(11);

            // Verifying hierarchy
            result.AreLinesHierarchical.Should().BeTrue();

            var rootNodes = result.GetRootNodes();
            rootNodes.Should().HaveCount(1);
            var rootNode = rootNodes.First();
            rootNode.GetCriteria<Technology>().Id.Should().Be(rootTechno.Id);
            rootNode.Total.Should().Be(11);
            rootNode.Children.Should().HaveCount(1);

            var parentNode1 = rootNode.Children.Where(node => node.GetCriteria<Technology>().Id == parentTechno1.Id).First();
            parentNode1.Total.Should().Be(11);
            parentNode1.Children.Should().HaveCount(2);

            var node1 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno1.Id).First();
            node1.Total.Should().Be(7);
            node1.Children.Should().HaveCount(0);

            var node2 = parentNode1.Children.Where(node => node.GetCriteria<Technology>().Id == appTechno2.Id).First();
            node2.Total.Should().Be(4);
            node2.Children.Should().HaveCount(0);
        }
    }
}
