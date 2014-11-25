// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PickersControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Models.Pickers;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class PickersControllerTest : BaseControllerTest<PickersController>
    {
        [Test]
        public void ContactsModal_should_return_view_with_all_contacts()
        {
            for (var i = 0; i < 50; i++)
                Factories.Contact.Save();

            var model = new ContactsModalOptions
            {
                PostUrl = Rand.Url()
            };

            var result = Controller.ContactsModal(model);
            result.Should().BePartialView("_ContactsModal");

            result.Model<ContactsList>().PostUrl.Should().Be(model.PostUrl);
            result.Model<ContactsList>().Contacts.Should().HaveCount(50);
        }

        [Test]
        public void ContactDetails_should_return_view_with_details()
        {
            Controller.ContactDetails(Rand.Int(5000)).Should().BeHttpNotFound();

            var contact = Factories.Contact.Save();

            var result = Controller.ContactDetails(contact.Id);

            result.Should().BePartialView("_ContactDetails");
            result.Model<ContactDetails>().Id.Should().Be(contact.Id);
            result.Model<ContactDetails>().FirstName.Should().Be(contact.FirstName);
            result.Model<ContactDetails>().LastName.Should().Be(contact.LastName);
            result.Model<ContactDetails>().Company.Should().Be(contact.Company);
            result.Model<ContactDetails>().Email.Should().Be(contact.Email);
            result.Model<ContactDetails>().ContactTypeId.Should().Be(contact.Type.Id);
        }

        [Test]
        public void SectorsModal_should_return_view()
        {
            var rootSector1 = Factories.Sector.Save();
            var rootSector2 = Factories.Sector.Save();

            var subSector1 = Factories.Sector.Save();
            rootSector1.AddChild(subSector1);

            var options = new SectorsModalOptions
            {
                TargetUpdateId = Rand.String()
            };

            var result = Controller.SectorsModal(options);
            result.Should().BePartialView("_SectorsModal");
            result.Model<SectorHierarchy>().RootSectors.Should().HaveCount(2);
        }

        [Test]
        public void TechnologiesModal_should_return_view()
        {
            var rootTechno3 = Factories.Technology.Save(t => t.Name = "ccc");
            var rootTechno2 = Factories.Technology.Save(t => t.Name = "bbb");
            var rootTechno1 = Factories.Technology.Save(t => t.Name = "aaa");

            var sector11 = Factories.Technology.Create(t => t.Name = "aaaaaa");
            rootTechno1.AddChild(sector11);
            var sector12 = Factories.Technology.Create(t => t.Name = "aaabbb");
            rootTechno1.AddChild(sector12);

            var sector21 = Factories.Technology.Create();
            rootTechno2.AddChild(sector21);

            var options = new TechnologiesModalOptions()
            {
                PostUrl = Rand.String()
            };

            var result = Controller.TechnologiesModal(options);
            result.Should().BePartialView("_TechnologiesModal");
            result.Model<TechnologyHierarchy>().RootTechnologies.Should().HaveCount(3);
            result.Model<TechnologyHierarchy>().PostUrl.Should().Be(options.PostUrl);
            result.Model<TechnologyHierarchy>().TechnologyTypes.Should().NotBeEmpty();
        }

        [Test]
        public void TechnologiesDetails_should_return_view()
        {
            var techno = Factories.Technology.Save();

            Controller.TechnologiesDetails(techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.TechnologiesDetails(techno.Id);
            result.Should().BePartialView("_TechnologyDetails");
            result.Model<TechnologyDetails>().Name.Should().Be(techno.Name);
        }

        [Test]
        public void ServersModal_should_return_all_servers()
        {
            for (int i = 0; i < 50; i++)
                Factories.Server.Save();

            var model = new ServersModalOptions
            {
                PostUrl = Rand.Url(),
            };

            var result = Controller.ServersModal(model);

            result.Model<ServersList>().PostUrl.Should().Be(model.PostUrl);
            result.Model<ServersList>().HideWithLandscape.Should().Be(false);
            result.Model<ServersList>().Servers.Should().HaveCount(50);
        }

        [Test]
        public void ServersModal_should_filter_with_landscape()
        {
            for (int i = 0; i < 10; i++)
                Factories.Server.Save(x => x.Landscape = null);

            Landscape landscape1 = Factories.Landscape.Save();
            Landscape landscape2 = Factories.Landscape.Save();
            Landscape landscape3 = Factories.Landscape.Save();

            for (int i = 0; i < 4; i++)
                Factories.Server.Save(s => s.Landscape = landscape1);

            for (int i = 0; i < 6; i++)
                Factories.Server.Save(s => s.Landscape = landscape2);

            for (int i = 0; i < 8; i++)
                Factories.Server.Save(s => s.Landscape = landscape3);

            var model = new ServersModalOptions
            {
                PostUrl = Rand.Url(),
                HideWithLandscape = true,
                CurLandscapeId = landscape3.Id
            };

            var result = Controller.ServersModal(model);

            result.Model<ServersList>().PostUrl.Should().Be(model.PostUrl);
            result.Model<ServersList>().HideWithLandscape.Should().Be(true);
            result.Model<ServersList>().Servers.Should().HaveCount(18);
        }

        [Test]
        public void ServerDetails_should_return_view()
        {
            var server = Factories.Server.Save();

            Controller.ServerDetails(server.Id + 1).Should().BeHttpNotFound();

            var result = Controller.ServerDetails(server.Id);
            result.Should().BePartialView("_ServerDetails");
            result.Model<ServerDetails>().Name.Should().Be(server.Name);
        }
    }
}
