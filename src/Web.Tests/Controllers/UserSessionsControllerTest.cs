// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSessionsControllerTest.cs" company="CGI">
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
using CGI.Reflex.Web.Models.UserSessions;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class UserSessionsControllerTest : BaseControllerTest<UserSessionsController>
    {
        [Test]
        public void Login_should_return_view()
        {
            var returnUrl = Rand.Url(local: true);

            var result = Controller.Login(returnUrl);

            result.Should().BeDefaultView();
            result.Model<Login>().ReturnUrl.Should().Be(returnUrl);
        }
        
        [Test]
        public void Boarding_should_return_view_when_token_valid()
        {
            var userAuth = new UserAuthentication { User = Factories.User.Save() };
            NHSession.Save(userAuth);

            var sat = userAuth.GenerateSingleAccessToken(TimeSpan.FromDays(2));
            NHSession.Flush();

            var result = Controller.Boarding(sat);
            result.Should().BeDefaultView();
            result.Model<Boarding>().SingleAccessToken.Should().Be(sat);
            result.Model<Boarding>().UserName.Should().Be(userAuth.User.UserName);
        }

        [Test]
        public void Boarding_should_return_notfound_when_token_doesnt_exist()
        {
            Controller.Boarding(Rand.String()).Should().BeHttpNotFound();
        }

        [Test]
        public void Boarding_should_return_perished_when_token_is_perished_or_user_locked_out()
        {
            var userAuth = new UserAuthentication { User = Factories.User.Save() };
            NHSession.Save(userAuth);

            var sat = userAuth.GenerateSingleAccessToken(Rand.DateTime(future: false));
            NHSession.Flush();

            Controller.Boarding(sat).Should().BeView("BoardingPerished");

            userAuth.User.IsLockedOut = true;
            sat = userAuth.GenerateSingleAccessToken(TimeSpan.FromDays(2));
            Controller.Boarding(sat).Should().BeView("BoardingPerished");
        }
        
        [Test]
        public void ResetPassword_should_return_view()
        {
            var result = Controller.ResetPassword();

            result.Should().BeDefaultView();
        }
    }
}
