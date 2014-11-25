// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.Users;
using CGI.Reflex.Web.Commands;

using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class UsersControllerTest : BaseControllerTest<UsersController>
    {
        private User CurrentUser { get; set; }

        private Role UserRole { get; set; }

        private Role AssignableRole { get; set; }

        public override void SetUp()
        {
            base.SetUp();
            UserRole = Factories.Role.Save();
            AssignableRole = Factories.Role.Save();
            UserRole.SetAllowedOperations(new[] { GetAllOperationsCommand.UserRoleAssignmentPrefix + AssignableRole.Name });
            NHSession.Save(UserRole);

            CurrentUser = Factories.User.Save(x => x.Role = UserRole);
            References.ReferencesConfiguration.CurrentUserCallback = () => CurrentUser;
        }

        [Test]
        public void Index_should_return_view()
        {
            var otherRole = Factories.Role.Save();
            var result = Controller.Index(new UsersIndex());
            result.Should().BeDefaultView();
            result.Model<UsersIndex>().Should().NotBeNull();
            result.Model<UsersIndex>().AuthorizedRoles.Should().OnlyContain(x => x.Id == AssignableRole.Id);
            result.Model<UsersIndex>().AvailableRoles.Should().HaveCount(NHSession.QueryOver<Role>().RowCount());
            result.Model<UsersIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Create_should_return_view_with_authorized_roles()
        {
            var result = Controller.Create();
            result.Model<UserEdit>().Roles.Should().OnlyContain(x => x.Id == AssignableRole.Id);
            result.Should().BeDefaultView();
            result.Model<UserEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_return_unauthorized_if_no_authorized_roles()
        {
            UserRole.SetAllowedOperations(Enumerable.Empty<string>());
            NHSession.Save(CurrentUser);

            var result = Controller.Create();
            result.Should().BeHttpUnauthorized();
        }

        [Test]
        public void Create_should_create_User()
        {
            var userEdit = new UserEdit
            {
                UserName = Rand.String(),
                Email = Rand.String(),
                Company = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany).Id,
                IsLockedOut = Rand.Bool(),
                RoleId = AssignableRole.Id
            };

            var result = Controller.Create(userEdit);
            result.Should().BeRedirectToActionName("Index");

            var user = new UserQuery { UserName = userEdit.UserName }.SingleOrDefault();
            user.Should().NotBeNull();
            user.UserName.Should().Be(userEdit.UserName);
            user.Email.Should().Be(userEdit.Email);
            user.Company.Should().Be(userEdit.Company.ToDomainValue());
            user.IsLockedOut.Should().Be(userEdit.IsLockedOut);
            user.Role.Id.Should().Be(AssignableRole.Id);
        }

        [Test]
        public void Edit_should_return_model_based_on_id_with_authorized_roles()
        {
            // Arrange
            var user = Factories.User.Save(x => x.Role = AssignableRole);

            // Act   
            var result = Controller.Edit(user.Id);
            result.Model<UserEdit>().Roles.Should().OnlyContain(x => x.Id == AssignableRole.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<UserEdit>().Id.Should().Be(user.Id);
            result.Model<UserEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_return_unauthorized_if_role_not_in_authorized_roles()
        {
            // Arrange
            var user = Factories.User.Save();

            // Act   
            var result = Controller.Edit(user.Id);
            result.Should().BeHttpUnauthorized();
        }

        [Test]
        public void Edit_should_update_User()
        {
            var user = Factories.User.Save();

            var userEdit = new UserEdit
            {
                Id = user.Id,
                UserName = Rand.String(),
                Email = Rand.Email(),
                Company = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany).Id,
                IsLockedOut = Rand.Bool(),
                RoleId = AssignableRole.Id
            };

            var result = Controller.Edit(user.Id, userEdit);
            result.Should().BeRedirectToActionName("Index");

            user.UserName.Should().Be(userEdit.UserName);
            user.Email.Should().Be(userEdit.Email);
            user.Company.Should().Be(userEdit.Company.ToDomainValue());
            user.IsLockedOut.Should().Be(userEdit.IsLockedOut);
            user.Role.Id.Should().Be(AssignableRole.Id);
        }

        [Test]
        public void Edit_should_not_update_User_if_role_not_authorized()
        {
            var user = Factories.User.Save();

            var userEdit = new UserEdit
            {
                Id = user.Id,
                UserName = Rand.String(),
                Email = Rand.Email(),
                Company = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany).Id,
                IsLockedOut = Rand.Bool(),
                RoleId = Factories.Role.Save().Id
            };

            var result = Controller.Edit(user.Id, userEdit);
            result.Should().BeHttpUnauthorized();

            user.UserName.Should().Be(user.UserName);
            user.Email.Should().Be(user.Email);
            user.Role.Id.Should().Be(user.Role.Id);
        }
    }
}
