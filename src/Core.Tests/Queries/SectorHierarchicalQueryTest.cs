// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorHierarchicalQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NHibernate;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class SectorHierarchicalQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_return_hierarchy_of_sectors_order_by_name()
        {
            var rootSector3 = Factories.Sector.Save(s => s.Name = "ccc");
            var rootSector2 = Factories.Sector.Save(s => s.Name = "bbb");
            var rootSector1 = Factories.Sector.Save(s => s.Name = "aaa");

            var sector11 = Factories.Sector.Create(s => s.Name = "aaaaaa");
            rootSector1.AddChild(sector11);
            var sector12 = Factories.Sector.Create(s => s.Name = "aaabbb");
            rootSector1.AddChild(sector12);

            var sector21 = Factories.Sector.Create();
            rootSector2.AddChild(sector21);

            // We must test associations after complete flush
            NHSession.Flush();
            NHSession.Clear();

            var result = new SectorHierarchicalQuery().List().ToList();
            
            result.Should().HaveCount(3);
            result[0].Name.Should().Be(rootSector1.Name);
            NHibernateUtil.IsInitialized(result[0].Children).Should().BeTrue();
            result[0].Children.Should().HaveCount(2);
            result[0].HasChildren().Should().BeTrue();
            result[0].Children.First().Name.Should().Be(sector11.Name);
            result[0].Children.Skip(1).First().Name.Should().Be(sector12.Name);

            result[1].Name.Should().Be(rootSector2.Name);
            NHibernateUtil.IsInitialized(result[1].Children).Should().BeTrue();
            result[1].Children.Should().HaveCount(1);
            result[1].HasChildren().Should().BeTrue();
            result[1].Children.First().Name.Should().Be(sector21.Name);

            result[2].Name.Should().Be(rootSector3.Name);
            result[2].Name.Should().Be(rootSector3.Name);
            NHibernateUtil.IsInitialized(result[2].Children).Should().BeTrue();
            result[2].Children.Should().HaveCount(0);
            result[2].HasChildren().Should().BeFalse();
        }
    }
}
