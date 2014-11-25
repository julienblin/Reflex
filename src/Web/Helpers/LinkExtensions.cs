// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class LinkExtensions
    {
        public static HtmlString PassiveAllowedActionLink(this HtmlHelper htmlHelper, string authorisation, string linkText, string actionName, object routeValues)
        {
            if (linkText == null)
                return null;

            if ((References.CurrentUser == null) || !References.CurrentUser.IsAllowed(authorisation))
                return new HtmlString(linkText);

            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, routeValues);
        }

        public static HtmlString PassiveAllowedActionLink(this HtmlHelper htmlHelper, string authorisation, string linkText, string actionName, string controllerName, object routeValues)
        {
            if (linkText == null)
                return null;

            if ((References.CurrentUser == null) || !References.CurrentUser.IsAllowed(authorisation))
                return new HtmlString(linkText);

            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, null);
        }

        public static HtmlString PassiveAllowedActionLink(this HtmlHelper htmlHelper, string authorisation, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            if (linkText == null)
                return null;

            if ((References.CurrentUser == null) || !References.CurrentUser.IsAllowed(authorisation))
                return new HtmlString(linkText);

            return System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, htmlAttributes);
        }
    }
}