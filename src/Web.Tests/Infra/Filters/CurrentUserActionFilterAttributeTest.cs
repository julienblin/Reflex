// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentUserActionFilterAttributeTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Configuration;
using CGI.Reflex.Web.Infra.Filters;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace CGI.Reflex.Web.Tests.Infra.Filters
{
    public class CurrentUserActionFilterAttributeTest : BaseActionFilterTest<CurrentUserActionFilterAttribute>
    {
        [Test]
        public void It_should_load_current_user_based_on_Id_in_Identity_Name_when_AuthenticationMode_Forms()
        {
            var config = ReflexConfigurationSection.GetConfigurationSection();
            config.AuthenticationMode = AuthenticationMode.Forms;
            Filter = new CurrentUserActionFilterAttribute(config);

            var user = Factories.User.Save();

            var identity = Mocks.Stub<IIdentity>();
            SetupResult.For(identity.Name).Return(user.Id.ToString());
            Mocks.Replay(identity);

            var principal = Mocks.Stub<IPrincipal>();
            SetupResult.For(principal.Identity).Return(identity);
            Mocks.Replay(principal);

            var request = Mocks.Stub<HttpRequestBase>();
            SetupResult.For(request.IsAuthenticated).Return(true);
            Mocks.Replay(request);

            var httpContextItems = new Hashtable();

            var context = Mocks.Stub<HttpContextBase>();
            SetupResult.For(context.Request).Return(request);
            SetupResult.For(context.Items).Return(httpContextItems);
            context.User = principal;
            Mocks.Replay(context);

            var controllerContext = Mocks.Stub<ControllerContext>();
            controllerContext.HttpContext = context;
            Mocks.Replay(controllerContext);
            var actionDescriptor = Mocks.Stub<ActionDescriptor>();
            Mocks.Replay(actionDescriptor);

            Filter.OnActionExecuting(new ActionExecutingContext(controllerContext, actionDescriptor, new Dictionary<string, object>()));

            httpContextItems.ContainsKey(MvcApplication.CurrentUserContextKey).Should().BeTrue();
            httpContextItems[MvcApplication.CurrentUserContextKey].Should().Be(user);
            NHSession.IsReadOnly(user).Should().BeTrue();
        }

        [Test]
        public void It_should_load_current_user_based_on_UserName_in_Identity_Name_when_AuthenticationMode_Windows()
        {
            var config = ReflexConfigurationSection.GetConfigurationSection();
            config.AuthenticationMode = AuthenticationMode.Windows;
            Filter = new CurrentUserActionFilterAttribute(config);

            var user = Factories.User.Save();

            var identity = Mocks.Stub<IIdentity>();
            SetupResult.For(identity.Name).Return(user.UserName.ToString());
            Mocks.Replay(identity);

            var principal = Mocks.Stub<IPrincipal>();
            SetupResult.For(principal.Identity).Return(identity);
            Mocks.Replay(principal);

            var request = Mocks.Stub<HttpRequestBase>();
            SetupResult.For(request.IsAuthenticated).Return(true);
            Mocks.Replay(request);

            var httpContextItems = new Hashtable();

            var context = Mocks.Stub<HttpContextBase>();
            SetupResult.For(context.Request).Return(request);
            SetupResult.For(context.Items).Return(httpContextItems);
            context.User = principal;
            Mocks.Replay(context);

            var controllerContext = Mocks.Stub<ControllerContext>();
            controllerContext.HttpContext = context;
            Mocks.Replay(controllerContext);
            var actionDescriptor = Mocks.Stub<ActionDescriptor>();
            Mocks.Replay(actionDescriptor);

            Filter.OnActionExecuting(new ActionExecutingContext(controllerContext, actionDescriptor, new Dictionary<string, object>()));

            httpContextItems.ContainsKey(MvcApplication.CurrentUserContextKey).Should().BeTrue();
            httpContextItems[MvcApplication.CurrentUserContextKey].Should().Be(user);
            NHSession.IsReadOnly(user).Should().BeTrue();
        }

        [Test]
        public void It_should_do_nothing_when_not_authenticated()
        {
            var request = Mocks.Stub<HttpRequestBase>();
            SetupResult.For(request.IsAuthenticated).Return(false);
            Mocks.Replay(request);

            var httpContextItems = new Hashtable();

            var context = Mocks.Stub<HttpContextBase>();
            SetupResult.For(context.Request).Return(request);
            SetupResult.For(context.Items).Return(httpContextItems);
            Mocks.Replay(context);

            var controllerContext = Mocks.Stub<ControllerContext>();
            controllerContext.HttpContext = context;
            Mocks.Replay(controllerContext);
            var actionDescriptor = Mocks.Stub<ActionDescriptor>();
            Mocks.Replay(actionDescriptor);

            Filter.OnActionExecuting(new ActionExecutingContext(controllerContext, actionDescriptor, new Dictionary<string, object>()));
            httpContextItems.ContainsKey(MvcApplication.CurrentUserContextKey).Should().BeFalse();
        }

        [Test]
        [TestCase(AuthenticationMode.None)]
        [TestCase(AuthenticationMode.Forms)]
        [TestCase(AuthenticationMode.Windows)]
        public void It_should_do_nothing_when_not_found(AuthenticationMode authenticationMode)
        {
            var config = ReflexConfigurationSection.GetConfigurationSection();
            config.AuthenticationMode = authenticationMode;
            Filter = new CurrentUserActionFilterAttribute(config);

            var identity = Mocks.Stub<IIdentity>();
            SetupResult.For(identity.Name).Return(Rand.String());
            Mocks.Replay(identity);

            var principal = Mocks.Stub<IPrincipal>();
            SetupResult.For(principal.Identity).Return(identity);
            Mocks.Replay(principal);

            var request = Mocks.Stub<HttpRequestBase>();
            SetupResult.For(request.IsAuthenticated).Return(true);
            Mocks.Replay(request);

            var httpContextItems = new Hashtable();

            var context = Mocks.Stub<HttpContextBase>();
            SetupResult.For(context.Request).Return(request);
            SetupResult.For(context.Items).Return(httpContextItems);
            context.User = principal;
            Mocks.Replay(context);

            var controllerContext = Mocks.Stub<ControllerContext>();
            controllerContext.HttpContext = context;
            Mocks.Replay(controllerContext);
            var actionDescriptor = Mocks.Stub<ActionDescriptor>();
            Mocks.Replay(actionDescriptor);

            Filter.OnActionExecuting(new ActionExecutingContext(controllerContext, actionDescriptor, new Dictionary<string, object>()));
            httpContextItems.ContainsKey(MvcApplication.CurrentUserContextKey).Should().BeFalse();
        }
    }
}
