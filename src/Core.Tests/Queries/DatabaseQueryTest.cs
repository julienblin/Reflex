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
    public class DatabaseQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new DatabaseQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_AssociatedWithApplicationId()
        {
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            Factories.Database.Save();

            var app1 = Factories.Application.Save();
            app1.AddDatabaseLinks(new[] { db1, db2 });

            var app2 = Factories.Application.Save();
            app2.AddDatabaseLink(db2);

            var app3 = Factories.Application.Save();

            new DatabaseQuery { AssociatedWithApplicationId = app1.Id }.Count().Should().Be(2);
            new DatabaseQuery { AssociatedWithApplicationId = app2.Id }.Count().Should().Be(1);
            new DatabaseQuery { AssociatedWithApplicationId = app3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NotAssociatedWithApplicationId()
        {
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            Factories.Database.Save();

            var app1 = Factories.Application.Save();
            app1.AddDatabaseLinks(new[] { db1, db2 });

            var app2 = Factories.Application.Save();
            app2.AddDatabaseLink(db2);

            var app3 = Factories.Application.Save();

            new DatabaseQuery { NotAssociatedWithApplicationId = app1.Id }.Count().Should().Be(1);
            new DatabaseQuery { NotAssociatedWithApplicationId = app2.Id }.Count().Should().Be(2);
            new DatabaseQuery { NotAssociatedWithApplicationId = app3.Id }.Count().Should().Be(3);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Database.Save(a => a.Name = name1);
            Factories.Database.Save(a => a.Name = name2);
            Factories.Database.Save(a => a.Name = name3);

            new DatabaseQuery { NameLike = name1 }.Count().Should().Be(2);
            new DatabaseQuery { NameLike = name2 }.Count().Should().Be(1);
            new DatabaseQuery { NameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_Should_Filter_By_All_Criteria()
        {
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            Factories.Database.Save();

            var app1 = Factories.Application.Save();
            app1.AddDatabaseLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDatabaseLink(db2);

            new DatabaseQuery
            {
                AssociatedWithApplicationId = app1.Id,
                NotAssociatedWithApplicationId = app2.Id,
                NameLike = db1.Name
            }.Count().Should().Be(1);
        }
    }
}
