// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTest.cs" company="CGI">
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
    public class ServerTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Server>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(15))
                .CheckProperty(x => x.Alias, Rand.String(15))
                .CheckProperty(x => x.Comments, Rand.LoremIpsum())
                .CheckProperty(x => x.EvergreenDate, Rand.DateTime(future: true))
                .CheckReference(x => x.Environment, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment))
                .CheckReference(x => x.Role, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole))
                .CheckReference(x => x.Status, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus))
                .CheckReference(x => x.Type, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType))
                .CheckReference(x => x.Landscape, Factories.Landscape.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_techno_links()
        {
            var server = Factories.Server.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            server.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Server>(server.Id);
            inttest.TechnologyLinks.Should().HaveCount(2);
            inttest.TechnologyLinks.Should().OnlyContain(tl => (tl.Technology.Id == techno1.Id) || (tl.Technology.Id == techno2.Id));
        }

        [Test]
        public void It_should_remove_techno_links()
        {
            var server = Factories.Server.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            server.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();

            server.RemoveTechnologyLink(techno1);

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Server>(server.Id);
            inttest.TechnologyLinks.Should().HaveCount(1);
            inttest.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno2.Id);

            NHSession.QueryOver<ServerTechnoLink>().RowCount().Should().Be(1);
        }
    }
}
