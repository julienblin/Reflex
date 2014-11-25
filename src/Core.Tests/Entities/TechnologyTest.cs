// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyTest.cs" company="CGI">
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
    public class TechnologyTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Technology>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.EndOfSupport, Rand.DateTime(future: true))
                .CheckReference(x => x.TechnologyType, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.TechnologyType))
                .CheckReference(x => x.Contact, Factories.Contact.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_have_linked_applications()
        {
            var techno = Factories.Technology.Save();
            var app = Factories.Application.Save();
            app.AddTechnologyLink(techno);

            NHSession.Flush();
            NHSession.Refresh(techno);
            techno.ApplicationLinks.Should().OnlyContain(atl => atl.Application == app);
        }

        [Test]
        public void It_should_have_linked_integrations()
        {
            var techno = Factories.Technology.Save();
            var integration = Factories.Integration.Save();
            integration.AddTechnologyLink(techno);

            NHSession.Flush();
            NHSession.Refresh(techno);
            techno.IntegrationLinks.Should().OnlyContain(atl => atl.Integration == integration);
        }

        [Test]
        public void It_should_return_AllIds()
        {
            var techno1 = Factories.Technology.Save();
            var techno11 = Factories.Technology.Save(t => t.Parent = techno1);
            var techno12 = Factories.Technology.Save(t => t.Parent = techno1);
            var techno111 = Factories.Technology.Save(t => t.Parent = techno11);

            NHSession.Refresh(techno1);
            NHSession.Refresh(techno11);
            NHSession.Refresh(techno12);
            NHSession.Refresh(techno111);

            techno1.AllIds.Should().HaveCount(4);
            techno1.AllIds.Should().OnlyContain(id => new[] { techno1.Id, techno11.Id, techno12.Id, techno111.Id }.Contains(id));
        }
    }
}
