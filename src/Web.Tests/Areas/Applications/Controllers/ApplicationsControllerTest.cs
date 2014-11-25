// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models;
using CGI.Reflex.Web.Areas.Applications.Models.Applications;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class ApplicationsControllerTest : BaseControllerTest<ApplicationsController>
    {
        [Test]
        public void Index_should_return_view()
        {
            StubStandardRequest();
            var result = Controller.Index(new ApplicationsIndex());
            result.Should().BeDefaultView();
            result.Model<ApplicationsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Index_should_return_partial_view_when_ajax()
        {
            StubAjaxRequest();
            var result = Controller.Index(new ApplicationsIndex());
            result.Should().BePartialView("_List");
            result.Model<ApplicationsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Create_should_return_view()
        {
            var result = Controller.Create();
            result.Should().BePartialView("_CreateModal");
            result.Model<AppHeader>().Should().NotBeNull();
        }

        [Test]
        public void Create_should_create_application()
        {
            var appHeader = new AppHeader
            {
                Code = Rand.String(4),
                Name = Rand.String(10),
                ApplicationTypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType).Id
            };

            StubStandardRequest();
            var result = Controller.Create(appHeader);
            result.Should().BeJSRedirect();

            var application = NHSession.QueryOver<Application>().SingleOrDefault();
            application.Should().NotBeNull();
            application.Code.Should().Be(appHeader.Code);
            application.Name.Should().Be(appHeader.Name);
            application.ApplicationType.Id.Should().Be(appHeader.ApplicationTypeId.Value);
        }
    }
}
