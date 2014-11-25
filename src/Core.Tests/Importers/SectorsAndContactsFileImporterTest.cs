// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorsAndContactsFileImporterTest.cs" company="CGI">
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
    public class SectorsAndContactsFileImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            for (var i = 0; i < 10; i++)
                Factories.Sector.Save();

            for (var i = 0; i < 50; i++)
                Factories.Contact.Save();

            using (var export = FileImporters.Export("SectorsAndContacts"))
            {
                var import = FileImporters.Import("SectorsAndContacts", export.Stream);
                import.Should().HaveCount(60);
                import.Should().OnlyContain(l => l.Status == LineResultStatus.Merged);
            }

            NHSession.QueryOver<Sector>().RowCount().Should().Be(10);
            NHSession.QueryOver<Contact>().RowCount().Should().Be(50);
        }
    }
}
