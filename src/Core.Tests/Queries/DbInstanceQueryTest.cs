// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class DbInstanceQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new DbInstanceQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_AssociatedWithApplicationId()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            Factories.DbInstance.Save();

            var app1 = Factories.Application.Save();
            app1.AddDbInstanceLinks(new[] { db1, db2 });

            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLink(db2);

            var app3 = Factories.Application.Save();

            new DbInstanceQuery { AssociatedWithApplicationId = app1.Id }.Count().Should().Be(2);
            new DbInstanceQuery { AssociatedWithApplicationId = app2.Id }.Count().Should().Be(1);
            new DbInstanceQuery { AssociatedWithApplicationId = app3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NotAssociatedWithApplicationId()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            Factories.DbInstance.Save();

            var app1 = Factories.Application.Save();
            app1.AddDbInstanceLinks(new[] { db1, db2 });

            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLink(db2);

            var app3 = Factories.Application.Save();

            new DbInstanceQuery { NotAssociatedWithApplicationId = app1.Id }.Count().Should().Be(1);
            new DbInstanceQuery { NotAssociatedWithApplicationId = app2.Id }.Count().Should().Be(2);
            new DbInstanceQuery { NotAssociatedWithApplicationId = app3.Id }.Count().Should().Be(3);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.DbInstance.Save(a => a.Name = name1);
            Factories.DbInstance.Save(a => a.Name = name2);
            Factories.DbInstance.Save(a => a.Name = name3);

            new DbInstanceQuery { NameLike = name1 }.Count().Should().Be(2);
            new DbInstanceQuery { NameLike = name2 }.Count().Should().Be(1);
            new DbInstanceQuery { NameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_All_Criteria()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            Factories.DbInstance.Save();

            var app1 = Factories.Application.Save();
            app1.AddDbInstanceLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLink(db2);

            new DbInstanceQuery
            {
                AssociatedWithApplicationId = app1.Id,
                NotAssociatedWithApplicationId = app2.Id,
                NameLike = db1.Name
            }.Count().Should().Be(1);
        }
    }
}
