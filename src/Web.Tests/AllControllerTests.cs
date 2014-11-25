// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllControllerTests.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models;

using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests
{
    [TestFixture]
    public class AllControllerTests : BaseControllerTest
    {
        [Test]
        public void All_controllers_should_derive_from_BaseController()
        {
            var controllerTypes = typeof(HomeController).Assembly.GetTypes()
                                                        .Where(t => typeof(Controller).IsAssignableFrom(t)
                                                                 && !t.IsAbstract)
                                                        .ToList();
            var controllersNotBaseController = controllerTypes.Where(t => !typeof(BaseController).IsAssignableFrom(t)).ToList();

            controllersNotBaseController.Should().HaveCount(0, string.Format("The following controllers does not derived from BaseController: {0}", string.Join(",", controllersNotBaseController)));
        }

        [Test]
        public void All_actions_should_be_marked_with_IsAllowed()
        {
            var controllerTypes = typeof(HomeController).Assembly.GetTypes()
                                                        .Where(t => typeof(Controller).IsAssignableFrom(t)
                                                                 && !t.IsAbstract
                                                                 && t != typeof(UserSessionsController)
                                                                 && t != typeof(ErrorController))
                                                        .ToList();
            foreach (var controllerType in controllerTypes)
            {
                var actions = controllerType.GetMethods().Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType));
                var actionsWithoutIsAllowed = actions.Where(a => !a.GetCustomAttributes(true).Any(ca => ca is IsAllowedAttribute));
                actionsWithoutIsAllowed.Should().HaveCount(0, string.Format("The following actions in controller {0} does not have IsAllowed attributes {1}", controllerType, string.Join(",", actionsWithoutIsAllowed)));
            }
        }

        [Test]
        public void All_controller_methods_that_uses_an_AbstractSearchResultModel_should_specify_a_default_order()
        {
            var abstractSearchResultModelType = typeof(AbstractSearchResultModel);
            var controllerTypes = typeof(HomeController).Assembly.GetTypes()
                                                        .Where(t => typeof(Controller).IsAssignableFrom(t)
                                                                 && !t.IsAbstract
                                                                 && t.GetMethods().Any(m => m.GetParameters().Length == 1 && abstractSearchResultModelType.IsAssignableFrom(m.GetParameters()[0].ParameterType)))
                                                        .ToList();
            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType
                              .GetMethods()
                              .Where(
                                  m =>
                                  m.GetParameters().Length == 1
                                  && abstractSearchResultModelType.IsAssignableFrom(m.GetParameters()[0].ParameterType))
                              .ToList();
                foreach (var method in methods)
                {
                    try
                    {
                        var controller = (Controller)Activator.CreateInstance(controllerType);
                        StubRequest(controller, new WebHeaderCollection());
                        var result = (ActionResult)method.Invoke(controller, new[] { Activator.CreateInstance(method.GetParameters()[0].ParameterType) });
                        var viewResult = result as ViewResultBase;
                        if (viewResult != null)
                        {
                            var model = viewResult.Model as AbstractSearchResultModel;
                            if (model != null)
                            {
                                model.OrderBy.Should().NotBeNullOrEmpty(string.Format("Method {0}.{1} should specify a default order,", controllerType, method.Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Method {0}.{1} could not be tested for default order. Reason: {2}", controllerType, method.Name, ex);
                    }
                }
            }
        }
    }
}
