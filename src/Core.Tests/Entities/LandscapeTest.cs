// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class LandscapeTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Landscape>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_retrieve_servers()
        {
            var landscape = Factories.Landscape.Save();
            var server1 = Factories.Server.Save(s => s.Landscape = landscape);
            var server2 = Factories.Server.Save();

            NHSession.Refresh(landscape);
            landscape.Servers.Should().OnlyContain(s => s == server1);
        }
    }
}
