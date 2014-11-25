// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class RoleQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new RoleQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String();
            var name2 = name1 + Rand.String();
            var name3 = Rand.String();

            Factories.Role.Save(a => a.Name = name1);
            Factories.Role.Save(a => a.Name = name2);
            Factories.Role.Save(a => a.Name = name3);

            new RoleQuery { NameLike = name1 }.Count().Should().Be(2);
            new RoleQuery { NameLike = name2 }.Count().Should().Be(1);
            new RoleQuery { NameLike = name3 }.Count().Should().Be(1);
        }
    }
}
