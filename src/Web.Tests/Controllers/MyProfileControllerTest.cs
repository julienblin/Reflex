// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyProfileControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Models.MyProfile;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class MyProfileControllerTest : BaseControllerTest<MyProfileController>
    {
        [Test]
        public void Index_should_return_default_view_with_current_user()
        {
            var user = Factories.User.Save();
            References.ReferencesConfiguration.CurrentUserCallback = () => user;

            var result = Controller.Index();

            result.Should().BeDefaultView();
            result.Model<MyProfileEdit>().Id.Should().Be(user.Id);
        }

        [Test]
        public void Edit_should_edit_current_user()
        {
            var user = Factories.User.Save();
            References.ReferencesConfiguration.CurrentUserCallback = () => user;

            var userAuth = new UserAuthentication { User = user };
            NHSession.Save(userAuth);

            const string Password = "Abcdefgh1";

            var model = new MyProfileEdit
            {
                UserName = Rand.String(),
                Email = Rand.Email(),
                Password = Password,
                PasswordConfirm = Password
            };

            var result = Controller.Edit(model);
            result.Should().BeRedirectToActionName("Index", "Home");
            NHSession.Flush();

            NHSession.Refresh(user);
            user.UserName.Should().Be(model.UserName);
            user.Email.Should().Be(model.Email);

            NHSession.Refresh(userAuth);
            userAuth.VerifyPassword(Password).Should().BeTrue();
        }

        [Test]
        public void Edit_should_check_password_policy()
        {
            var model = new MyProfileEdit
            {
                UserName = Rand.String(),
                Email = Rand.Email(),
                Password = "a",
                PasswordConfirm = "aa"
            };

            model.Validate(null).Should().HaveCount(2);
        }
    }
}
