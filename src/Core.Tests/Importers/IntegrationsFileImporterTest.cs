// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsFileImporterTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Importers;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Importers
{
    public class IntegrationsFileImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            for (var i = 0; i < 20; i++)
            {
                var integration = Factories.Integration.Save();
                if (i % 1 == 0)
                {
                    integration.AddTechnologyLink(techno1);
                    integration.AddTechnologyLink(techno2);
                }
                else
                {
                    integration.AddTechnologyLink(techno2);
                }
            }

            using (var export = FileImporters.Export("Integrations"))
            {
                var import = FileImporters.Import("Integrations", export.Stream);
                import.Should().HaveCount(60);
                import.Should().OnlyContain(l => l.Status == LineResultStatus.Merged);
            }

            NHSession.QueryOver<Integration>().RowCount().Should().Be(20);
            NHSession.QueryOver<IntegrationTechnoLink>().RowCount().Should().Be(40);
        }
    }
}
