// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesImporterTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Importers;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Importers
{
    public class DomainValuesImporterTest : BaseDbTest
    {
        [Test]
        public void It_should_round_trip()
        {
            for (var i = 0; i < 50; i++)
                Factories.DomainValue.Save();

            using (var export = FileImporters.Export("DomainValues"))
            {
                var import = FileImporters.Import("DomainValues", export.Stream);
                import.Should().HaveCount(50);
                import.Should().OnlyContain(l => l.Status == LineResultStatus.Merged);
            }

            new DomainValueQuery().Count().Should().Be(50);
        }
    }
}
