// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesAndServersFileImporterTest.cs" company="CGI">
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
    public class LandscapesAndServersFileImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            for (var i = 0; i < 20; i++)
                Factories.Server.Save();

            for (var i = 0; i < 10; i++)
            {
                var server = Factories.Server.Save();
                server.AddTechnologyLinks(new[] { techno1, techno2 });
            }

            using (var export = FileImporters.Export("LandscapesAndServers"))
            {
                var import = FileImporters.Import("LandscapesAndServers", export.Stream);
                import.Should().HaveCount(80);
                import.Should().OnlyContain(l => l.Status == LineResultStatus.Merged);
            }

            NHSession.QueryOver<Landscape>().RowCount().Should().Be(30);
            NHSession.QueryOver<Server>().RowCount().Should().Be(30);
            NHSession.QueryOver<ServerTechnoLink>().RowCount().Should().Be(20);
        }
    }
}
