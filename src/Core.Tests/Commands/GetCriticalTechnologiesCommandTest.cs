// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCriticalTechnologiesCommandTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Commands
{
    public class GetCriticalTechnologiesCommandTest : BaseDbTest
    {
        [Test]
        public void It_should_have_results()
        {
            var validTechno1 = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: true) + TimeSpan.FromDays(400));
            var validTechno2 = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: true) + TimeSpan.FromDays(400));

            var mediumTechno = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: true));

            var outDatedTechno = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: false));

            var riskyApp = Factories.Application.Save();
            riskyApp.AddTechnologyLink(outDatedTechno);

            for (var i = 0; i < 10; i++)
            {
                var mediumApp = Factories.Application.Save();
                mediumApp.AddTechnologyLink(mediumTechno);
            }

            for (var i = 0; i < 30; i++)
            {
                var okApp = Factories.Application.Save();
                okApp.AddTechnologyLink(validTechno1);
            }

            NHSession.Flush();
            NHSession.Clear();

            var result = new GetCriticalTechnologiesCommand().Execute().ToList();
            result.Should().HaveCount(4);

            result[0].GetTechno().Id.Should().Be(mediumTechno.Id);
            result[1].GetTechno().Id.Should().Be(outDatedTechno.Id);
            result[2].GetTechno().Id.Should().Be(validTechno1.Id);
            result[3].GetTechno().Id.Should().Be(validTechno2.Id);
        }

        [Test]
        public void It_should_not_return_technologies_with_no_endofsupport_date()
        {
            var validTechno1 = Factories.Technology.Save(t => t.EndOfSupport = null);
            var validTechno2 = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: true) + TimeSpan.FromDays(400));

            var mediumTechno = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: true));

            var outDatedTechno = Factories.Technology.Save(t => t.EndOfSupport = Rand.DateTime(future: false));

            var riskyApp = Factories.Application.Save();
            riskyApp.AddTechnologyLink(outDatedTechno);

            for (var i = 0; i < 10; i++)
            {
                var mediumApp = Factories.Application.Save();
                mediumApp.AddTechnologyLink(mediumTechno);
            }

            for (var i = 0; i < 30; i++)
            {
                var okApp = Factories.Application.Save();
                okApp.AddTechnologyLink(validTechno1);
            }

            NHSession.Flush();
            NHSession.Clear();

            var result = new GetCriticalTechnologiesCommand().Execute().ToList();
            result.Should().HaveCount(3);

            result[0].GetTechno().Id.Should().Be(mediumTechno.Id);
            result[1].GetTechno().Id.Should().Be(outDatedTechno.Id);
            result[2].GetTechno().Id.Should().Be(validTechno2.Id);
        }
    }
}
