// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyQueryTest.cs" company="CGI">
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
    public class TechnologyQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new TechnologyQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Technology.Save(t => t.Name = name1);
            Factories.Technology.Save(t => t.Name = name2);
            Factories.Technology.Save(t => t.Name = name3);

            new TechnologyQuery { NameLike = name1 }.Count().Should().Be(2);
            new TechnologyQuery { NameLike = name2 }.Count().Should().Be(1);
            new TechnologyQuery { NameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_TechnologyTypeId()
        {
            var technologyType = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.TechnologyType);

            Factories.Technology.Save(t => t.TechnologyType = technologyType);
            Factories.Technology.Save();
            Factories.Technology.Save();

            new TechnologyQuery { TechnologyTypeId = technologyType.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var t = Factories.Technology.Save();

            new TechnologyQuery
            {
                NameLike = t.Name,
                TechnologyTypeId = t.TechnologyType.Id
            }.Count().Should().Be(1);
        }
    }
}
