// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Calculation;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Calculation
{
    public class TechnologiesTest : BaseDbTest
    {
        [Test]
        public void It_should_return_null_value_when_no_techno()
        {
            var application = Factories.Application.Save();

            new Technologies().Calculate(application).Should().Be(null);
        }

        [Test]
        public void It_should_return_100_when_all_techno_are_up_top_date()
        {
            var application = Factories.Application.Save();

            var techno1 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddYears(1).AddDays(1));
            var techno2 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddYears(2));

            application.AddTechnologyLink(techno1);
            application.AddTechnologyLink(techno2);

            new Technologies().Calculate(application).Should().Be(100);
        }

        [Test]
        public void It_should_return_25_when_all_techno_have_1_year_left()
        {
            var application = Factories.Application.Save();

            var techno1 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddMonths(Rand.Int(11) + 1));
            var techno2 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddDays(1));

            application.AddTechnologyLink(techno1);
            application.AddTechnologyLink(techno2);

            new Technologies().Calculate(application).Should().Be(25);
        }

        [Test]
        public void It_should_return_0_when_all_techno_are_outdated()
        {
            var application = Factories.Application.Save();

            var techno1 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddMonths(-(Rand.Int(12) + 1)));
            var techno2 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddDays(-1));

            application.AddTechnologyLink(techno1);
            application.AddTechnologyLink(techno2);

            new Technologies().Calculate(application).Should().Be(0);
        }

        [Test]
        public void It_should_return_25_with_all_techno_date_possibility()
        {
            var application = Factories.Application.Save();

            var techno1 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddYears(2));
            var techno2 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddDays(1));
            var techno3 = Factories.Technology.Save(t => t.EndOfSupport = DateTime.Now.AddMonths(-(Rand.Int(12) + 1)));

            application.AddTechnologyLink(techno1);
            application.AddTechnologyLink(techno2);
            application.AddTechnologyLink(techno3);

            new Technologies().Calculate(application).Should().Be(25);
        }
    }
}
