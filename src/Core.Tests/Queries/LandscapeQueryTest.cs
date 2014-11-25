// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeQueryTest.cs" company="CGI">
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
    public class LandscapeQueryTest : BaseDbTest
    {
        [Test]
        public void It_Should_Work_With_No_Entity()
        {
            new LandscapeQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_Name_Like()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Landscape.Save(a => a.Name = name1);
            Factories.Landscape.Save(a => a.Name = name2);
            Factories.Landscape.Save(a => a.Name = name3);

            new LandscapeQuery { NameLike = name1 }.Count().Should().Be(2);
            new LandscapeQuery { NameLike = name2 }.Count().Should().Be(1);
            new LandscapeQuery { NameLike = name3 }.Count().Should().Be(1);
        }
    }
}
