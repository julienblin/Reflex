// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Technologies.Controllers;
using CGI.Reflex.Web.Areas.Technologies.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Technologies.Controllers
{
    public class TechnologiesControllerTest : BaseControllerTest<TechnologiesController>
    {
        [Test]
        public void Index_should_return_view()
        {
            Factories.Technology.Save();
            Factories.Technology.Save();

            var result = Controller.Index();
            result.Should().BeDefaultView();
            result.Model<TechnologyHierarchy>().RootTechnologies.Should().HaveCount(2);
        }

        [Test]
        public void Details_should_return_view()
        {
            var contact = Factories.Contact.Save();
            var techno = Factories.Technology.Save(t => t.Contact = contact);

            Controller.Details(techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Details(techno.Id);
            result.Should().BePartialView("_Details");
            result.Model<TechnologyEdit>().Id.Should().Be(techno.Id);
            result.Model<TechnologyEdit>().Name.Should().Be(techno.Name);
            result.Model<TechnologyEdit>().ParentFullName.Should().BeEmpty();
            result.Model<TechnologyEdit>().EndOfSupport.Should().Be(techno.EndOfSupport);
            result.Model<TechnologyEdit>().TechnologyTypeId.Should().Be(techno.TechnologyType.ToId());
            result.Model<TechnologyEdit>().ContactId.Should().Be(techno.Contact.ToId());
            result.Model<TechnologyEdit>().HasChildren.Should().Be(techno.HasChildren());
        }

        [Test]
        public void TreeData_should_return_tree_info()
        {
            var childTechno = Factories.Technology.Save();
            var parentTechno = Factories.Technology.Create();
            parentTechno.AddChild(childTechno);

            NHSession.Save(parentTechno);

            var result = Controller.TreeData(parentTechno.Id);
            result.Should().BeJsonResult();
            var jsonResultData = result.As<JsonResult>().Data as IEnumerable<dynamic>;
            jsonResultData.Should().HaveCount(1);
        }

        [Test]
        public void TreeData_should_return_empty_array_on_not_found()
        {
            var result = Controller.TreeData(0);
            result.Should().BeJsonResult();
            var jsonResultData = result.As<JsonResult>().Data as IEnumerable<dynamic>;
            jsonResultData.Should().HaveCount(0);
        }

        [Test]
        public void Create_should_return_view()
        {
            var parentTechno = Factories.Technology.Save();

            Controller.Create(parentTechno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Create(parentTechno.Id);
            result.Should().BePartialView("_Form");
            result.Model<TechnologyEdit>().ParentId.Should().Be(parentTechno.Id);
            result.Model<TechnologyEdit>().ParentFullName.Should().Be(parentTechno.FullName);
            result.Model<TechnologyEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_techno()
        {
            var parentTechno = Factories.Technology.Save();
            Controller.Create(new TechnologyEdit { ParentId = parentTechno.Id + 1 }).Should().BeHttpNotFound();

            var model = new TechnologyEdit
            {
                ParentId = parentTechno.Id,
                Name = Rand.String(),
                EndOfSupport = Rand.DateTime(future: true),
                TechnologyTypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.TechnologyType).Id,
                ContactId = Factories.Contact.Save().Id
            };

            var result = Controller.Create(model);
            result.Should().BePartialView("_Details");
            result.Model<TechnologyEdit>().JustCreated.Should().BeTrue();

            var techno = NHSession.QueryOver<Technology>().Where(t => t.Parent == parentTechno).SingleOrDefault();
            techno.Should().NotBeNull();
            techno.Name.Should().Be(model.Name);
            techno.EndOfSupport.Should().Be(model.EndOfSupport);
            techno.TechnologyType.Id.Should().Be(model.TechnologyTypeId.Value);
            techno.Contact.Id.Should().Be(model.ContactId.Value);
        }

        [Test]
        public void Edit_should_return_view()
        {
            var techno = Factories.Technology.Save();

            Controller.Edit(techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Edit(techno.Id);
            result.Should().BePartialView("_Form");
            result.Model<TechnologyEdit>().Id.Should().Be(techno.Id);
            result.Model<TechnologyEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_edit_techno()
        {
            var techno = Factories.Technology.Save();

            var model = new TechnologyEdit
            {
                Id = techno.Id,
                Name = Rand.String(),
                EndOfSupport = Rand.DateTime(future: true),
                TechnologyTypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.TechnologyType).Id,
                ContactId = Factories.Contact.Save().Id
            };

            Controller.Edit(techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Edit(model);
            result.Should().BePartialView("_Details");

            techno.Name.Should().Be(model.Name);
            techno.EndOfSupport.Should().Be(model.EndOfSupport);
            techno.TechnologyType.Id.Should().Be(model.TechnologyTypeId.Value);
            techno.Contact.Id.Should().Be(model.ContactId.Value);
        }

        [Test]
        public void Delete_should_delete()
        {
            var techno = Factories.Technology.Save();
            Controller.Delete(techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Delete(techno.Id);
            result.Should().BeHttpStatusCodeResult(HttpStatusCode.OK);

            NHSession.Get<Technology>(techno.Id).Should().BeNull();
        }

        [Test]
        public void Delete_should_not_delete_if_linked_entities()
        {
            var techno = Factories.Technology.Save();
            var app = Factories.Application.Save();
            app.AddTechnologyLink(techno);

            var result = Controller.Delete(techno.Id);
            result.Should().BeHttpStatusCodeResult(HttpStatusCode.NotAcceptable);
        }
    }
}
