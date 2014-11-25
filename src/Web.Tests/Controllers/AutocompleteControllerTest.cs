// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutocompleteControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using CGI.Reflex.Web.Controllers;

using FluentAssertions;

using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class AutocompleteControllerTest : BaseControllerTest<AutocompleteController>
    {
        [Test]
        public void ApplicationNames_should_return_application_names_based_on_parameter()
        {
            var application1 = Factories.Application.Save();
            Factories.Application.Save();

            var result = Controller.ApplicationNames(application1.Name.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { application1.Name });
        }

        [Test]
        public void ApplicationCodes_should_return_application_codes_based_on_parameter()
        {
            var application1 = Factories.Application.Save();
            Factories.Application.Save();

            var result = Controller.ApplicationCodes(application1.Code.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { application1.Code });
        }

        [Test]
        public void ApplicationCodeAndNames_should_return_application_codes_and_names_based_on_parameter()
        {
            var application1 = Factories.Application.Save();
            Factories.Application.Save();

            var result = Controller.ApplicationNamesAndCodes(application1.Name.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { application1.Name });

            result = Controller.ApplicationNamesAndCodes(application1.Code.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { application1.Code });
        }

        [Test]
        public void EventSources_should_return_event_sources_based_on_parameter()
        {
            var event1 = Factories.Event.Save();
            Factories.Event.Save();

            var result = Controller.EventSources(event1.Source.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { event1.Source });
        }

        [Test]
        public void ServerNames_should_return_server_names_based_on_parameter()
        {
            var server1 = Factories.Server.Save();
            Factories.Server.Save();

            var result = Controller.ServerNames(server1.Name.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { server1.Name });
        }

        [Test]
        public void ServerAliases_should_return_server_aliases_based_on_parameter()
        {
            var server1 = Factories.Server.Save();
            Factories.Server.Save();

            var result = Controller.ServerAliases(server1.Alias.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { server1.Alias });
        }

        [Test]
        public void ServerNamesAndAliases_should_return_server_names_and_aliases_based_on_parameter()
        {
            var server1 = Factories.Server.Save();
            Factories.Server.Save();

            var result = Controller.ServerNamesAndAliases(server1.Name.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { server1.Name });

            result = Controller.ServerNamesAndAliases(server1.Alias.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { server1.Alias });
        }

        [Test]
        public void TechnologyNames_should_return_technology_names_based_on_parameter()
        {
            var techno1 = Factories.Technology.Save();
            Factories.Technology.Save();

            var result = Controller.TechnologyNames(techno1.Name.Substring(0, 2));
            result.Should().BeJsonResult();
            result.As<JsonResult>()
                  .Data.As<IEnumerable<string>>()
                  .Should()
                  .Contain(new[] { techno1.Name });
        }
    }
}
