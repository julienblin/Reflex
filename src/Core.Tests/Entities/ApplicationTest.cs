// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationTest.cs" company="CGI">
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
    public class ApplicationTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Application>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.Code, Rand.String(5))
                .CheckReference(x => x.AppInfo.Sector, Factories.Sector.Save())
                .CheckProperty(x => x.AppInfo.Description, Rand.LoremIpsum())
                .CheckProperty(x => x.AppInfo.MaintenanceWindow, Rand.LoremIpsum())
                .CheckProperty(x => x.AppInfo.Notes, Rand.LoremIpsum())
                .CheckReference(x => x.ApplicationType, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType))
                .CheckReference(x => x.AppInfo.Status, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus))
                .CheckReference(x => x.AppInfo.Criticity, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity))
                .CheckReference(x => x.AppInfo.UserRange, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationUserRange))
                .CheckReference(x => x.AppInfo.Certification, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCertification))
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_add_techno_links()
        {
            var application = Factories.Application.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            application.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.TechnologyLinks.Should().HaveCount(2);
            apptest.TechnologyLinks.Should().OnlyContain(tl => (tl.Technology.Id == techno1.Id) || (tl.Technology.Id == techno2.Id));
        }

        [Test]
        public void It_should_remove_techno_links()
        {
            var application = Factories.Application.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            application.AddTechnologyLinks(new[] { techno1, techno2 });

            NHSession.Flush();

            application.RemoveTechnologyLink(techno1);

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.TechnologyLinks.Should().HaveCount(1);
            apptest.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno2.Id);

            NHSession.QueryOver<AppTechnoLink>().RowCount().Should().Be(1);
        }

        [Test]
        public void It_should_add_contact_links()
        {
            var application = Factories.Application.Save();
            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save();

            application.AddContactLinks(new[] { contact1, contact2 });

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.ContactLinks.Should().HaveCount(2);
            apptest.ContactLinks.Should().OnlyContain(tl => (tl.Contact.Id == contact1.Id) || (tl.Contact.Id == contact2.Id));
        }

        [Test]
        public void It_should_remove_contact_links()
        {
            var application = Factories.Application.Save();
            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save();

            application.AddContactLinks(new[] { contact1, contact2 });

            NHSession.Flush();

            application.RemoveContactLink(contact1);

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.ContactLinks.Should().HaveCount(1);
            apptest.ContactLinks.Should().OnlyContain(cl => cl.Contact.Id == contact2.Id);

            NHSession.QueryOver<AppContactLink>().RowCount().Should().Be(1);
        }

        [Test]
        public void It_should_add_dbInstance_links()
        {
            var application = Factories.Application.Save();
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();

            application.AddDbInstanceLinks(new[] { db1, db2 });

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.DbInstanceLinks.Should().HaveCount(2);
            apptest.DbInstanceLinks.Should().OnlyContain(dl => (dl.DbInstances.Id == db1.Id) || (dl.DbInstances.Id == db2.Id));
        }

        [Test]
        public void It_should_remove_dbInstance_links()
        {
            var application = Factories.Application.Save();
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            var db3 = Factories.DbInstance.Save();

            application.AddDbInstanceLinks(new[] { db1, db2, db3 });

            NHSession.Flush();

            application.RemoveDbInstanceLink(db1);
            application.RemoveDbInstanceLink(db3);

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.DbInstanceLinks.Should().HaveCount(1);
            apptest.DbInstanceLinks.Should().OnlyContain(dl => dl.DbInstances.Id == db2.Id);

            NHSession.QueryOver<AppDbInstanceLink>().RowCount().Should().Be(1);
        }

        [Test]
        public void It_should_add_server_links()
        {
            var application = Factories.Application.Save();
            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();

            application.AddServerLinks(new[] { server1, server2 });

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.ServerLinks.Should().HaveCount(2);
            apptest.ServerLinks.Should().OnlyContain(sl => (sl.Server.Id == server1.Id) || (sl.Server.Id == server2.Id));
        }

        [Test]
        public void It_should_remove_server_links()
        {
            var application = Factories.Application.Save();
            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();

            application.AddServerLinks(new[] { server1, server2 });

            NHSession.Flush();

            application.RemoveServerLink(server1);

            NHSession.Flush();
            NHSession.Clear();

            var apptest = NHSession.Get<Application>(application.Id);
            apptest.ServerLinks.Should().HaveCount(1);
            apptest.ServerLinks.Should().OnlyContain(sl => sl.Server.Id == server2.Id);

            NHSession.QueryOver<AppServerLink>().RowCount().Should().Be(1);
        }
    }
}
