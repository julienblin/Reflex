// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Organizations.Controllers;
using CGI.Reflex.Web.Areas.Organizations.Models.Contacts;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Organizations.Controllers
{
    public class ContactsControllerTest : BaseControllerTest<ContactsController>
    {
        [Test]
        public void Index_should_work_with_no_value_and_return_view()
        {
            StubStandardRequest();
            var result = Controller.Index(new ContactsIndex());
            result.Should().BeDefaultView();
            result.Model<ContactsIndex>().Should().NotBeNull();
            result.Model<ContactsIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_partial_view_when_ajax()
        {
            StubAjaxRequest();
            var result = Controller.Index(new ContactsIndex());
            result.Should().BePartialView("_List");
            result.Model<ContactsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Create_should_return_view()
        {
            var result = Controller.Create(Rand.Url(), 1);
            result.Should().BeDefaultView();
            result.Model<ContactEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_Contact()
        {
            ContactEdit contactEdit = new ContactEdit
            {
                LastName = Rand.String(),
                FirstName = Rand.String(),
                Company = Rand.String(),
                TypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType).Id,
                Email = Rand.String(),
                PhoneNumber = Rand.String(),
                Notes = Rand.LoremIpsum(),
                SectorId = Factories.Sector.Save().Id
            };

            var result = Controller.Create(contactEdit);
            result.Should().BeRedirectToActionName("Index");

            Contact contact = NHSession.QueryOver<Contact>().SingleOrDefault();
            contact.Should().NotBeNull();

            contact.LastName.Should().Be(contactEdit.LastName);
            contact.FirstName.Should().Be(contactEdit.FirstName);
            contact.Company.Should().Be(contactEdit.Company);
            contact.Type.Id.Should().Be(contactEdit.TypeId.Value);
            contact.Email.Should().Be(contactEdit.Email);
            contact.PhoneNumber.Should().Be(contactEdit.PhoneNumber);
            contact.Notes.Should().Be(contactEdit.Notes);
            contact.Sector.Id.Should().Be(contactEdit.SectorId.Value);
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            // Arrange
            var contact = Factories.Contact.Save();

            // Act   
            var result = Controller.Edit(contact.Id, Rand.Url(), 2);

            // Assert
            result.Should().BeDefaultView();
            result.Model<ContactEdit>().Id.Should().Be(contact.Id);
            result.Model<ContactEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_update_User()
        {
            Contact contact = Factories.Contact.Save();

            ContactEdit contactEdit = new ContactEdit
            {
                Id = contact.Id,
                ConcurrencyVersion = contact.ConcurrencyVersion,
                LastName = Rand.String(),
                FirstName = Rand.String(),
                Company = Rand.String(),
                TypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType).Id,
                Email = Rand.String(),
                PhoneNumber = Rand.String(),
                Notes = Rand.LoremIpsum(),
                SectorId = Factories.Sector.Save().Id
            };

            var result = Controller.Edit(contactEdit);
            result.Should().BeRedirectToActionName("Index");

            contact.LastName.Should().Be(contactEdit.LastName);
            contact.FirstName.Should().Be(contactEdit.FirstName);
            contact.Company.Should().Be(contactEdit.Company);
            contact.Type.Id.Should().Be(contactEdit.TypeId.Value);
            contact.Email.Should().Be(contactEdit.Email);
            contact.PhoneNumber.Should().Be(contactEdit.PhoneNumber);
            contact.Notes.Should().Be(contactEdit.Notes);
            contact.Sector.Id.Should().Be(contactEdit.SectorId.Value);
        }

        [Test]
        public void Delete_Popup()
        {
            var contact = Factories.Contact.Save();

            Controller.Delete(contact.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(contact.Id);

            result.Should().BePartialView("_DeleteModal");
            NHSession.Get<Contact>(contact.Id).Should().NotBeNull();
        }

        [Test]
        public void Delete_Submit()
        {
            var contact = Factories.Contact.Save();

            Controller.Delete(new ContactEdit { Id = contact.Id + 1 }).Should().BeHttpNotFound();
            var result = Controller.Delete(new ContactEdit { Id = contact.Id });

            result.Should().BeRedirectToActionName("Index");
            NHSession.Get<Contact>(contact.Id).Should().BeNull();
        }
    }
}
