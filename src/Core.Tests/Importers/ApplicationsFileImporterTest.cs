// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsFileImporterTest.cs" company="CGI">
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
    public class ApplicationsFileImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            var techno = Factories.Technology.Save();
            var server = Factories.Server.Save();
            var dbInstance = Factories.DbInstance.Save();
            var contact = Factories.Contact.Save();

            for (var i = 0; i < 20; i++)
            {
                var app = Factories.Application.Save();
                app.AddTechnologyLink(techno);
                app.AddServerLink(server);
                app.AddDbInstanceLink(dbInstance);
                app.AddContactLink(contact);

                Factories.Event.Save(e => e.Application = app);
            }

            NHSession.Flush();
            NHSession.Clear();

            using (var export = FileImporters.Export("Applications"))
            {
                var import = FileImporters.Import("Applications", export.Stream);
                import.Should().HaveCount(140);
                import.Should().OnlyContain(l =>
                    l.Status == LineResultStatus.Merged
                 || (l.Status == LineResultStatus.Created && l.Section == "Events"));
            }

            NHSession.QueryOver<Application>().RowCount().Should().Be(40);
            NHSession.QueryOver<AppTechnoLink>().RowCount().Should().Be(20);
            NHSession.QueryOver<AppServerLink>().RowCount().Should().Be(20);
            NHSession.QueryOver<AppDbInstanceLink>().RowCount().Should().Be(20);
            NHSession.QueryOver<AppContactLink>().RowCount().Should().Be(20);
            NHSession.QueryOver<Event>().RowCount().Should().Be(40);
        }
    }
}
