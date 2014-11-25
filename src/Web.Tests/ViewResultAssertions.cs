// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewResultAssertions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Web.Infra.Results;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace CGI.Reflex.Web.Tests
{
    public static class ViewResultAssertions
    {
        public static AndConstraint<ObjectAssertions> BeDefaultView(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)assertions.Subject;
            viewResult.ViewName.Should().BeEmpty();
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeView(this ObjectAssertions assertions, string viewName)
        {
            assertions.Subject.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)assertions.Subject;
            viewResult.ViewName.Should().Be(viewName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BePartialView(this ObjectAssertions assertions, string viewName)
        {
            assertions.Subject.Should().BeOfType<PartialViewResult>();
            var viewResult = (PartialViewResult)assertions.Subject;
            viewResult.ViewName.Should().Be(viewName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeHttpNotFound(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<HttpNotFoundResult>();
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeHttpUnauthorized(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<HttpUnauthorizedResult>();
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeRedirectToRouteName(this ObjectAssertions assertions, string routeName)
        {
            assertions.Subject.Should().BeOfType<RedirectToRouteResult>();
            var routeResult = (RedirectToRouteResult)assertions.Subject;
            routeResult.RouteName.Should().Be(routeName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeRedirectToActionName(this ObjectAssertions assertions, string actionName)
        {
            assertions.Subject.Should().BeOfType<RedirectToRouteResult>();
            var routeResult = (RedirectToRouteResult)assertions.Subject;
            routeResult.RouteValues.Should().ContainKey("action");
            routeResult.RouteValues["action"].Should().Be(actionName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeRedirectToActionName(this ObjectAssertions assertions, string actionName, string controllerName)
        {
            assertions.Subject.Should().BeOfType<RedirectToRouteResult>();
            var routeResult = (RedirectToRouteResult)assertions.Subject;
            routeResult.RouteValues.Should().ContainKey("action");
            routeResult.RouteValues["action"].Should().Be(actionName);
            routeResult.RouteValues["controller"].Should().Be(controllerName);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeChartResult(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<ChartResult>();
            ((ChartResult)assertions.Subject).Chart.Should().NotBeNull();
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeJSRedirect(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<JavaScriptResult>();
            var result = (JavaScriptResult)assertions.Subject;
            result.Script.Should().StartWith("window.top.location.href = ");
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeJsonResult(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<JsonResult>();
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeHttpStatusCodeResult(this ObjectAssertions assertions, HttpStatusCode statusCode)
        {
            assertions.Subject.Should().BeOfType<HttpStatusCodeResult>();
            var httpStatusCodeResult = (HttpStatusCodeResult)assertions.Subject;
            httpStatusCodeResult.StatusCode.Should().Be((int)statusCode);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<ObjectAssertions> BeFileStreamResult(this ObjectAssertions assertions)
        {
            assertions.Subject.Should().BeOfType<FileStreamResult>();
            return new AndConstraint<ObjectAssertions>(assertions);
        }
    }
}
