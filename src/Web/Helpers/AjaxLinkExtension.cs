// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AjaxLinkExtension.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class AjaxLinkExtension
    {
        public const string DefaultModalId = "defaultModal";
        public const string LargeModalId = "largeModal";

        public static HtmlString ActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repID, actionName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static HtmlString ActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static HtmlString ModalActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var ajaxOptions = new AjaxOptions { HttpMethod = "Get", UpdateTargetId = DefaultModalId, InsertionMode = InsertionMode.Replace, OnSuccess = string.Format("$('#{0}').modal('show');", DefaultModalId) };
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static HtmlString ModalActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var ajaxOptions = new AjaxOptions { HttpMethod = "Get", UpdateTargetId = DefaultModalId, InsertionMode = InsertionMode.Replace, OnSuccess = string.Format("$('#{0}').modal('show');", DefaultModalId) };
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static HtmlString LargeModalActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var ajaxOptions = new AjaxOptions { HttpMethod = "Get", UpdateTargetId = LargeModalId, InsertionMode = InsertionMode.Replace, OnSuccess = string.Format("$('#{0}').modal('show');", LargeModalId) };
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static HtmlString LargeModalActionLinkRaw(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var ajaxOptions = new AjaxOptions { HttpMethod = "Get", UpdateTargetId = LargeModalId, InsertionMode = InsertionMode.Replace, OnSuccess = string.Format("$('#{0}').modal('show');", LargeModalId) };
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return new HtmlString(lnk.ToString().Replace(repID, linkText));
        }

        public static MvcForm BeginModalForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, object htmlAttributes)
        {
            return ajaxHelper.BeginForm(actionName, controllerName, new AjaxOptions { UpdateTargetId = DefaultModalId, InsertionMode = InsertionMode.Replace, HttpMethod = "Post" }, htmlAttributes);
        }
    }
}