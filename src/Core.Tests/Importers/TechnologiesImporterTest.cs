// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesImporterTest.cs" company="CGI">
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
    public class TechnologiesImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => t.Parent = rootTechno);
            var parentTechno2 = Factories.Technology.Save(t => t.Parent = rootTechno);

            for (var i = 0; i < 10; i++)
                Factories.Technology.Save(t => t.Parent = parentTechno1);

            for (var i = 0; i < 10; i++)
                Factories.Technology.Save(t => t.Parent = parentTechno2);

            NHSession.Flush();
            NHSession.Clear();

            using (var export = FileImporters.Export("Technologies"))
            {
                var import = FileImporters.Import("Technologies", export.Stream);
                import.Should().HaveCount(20);
                import.Should().OnlyContain(l => l.Status == LineResultStatus.Merged);
            }

            NHSession.QueryOver<Technology>().RowCount().Should().Be(23);
        }
    }
}
