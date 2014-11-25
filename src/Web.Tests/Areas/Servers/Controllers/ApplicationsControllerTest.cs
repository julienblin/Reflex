// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Web.Areas.Servers.Controllers;
using CGI.Reflex.Web.Areas.Servers.Models.Applications;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Servers.Controllers
{
    public class ApplicationsControllerTest : BaseControllerTest<ApplicationsController>
    {
        [Test]
        public void Details_should_return_view()
        {
            var server = Factories.Server.Save();

            StubStandardRequest();
            var result = Controller.Details(server.Id, new ApplicationsIndex());
            result.Should().BeDefaultView();
            result.Model<ApplicationsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Details_should_return_partial_view_when_ajax()
        {
            var server = Factories.Server.Save();

            StubAjaxRequest();
            var result = Controller.Details(server.Id, new ApplicationsIndex());
            result.Should().BePartialView("_List");
            result.Model<ApplicationsIndex>().Should().NotBeNull();
        }
    }
}
