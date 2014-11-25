// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.Contacts;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class ContactsControllerTest : BaseControllerTest<ContactsController>
    {
        [Test]
        public void Details_should_return_view()
        {
            var contact = Factories.Contact.Save();
            var application = Factories.Application.Save(a => a.AddContactLink(contact));

            var result = Controller.Details(application.Id, new ContactsDetails());
            
            result.Should().BeDefaultView();
            result.Model<ContactsDetails>().Should().NotBeNull();
            result.Model<ContactsDetails>().SearchResults.Items.Should().HaveCount(1);
        }

        [Test]
        public void AddContacts_should_add_contacts()
        {
            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save();
            var application = Factories.Application.Save();

            var result = Controller.AddContacts(application.Id, new[] { contact1.Id, contact2.Id });

            result.Should().BeRedirectToActionName("Details");

            application.ContactLinks.Should().HaveCount(2);
        }

        [Test]
        public void Edit_should_return_not_found_if_link_not_exist()
        {
            var result = Controller.Edit(1, Rand.Url(), 1);

            result.Should().BeHttpNotFound();
        }

        [Test]
        public void Edit_should_return_view()
        {
            var link = Factories.AppContactLink.Save();

            var result = Controller.Edit(link.Id, Rand.Url(), link.Application.Id);

            result.Should().BePartialView("_EditModal");
            result.Model<ContactLinkEdit>().Should().NotBeNull();
            result.Model<ContactLinkEdit>().Id.Should().Be(link.Id);
        }

        [Test]
        public void Edit_should_update_link()
        {
            var link = Factories.AppContactLink.Save();
            var roleApp1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);
            var roleApp2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactRoleInApp);

            var model = new ContactLinkEdit
            {
                Id = link.Id,
                RoleInAppIds = new[] { roleApp1.Id, roleApp2.Id }
            };

            StubStandardRequest();

            var result = Controller.Edit(model);
            result.Should().BeJSRedirect();
            link.RolesInApp.Should().HaveCount(2);
        }

        [Test]
        public void RemoveLink_should_return_not_found()
        {
            var result = Controller.RemoveLink(1);

            result.Should().BeHttpNotFound();
        }

        [Test]
        public void RemoveLink_should_return_view()
        {
            var link = Factories.AppContactLink.Save();

            var result = Controller.RemoveLink(link.Id);

            result.Should().BePartialView("_RemoveLinkModal");
            NHSession.Get<AppContactLink>(link.Id).Should().NotBeNull();
        }

        [Test]
        public void RemoveLink_should_delete()
        {
            var link = Factories.AppContactLink.Save();
            Factories.AppContactLink.Save();

            var model = new ContactLinkEdit { Id = link.Id };

            var result = Controller.RemoveLink(model);

            result.Should().BeOfType<RedirectToRouteResult>();
            NHSession.Get<AppContactLink>(link.Id).Should().BeNull();
            NHSession.QueryOver<AppContactLink>().List().Should().HaveCount(1);
        }
    }
}
