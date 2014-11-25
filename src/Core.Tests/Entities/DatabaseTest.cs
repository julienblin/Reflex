using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests.Factories;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class DatabaseTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Database>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_techno_links()
        {
            var database = Factories.Database.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            database.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Database>(database.Id);
            inttest.TechnologyLinks.Should().HaveCount(2);
            inttest.TechnologyLinks.Should().OnlyContain(tl => (tl.Technology.Id == techno1.Id) || (tl.Technology.Id == techno2.Id));
        }

        [Test]
        public void It_should_remove_techno_links()
        {
            var database = Factories.Database.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            database.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();

            database.RemoveTechnologyLink(techno1);

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Database>(database.Id);
            inttest.TechnologyLinks.Should().HaveCount(1);
            inttest.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno2.Id);

            NHSession.QueryOver<DatabaseTechnoLink>().RowCount().Should().Be(1);
        }

        [Test]
        public void It_should_add_server_links()
        {
            var database = Factories.Database.Save();
            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();

            var link1 = new DatabaseServerLink { Server = server1, DatabaseName = Rand.String(), DbInstanceName = Rand.String() };
            database.AddServerLink(link1);

            var link2 = new DatabaseServerLink { Server = server2, DatabaseName = Rand.String(), DbInstanceName = Rand.String() };
            database.AddServerLink(link2);

            NHSession.Flush();
            NHSession.Clear();

            var dbtest = NHSession.Get<Database>(database.Id);
            dbtest.ServerLinks.Should().HaveCount(2);
            dbtest.ServerLinks.Should().OnlyContain(dsl => (dsl.Server.Id == server1.Id) || (dsl.Server.Id == server2.Id));
        }

        [Test]
        public void It_should_remove_server_links()
        {
            var database = Factories.Database.Save();
            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();

            var link1 = new DatabaseServerLink { Server = server1, DatabaseName = Rand.String(), DbInstanceName = Rand.String() };
            database.AddServerLink(link1);

            var link2 = new DatabaseServerLink { Server = server2, DatabaseName = Rand.String(), DbInstanceName = Rand.String() };
            database.AddServerLink(link2);

            NHSession.Flush();

            database.RemoveServerLink(link1.Id);

            NHSession.Flush();
            NHSession.Clear();

            var dbtest = NHSession.Get<Database>(database.Id);
            dbtest.ServerLinks.Should().HaveCount(1);
            dbtest.ServerLinks.Should().OnlyContain(dsl => dsl.Server.Id == server2.Id);

            NHSession.QueryOver<DatabaseServerLink>().RowCount().Should().Be(1);
        }
    }
}
