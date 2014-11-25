// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="CGI">
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
    public class IntegrationTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Integration>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(15))
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .CheckProperty(x => x.DataDescription, Rand.LoremIpsum())
                .CheckProperty(x => x.Frequency, Rand.String(40))
                .CheckProperty(x => x.Comments, Rand.LoremIpsum())
                .CheckReference(x => x.AppSource, Factories.Application.Save())
                .CheckReference(x => x.AppDest, Factories.Application.Save())
                .CheckReference(x => x.Nature, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature))
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_techno_links()
        {
            var integration = Factories.Integration.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            integration.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Integration>(integration.Id);
            inttest.TechnologyLinks.Should().HaveCount(2);
            inttest.TechnologyLinks.Should().OnlyContain(tl => (tl.Technology.Id == techno1.Id) || (tl.Technology.Id == techno2.Id));
        }

        [Test]
        public void It_should_remove_techno_links()
        {
            var integration = Factories.Integration.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            integration.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();

            integration.RemoveTechnologyLink(techno1);

            NHSession.Flush();
            NHSession.Clear();

            var inttest = NHSession.Get<Integration>(integration.Id);
            inttest.TechnologyLinks.Should().HaveCount(1);
            inttest.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno2.Id);

            NHSession.QueryOver<IntegrationTechnoLink>().RowCount().Should().Be(1);
        }
    }
}
