// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexWebViewPage.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Views
{
    [ExcludeFromCodeCoverage]
    public abstract class ReflexWebViewPage : WebViewPage
    {
        public string CurrentArea
        {
            get
            {
                return (string)ViewContext.RouteData.DataTokens["area"];
            }
        }

        public bool IsAllowed(string operation)
        {
            if (References.CurrentUser == null)
                return false;

            return References.CurrentUser.IsAllowed(operation);
        }

        public bool IsAllowed(IEnumerable<string> operations, IsAllowedOperator @operator = IsAllowedOperator.And)
        {
            if (References.CurrentUser == null)
                return false;

            var authorized = @operator == IsAllowedOperator.And;
            foreach (var op in operations)
            {
                switch (@operator)
                {
                    case IsAllowedOperator.And:
                        authorized = authorized && References.CurrentUser.IsAllowed(op);
                        break;
                    case IsAllowedOperator.Or:
                        authorized = authorized || References.CurrentUser.IsAllowed(op);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return authorized;
        }

        public bool IsActionAllowed(string actionName)
        {
            return IsActionAllowed(actionName, null);
        }

        public bool IsActionAllowed(string actionName, string controllerName)
        {
            var controllerBase = string.IsNullOrEmpty(controllerName)
                                     ? Html.ViewContext.Controller
                                     : GetControllerByName(controllerName);
            var controllerContext = new ControllerContext(Html.ViewContext.RequestContext, controllerBase);
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);
            if (actionDescriptor == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find action {0} for controller {1}.",
                        actionName,
                        controllerDescriptor.ControllerName));

            var isAllowedAttribute =
                actionDescriptor.GetCustomAttributes(typeof(IsAllowedAttribute), true).Cast<IsAllowedAttribute>().FirstOrDefault();
            return isAllowedAttribute == null || IsAllowed(isAllowedAttribute.Operations, isAllowedAttribute.Operator);
        }

        private ControllerBase GetControllerByName(string controllerName)
        {
            var factory = ControllerBuilder.Current.GetControllerFactory();
            var controller = factory.CreateController(Html.ViewContext.RequestContext, controllerName);
            if (controller == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The IControllerFactory '{0}' did not return a controller for the name '{1}'.",
                        factory.GetType(),
                        controllerName));
            return (ControllerBase)controller;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [ExcludeFromCodeCoverage]
    public abstract class ReflexWebViewPage<T> : WebViewPage<T>
    {
        public string CurrentArea
        {
            get { return (string)ViewContext.RouteData.DataTokens["area"]; }
        }

        public bool IsAllowed(string operation)
        {
            if (References.CurrentUser == null)
                return false;

            return References.CurrentUser.IsAllowed(operation);
        }

        public bool IsAllowed(IEnumerable<string> operations, IsAllowedOperator @operator = IsAllowedOperator.And)
        {
            if (References.CurrentUser == null)
                return false;

            var authorized = @operator == IsAllowedOperator.And;
            foreach (var op in operations)
            {
                switch (@operator)
                {
                    case IsAllowedOperator.And:
                        authorized = authorized && References.CurrentUser.IsAllowed(op);
                        break;
                    case IsAllowedOperator.Or:
                        authorized = authorized || References.CurrentUser.IsAllowed(op);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return authorized;
        }

        public bool IsActionAllowed(string actionName)
        {
            return IsActionAllowed(actionName, (string)null);
        }

        public bool IsActionAllowed(string actionName, string controllerName)
        {
            return IsActionAllowed(actionName, controllerName, null);
        }

        public bool IsActionAllowed(string actionName, Type controllerType)
        {
            return IsActionAllowed(
                actionName,
                controllerType.Name.Replace("Controller", string.Empty),
                controllerType.Namespace);
        }

        public bool IsActionAllowed(string actionName, string controllerName, string @namespace)
        {
            var controllerBase = string.IsNullOrEmpty(controllerName)
                                     ? Html.ViewContext.Controller
                                     : GetControllerByName(controllerName, @namespace);
            var controllerContext = new ControllerContext(Html.ViewContext.RequestContext, controllerBase);
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());

            var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName)
                                   ?? controllerDescriptor.GetCanonicalActions().FirstOrDefault(x => x.ActionName.Equals(actionName, StringComparison.InvariantCulture))
                                   ?? controllerDescriptor.GetCanonicalActions().FirstOrDefault(x => x.ActionName.Replace("Async", string.Empty).Equals(actionName, StringComparison.InvariantCulture));

            if (actionDescriptor == null)
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to find action {0} for controller {1}.",
                        actionName,
                        controllerDescriptor.ControllerName));

            var isAllowedAttribute =
                actionDescriptor.GetCustomAttributes(typeof(IsAllowedAttribute), true).Cast<IsAllowedAttribute>().FirstOrDefault();
            return isAllowedAttribute == null || IsAllowed(isAllowedAttribute.Operations, isAllowedAttribute.Operator);
        }

        private ControllerBase GetControllerByName(string controllerName, string @namespace)
        {
            var factory = ControllerBuilder.Current.GetControllerFactory();

            var previousNamespaces = Html.ViewContext.RequestContext.RouteData.DataTokens["namespaces"];
            if (!string.IsNullOrEmpty(@namespace))
                Html.ViewContext.RequestContext.RouteData.DataTokens["namespaces"] = new[] { @namespace };

            var controller = factory.CreateController(Html.ViewContext.RequestContext, controllerName);
            if (controller == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The IControllerFactory '{0}' did not return a controller for the name '{1}'.",
                        factory.GetType(),
                        controllerName));

            if (previousNamespaces != null)
                Html.ViewContext.RequestContext.RouteData.DataTokens["namespaces"] = previousNamespaces;

            return (ControllerBase)controller;
        }
    }
}