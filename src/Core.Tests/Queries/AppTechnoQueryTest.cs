// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppTechnoQueryTest.cs" company="CGI">
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
    public class AppTechnoQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_fetch_technologies()
        {
            var technoApp1 = Factories.Technology.Save();
            var technoApp2 = Factories.Technology.Save();
            var technoApp3 = Factories.Technology.Save();

            var technoDb1 = Factories.Technology.Save();
            var technoDb2 = Factories.Technology.Save();
            var technoDb3 = Factories.Technology.Save();

            var technoInt1 = Factories.Technology.Save();
            var technoInt2 = Factories.Technology.Save();
            var technoInt3 = Factories.Technology.Save();

            var app = Factories.Application.Save();
            app.AddTechnologyLinks(new[] { technoApp1, technoApp2 });

            var int1 = Factories.Integration.Save(i => i.AppSource = app);
            int1.AddTechnologyLinks(new[] { technoInt1, technoInt2 });
            var int2 = Factories.Integration.Save(i => i.AppDest = app);
            int2.AddTechnologyLink(technoInt1);

            NHSession.Flush();

            var result = new AppTechnoQuery { ApplicationId = app.Id }.Execute();

            result.ApplicationTechnologies.Should().HaveCount(2);
            result.ApplicationTechnologies.Should().Contain(technoApp1);
            result.ApplicationTechnologies.Should().Contain(technoApp2);
            result.ApplicationTechnologies.Should().NotContain(technoApp3);

            result.IntegrationTechnologies.Should().HaveCount(2);
            result.IntegrationTechnologies.Should().Contain(technoInt1);
            result.IntegrationTechnologies.Should().Contain(technoInt2);
            result.IntegrationTechnologies.Should().NotContain(technoInt3);
        }
    }
}
