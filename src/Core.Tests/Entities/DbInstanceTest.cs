// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstanceTest.cs" company="CGI">
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
    public class DbInstanceTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<DbInstance>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.Server, Factories.Server.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_techno_links()
        {
            var server = Factories.Server.Save();
            var dbInstance = Factories.DbInstance.Save(db => db.Server = server);
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            dbInstance.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();

            var inttest = NHSession.Get<DbInstance>(dbInstance.Id);
            inttest.TechnologyLinks.Should().HaveCount(2);
            inttest.TechnologyLinks.Should().OnlyContain(tl => (tl.Technology.Id == techno1.Id) || (tl.Technology.Id == techno2.Id));
        }

        [Test]
        public void It_should_remove_techno_links()
        {
            var server = Factories.Server.Save();
            var dbInstance = Factories.DbInstance.Save(db => db.Server = server);
            var techno1 = Factories.Technology.Save();

            dbInstance.AddTechnologyLinks(new[] { techno1 });

            NHSession.Flush();

            dbInstance.RemoveTechnologyLink(techno1);

            NHSession.Flush();

            var inttest = NHSession.Get<DbInstance>(dbInstance.Id);
            inttest.TechnologyLinks.Should().HaveCount(0);
        }
    }
}
