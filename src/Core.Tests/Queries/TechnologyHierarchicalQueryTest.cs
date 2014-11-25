// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyHierarchicalQueryTest.cs" company="CGI">
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
    public class TechnologyHierarchicalQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_return_hierarchy_of_technologies_order_by_name()
        {
            var rootTechno3 = Factories.Technology.Save(t => t.Name = "ccc");
            var rootTechno2 = Factories.Technology.Save(t => t.Name = "bbb");
            var rootTechno1 = Factories.Technology.Save(t => t.Name = "aaa");

            var sector11 = Factories.Technology.Create(t => t.Name = "aaaaaa");
            rootTechno1.AddChild(sector11);
            var sector12 = Factories.Technology.Create(t => t.Name = "aaabbb");
            rootTechno1.AddChild(sector12);

            var sector21 = Factories.Technology.Create();
            rootTechno2.AddChild(sector21);

            // We must test associations after complete flush
            NHSession.Flush();
            NHSession.Clear();

            var result = new TechnologyHierarchicalQuery().List().ToList();
            
            result.Should().HaveCount(3);
            result[0].Name.Should().Be(rootTechno1.Name);
            NHibernateUtil.IsInitialized(result[0].Children).Should().BeTrue();
            result[0].Children.Should().HaveCount(2);
            result[0].HasChildren().Should().BeTrue();
            result[0].Children.First().Name.Should().Be(sector11.Name);
            result[0].Children.Skip(1).First().Name.Should().Be(sector12.Name);

            result[1].Name.Should().Be(rootTechno2.Name);
            NHibernateUtil.IsInitialized(result[1].Children).Should().BeTrue();
            result[1].Children.Should().HaveCount(1);
            result[1].HasChildren().Should().BeTrue();
            result[1].Children.First().Name.Should().Be(sector21.Name);

            result[2].Name.Should().Be(rootTechno3.Name);
            result[2].Name.Should().Be(rootTechno3.Name);
            NHibernateUtil.IsInitialized(result[2].Children).Should().BeTrue();
            result[2].Children.Should().HaveCount(0);
            result[2].HasChildren().Should().BeFalse();
        }
    }
}
