// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.DomainValues;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class DomainValuesControllerTest : BaseControllerTest<DomainValuesController>
    {
        [Test]
        public void Index_should_work_with_no_value_and_return_view()
        {
            StubStandardRequest();
            var result = Controller.Index(new DomainValuesIndex());
            result.Should().BeDefaultView();
            result.Model<DomainValuesIndex>().Should().NotBeNull();
        }

        [Test]
        public void Index_should_return_partial_view_when_ajax()
        {
            StubAjaxRequest();
            var result = Controller.Index(new DomainValuesIndex());
            result.Should().BePartialView("_List");
            result.Model<DomainValuesIndex>().Should().NotBeNull();
        }

        [Test]
        public void Index_should_return_values_for_current_category()
        {
            var category1 = Rand.Enum<DomainValueCategory>();
            var category2 = Rand.Enum<DomainValueCategory>();
            while (category2 == category1)
                category2 = Rand.Enum<DomainValueCategory>();

            var dv1 = Factories.DomainValue.Save(dv => dv.Category = category1);
            var dv2 = Factories.DomainValue.Save(dv => dv.Category = category1);
            var dv3 = Factories.DomainValue.Save(dv => dv.Category = category2);

            StubStandardRequest();
            var model = Controller.Index(new DomainValuesIndex { Category = category1 }).Model<DomainValuesIndex>();

            model.Should().NotBeNull();
            model.Items.Should().HaveCount(2);
        }

        [Test]
        public void Create_should_return_model_based_on_category()
        {
            var category = Rand.Enum<DomainValueCategory>();
            
            var result = Controller.Create(category);

            result.Should().BeDefaultView();
            result.Model<DomainValueEdit>().Category.Should().Be(category);
            result.Model<DomainValueEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_DomainValue()
        {
            // Arrange
            var domainValueEdit = new DomainValueEdit
            {
                Name = Rand.String(),
                Description = Rand.String(),
                Category = Rand.Enum<DomainValueCategory>()
            };

            // Act 
            var result = Controller.Create(domainValueEdit);

            // Assert
            result.Should().BeRedirectToActionName("Index");
            ((RedirectToRouteResult)result).RouteValues.Should().ContainKey("Category");
            ((RedirectToRouteResult)result).RouteValues["Category"].Should().Be(domainValueEdit.Category);

            var domainValue = NHSession.QueryOver<DomainValue>().SingleOrDefault();
            domainValue.Should().NotBeNull();
            domainValue.Name.Should().Be(domainValueEdit.Name);
            domainValue.Category.Should().Be(domainValueEdit.Category);
            domainValue.Description.Should().Be(domainValueEdit.Description);
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            // Arrange
            var dv = Factories.DomainValue.Save();

            // Act          
            Controller.Edit(dv.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Edit(dv.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<DomainValueEdit>().Id.Should().Be(dv.Id);
            result.Model<DomainValueEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_return_model_based_on_DomaineValue()
        {
            var dv = Factories.DomainValue.Save();

            var domainValueEdit = new DomainValueEdit
            {
                Id = dv.Id,
                Name = Rand.String(),
                Description = Rand.String(),
                Category = Rand.Enum<DomainValueCategory>(),
                ConcurrencyVersion = dv.ConcurrencyVersion
            };

            Controller.Edit(new DomainValueEdit { Id = dv.Id + 1 }).Should().BeHttpNotFound();          
            StubStandardRequest();
            var result = Controller.Edit(domainValueEdit);
           
            dv.Name.Should().Be(domainValueEdit.Name);
            dv.Category.Should().Be(domainValueEdit.Category);
            dv.Description.Should().Be(domainValueEdit.Description);
        }

        [Test]
        public void Edit_should_not_Update_when_concurrency_version_doesnt_match()
        {
            var dv = Factories.DomainValue.Save();
            
            var domainValueEdit = new DomainValueEdit
            {
                Id = dv.Id,
                Name = Rand.String(),
                Description = Rand.String(),
                Category = Rand.Enum<DomainValueCategory>(),
                ConcurrencyVersion = dv.ConcurrencyVersion - 1
             };

            StubStandardRequest();
            var result = Controller.Edit(domainValueEdit);

            dv.Name.Should().NotBe(domainValueEdit.Name);
            dv.Description.Should().NotBe(domainValueEdit.Description);
        }

        [Test]
        public void Delete_Popup()
        {
            var dv = Factories.DomainValue.Save();

            Controller.Delete(dv.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(dv.Id);

            result.Should().BePartialView("_DeleteModal");
            NHSession.Get<DomainValue>(dv.Id).Should().NotBeNull();
        }

        [Test]
        public void Delete_Submit()
        {
            var dv = Factories.DomainValue.Save();

            Controller.Delete(new DomainValueEdit { Id = dv.Id + 1 }).Should().BeHttpNotFound();
            var result = Controller.Delete(new DomainValueEdit { Id = dv.Id });

            result.Should().BeOfType<RedirectToRouteResult>();
            NHSession.Get<DomainValue>(dv.Id).Should().BeNull();
        }

        [Test]
        public void Reorder_should_change_direction_to_up()
        {
            var dvCategory = Rand.Enum<DomainValueCategory>();
            var dv0 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 0; });
            var dv1 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 1; });
            var dv2 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 2; });

            Controller.Reorder(dv2.Id + 1, "up").Should().BeHttpNotFound();
            StubAjaxRequest();

            var result = Controller.Reorder(dv1.Id, "up");
            result.Should().BePartialView("_List");
            var dvResult = new DomainValueQuery { Category = dvCategory }.OrderBy(d => d.DisplayOrder).List().ToList();
            dvResult[0].Id.Should().Be(dv1.Id);
            dvResult[1].Id.Should().Be(dv0.Id);
            dvResult[2].Id.Should().Be(dv2.Id);      
        }

        [Test]
        public void Reorder_should_change_direction_to_down()
        {
            var dvCategory = Rand.Enum<DomainValueCategory>();
            var dv0 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 0; });
            var dv1 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 1; });
            var dv2 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 2; });

            Controller.Reorder(dv2.Id + 1, "down").Should().BeHttpNotFound();
            StubAjaxRequest();

            var result = Controller.Reorder(dv1.Id, "down");
            result.Should().BePartialView("_List");
            var dvResult = new DomainValueQuery { Category = dvCategory }.OrderBy(d => d.DisplayOrder).List().ToList();
            dvResult[0].Id.Should().Be(dv0.Id);
            dvResult[1].Id.Should().Be(dv2.Id);
            dvResult[2].Id.Should().Be(dv1.Id);
        }

        [Test]
        public void Reorder_should_not_change_direction_to_down_when_is_the_last_item()
        {
            var dvCategory = Rand.Enum<DomainValueCategory>();
            var dv0 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 0; });
            var dv1 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 1; });
            var dv2 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 2; });

            Controller.Reorder(dv2.Id + 1, "down").Should().BeHttpNotFound();
            StubAjaxRequest();

            var result = Controller.Reorder(dv2.Id, "down");
            result.Should().BePartialView("_List");
            var dvResult = new DomainValueQuery { Category = dvCategory }.OrderBy(d => d.DisplayOrder).List().ToList();
            dvResult[0].Id.Should().Be(dv0.Id);
            dvResult[1].Id.Should().Be(dv1.Id);
            dvResult[2].Id.Should().Be(dv2.Id);
        }

        [Test]
        public void Reorder_should_not_change_direction_to_up_when_is_the_first_item()
        {
            var dvCategory = Rand.Enum<DomainValueCategory>();
            var dv0 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 0; });
            var dv1 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 1; });
            var dv2 = Factories.DomainValue.Save(d => { d.Category = dvCategory; d.DisplayOrder = 2; });

            Controller.Reorder(dv2.Id + 1, "down").Should().BeHttpNotFound();
            StubAjaxRequest();
            
            var result = Controller.Reorder(dv2.Id, "down");
            result.Should().BePartialView("_List");
            var dvResult = new DomainValueQuery { Category = dvCategory }.OrderBy(d => d.DisplayOrder).List().ToList();
            dvResult[0].Id.Should().Be(dv0.Id);
            dvResult[1].Id.Should().Be(dv1.Id);
            dvResult[2].Id.Should().Be(dv2.Id);
        }
    }
}
