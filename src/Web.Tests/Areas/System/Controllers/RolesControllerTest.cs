// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RolesControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.Roles;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class RolesControllerTest : BaseControllerTest<RolesController>
    {
        [Test]
        public void Index_should_return_view()
        {
            var result = Controller.Index(new RolesIndex());
            result.Should().BeDefaultView();
            result.Model<RolesIndex>().Should().NotBeNull();
            result.Model<RolesIndex>().AllowedOperations.Count().Should().BeGreaterThan(1);
            result.Model<RolesIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Index_should_filter_by_allowed_operations()
        {
            var role1 = Factories.Role.Create();
            role1.SetAllowedOperations(new[] { "/Foo/Bar", "/Reflex/*" });
            NHSession.Save(role1);

            var role2 = Factories.Role.Create();
            role2.SetAllowedOperations(new[] { "/Foo/*" });
            NHSession.Save(role2);

            var result = Controller.Index(new RolesIndex { AllowedOperation = "/Foo/Bar" });
            result.Model<RolesIndex>().SearchDefined.Should().BeTrue();
            result.Model<RolesIndex>().SearchResults.TotalItems.Should().Be(2);
            result.Model<RolesIndex>().SearchResults.Items.Should().OnlyContain(r => new[] { role1, role2 }.Select(x => x.Id).Contains(r.Id));

            result = Controller.Index(new RolesIndex { AllowedOperation = "/Reflex" });
            result.Model<RolesIndex>().SearchDefined.Should().BeTrue();
            result.Model<RolesIndex>().SearchResults.TotalItems.Should().Be(1);
            result.Model<RolesIndex>().SearchResults.Items.Should().OnlyContain(r => new[] { role1 }.Select(x => x.Id).Contains(r.Id));
        }

        [Test]
        public void Create_should_return_view()
        {
            var result = Controller.Create();
            result.Should().BeDefaultView();
            result.Model<RoleEdit>().AvailableFunctions.Should().NotBeNull().And.NotBeEmpty();
            result.Model<RoleEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_Role()
        {
            var roleEdit = new RoleEdit
            {
                Name = Rand.String(),
                Description = Rand.LoremIpsum(255),
                AllowedOperations = new[] { "/Applications", "/Configuration" },
                DeniedOperations = new[] { "/Users", "/Roles" },
            };

            var result = Controller.Create(roleEdit);
            result.Should().BeRedirectToActionName("Index");

            var role = NHSession.QueryOver<Role>().SingleOrDefault();
            role.Should().NotBeNull();
            role.Name.Should().Be(roleEdit.Name);
            role.Description.Should().Be(roleEdit.Description);
            role.AllowedOperations.Should().OnlyContain(s => s == "/Applications" || s == "/Configuration");
            role.DeniedOperations.Should().OnlyContain(s => s == "/Users" || s == "/Roles");
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            // Arrange
            var role = Factories.Role.Save();

            // Act   
            Controller.Edit(role.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Edit(role.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<RoleEdit>().Id.Should().Be(role.Id);
            result.Model<RoleEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_update_Role()
        {
            var role = Factories.Role.Save();
          
            var roleEdit = new RoleEdit
            {
                Id = role.Id,
                Name = Rand.String(),
                Description = Rand.LoremIpsum(255),
                AllowedOperations = new[] { "/Applications", "/Tests" },
                DeniedOperations = new[] { "/Categories", "/Artistes" },
                ConcurrencyVersion = role.ConcurrencyVersion
            };

            Controller.Edit(new RoleEdit { Id = role.Id + 1 }).Should().BeHttpNotFound(); 
            StubStandardRequest();
            var result = Controller.Edit(roleEdit);
            result.Should().BeRedirectToActionName("Index");

            role.Name.Should().Be(roleEdit.Name);
            role.Description.Should().Be(roleEdit.Description);
            role.AllowedOperations.Should().OnlyContain(s => s == "/Applications" || s == "/Tests");
            role.DeniedOperations.Should().OnlyContain(s => s == "/Categories" || s == "/Artistes");
        }

        [Test]
        public void Edit_should_not_update_Role_concurrency_version_doesnt_match()
        {
            var role = Factories.Role.Save();
           
            var roleEdit = new RoleEdit
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                AllowedOperations = new[] { "/Applications", "/Tests" },
                DeniedOperations = new[] { "/Categories", "/Artistes" },
                ConcurrencyVersion = role.ConcurrencyVersion - 1
            };

            StubStandardRequest();
            var result = Controller.Edit(roleEdit);
            result.Should().BeDefaultView();

            role.AllowedOperations.Should().NotContain("/Applications");
            role.DeniedOperations.Should().NotContain("/Categories");
        }

        [Test]
        public void Delete_Popup()
        {
            var role = Factories.Role.Save();

            Controller.Delete(role.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(role.Id);

            result.Should().BePartialView("_DeleteModal");
            NHSession.Get<Role>(role.Id).Should().NotBeNull();
        }

        [Test]
        public void Delete_Submit()
        {
            var role = Factories.Role.Save();

            Controller.Delete(new RoleEdit { Id = role.Id + 1 }).Should().BeHttpNotFound();
            var result = Controller.Delete(new RoleEdit { Id = role.Id });

            result.Should().BeRedirectToActionName("Index");
            NHSession.Get<Role>(role.Id).Should().BeNull();
        }
    }
}
