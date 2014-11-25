// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueQueryTest.cs" company="CGI">
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
    public class DomainValueQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new DomainValueQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_Category()
        {
            var category1 = Rand.Enum<DomainValueCategory>();
            var category2 = Rand.Enum<DomainValueCategory>();
            while (category2 == category1)
                category2 = Rand.Enum<DomainValueCategory>();

            Factories.DomainValue.Save(dv => dv.Category = category1);
            Factories.DomainValue.Save(dv => dv.Category = category1);
            Factories.DomainValue.Save(dv => dv.Category = category2);

            new DomainValueQuery { Category = category1 }.Count().Should().Be(2);
            new DomainValueQuery { Category = category2 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_Name()
        {
            var name1 = Rand.String();
            var name2 = Rand.String();

            Factories.DomainValue.Save(dv => dv.Name = name1);
            Factories.DomainValue.Save(dv => dv.Name = name1);
            Factories.DomainValue.Save(dv => dv.Name = name2);

            new DomainValueQuery { Name = name1 }.Count().Should().Be(2);
            new DomainValueQuery { Name = name2 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var dv1 = Factories.DomainValue.Save();
            Factories.DomainValue.Save();
            Factories.DomainValue.Save();

            new DomainValueQuery
            {
                Category = dv1.Category,
                Name = dv1.Name
            }.Count().Should().Be(1);
        }
    }
}
