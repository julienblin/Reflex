// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLinkTest.cs" company="CGI">
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
    public class AppContactLinkTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<AppContactLink>(NHSession, new PersistenceEqualityComparer())
                .CheckReference(x => x.Application, Factories.Application.Save())
                .CheckReference(x => x.Contact, Factories.Contact.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_RoleInApp()
        {
            var application = Factories.Application.Save();
            var contact = Factories.Contact.Save();
            var roleInApp = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);

            application.AddContactLink(contact);
            application.ContactLinks.First().AddRoleInApp(roleInApp);

            NHSession.Flush();
            NHSession.Clear();

            application = NHSession.Get<Application>(application.Id);

            application.ContactLinks.First().RolesInApp.Should().OnlyContain(ria => ria.Id == roleInApp.Id);
        }

        [Test]
        public void It_should_remove_RoleInApp()
        {
            var application = Factories.Application.Save();
            var contact = Factories.Contact.Save();
            var roleInApp1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);
            var roleInApp2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);

            application.AddContactLink(contact);
            application.ContactLinks.First().AddRoleInApp(roleInApp1);
            application.ContactLinks.First().AddRoleInApp(roleInApp2);

            NHSession.Flush();
            NHSession.Clear();

            application = NHSession.Get<Application>(application.Id);

            application.ContactLinks.First().RemoveRoleInApp(roleInApp2.Id);

            NHSession.Flush();
            NHSession.Clear();

            application.ContactLinks.First().RolesInApp.Should().OnlyContain(ria => ria.Id == roleInApp1.Id);
        }

        [Test]
        public void It_should_set_RoleInApp()
        {
            var application = Factories.Application.Save();
            var contact = Factories.Contact.Save();
            var roleInApp1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);
            var roleInApp2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);

            application.AddContactLink(contact);
            application.ContactLinks.First().AddRoleInApp(roleInApp1);

            NHSession.Flush();
            NHSession.Clear();

            application = NHSession.Get<Application>(application.Id);

            application.ContactLinks.First().SetRoleInApp(new[] { roleInApp2 });

            NHSession.Flush();
            NHSession.Clear();

            application.ContactLinks.First().RolesInApp.Should().OnlyContain(ria => ria.Id == roleInApp2.Id);
        }
    }
}
