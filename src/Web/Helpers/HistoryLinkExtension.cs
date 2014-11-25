// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryLinkExtension.cs" company="CGI">
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
    public static class HistoryLinkExtension
    {
        public static HtmlString HistoryLink(this HtmlHelper htmlHelper, string entityName, int id)
        {
            return htmlHelper.HistoryLink(entityName, id, null);
        }

        public static HtmlString HistoryLink(this HtmlHelper htmlHelper, string entityName, int id, string btnClass)
        {
            return htmlHelper.HistoryLink(entityName, id, btnClass, null);
        }

        public static HtmlString HistoryLink(this HtmlHelper htmlHelper, string entityName, int id, string btnClass, string tooltip)
        {
            return htmlHelper.HistoryLink(entityName, id, btnClass, tooltip, null, null);
        }

        public static HtmlString HistoryLink(this HtmlHelper htmlHelper, string entityName, int id, string btnClass, string tooltip, string returnUrl)
        {
            return htmlHelper.HistoryLink(entityName, id, btnClass, tooltip, null, returnUrl);
        }

        public static HtmlString HistoryLinkProperty(this HtmlHelper htmlHelper, string entityName, int id, string propertyName)
        {
            return htmlHelper.HistoryLinkProperty(entityName, id, null, propertyName);
        }

        public static HtmlString HistoryLinkProperty(this HtmlHelper htmlHelper, string entityName, int id, string btnClass, string propertyName)
        {
            return htmlHelper.HistoryLinkProperty(entityName, id, btnClass, null, propertyName);
        }

        public static HtmlString HistoryLinkProperty(this HtmlHelper htmlHelper, string entityName, int id, string btnClass, string tooltip, string propertyName)
        {
            return htmlHelper.HistoryLink(entityName, id, btnClass, tooltip, propertyName, null);
        }

        public static HtmlString HistoryLink(this HtmlHelper htmlHelper, string entityName, int id, string btnClass, string tooltip, string propertyName, string returnUrl)
        {
            if (References.CurrentUser == null || !References.CurrentUser.IsAllowed("/History"))
                return null;

            var routeDataValues = htmlHelper.ViewContext.RequestContext.RouteData.Values;

            routeDataValues.Remove("X-Requested-With");

            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(htmlHelper.ViewContext.RequestContext, routeDataValues);

            var tag = new TagBuilder("a");
            tag.Attributes.Add("href", UrlHelper.GenerateUrl("History", null, null, new RouteValueDictionary(new { entity = entityName, id, propertyName, returnUrl = returnUrl ?? virtualPathForArea.VirtualPath }), htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, false));
            tag.Attributes.Add("class", string.IsNullOrEmpty(btnClass) ? "btn" : string.Concat("btn ", btnClass));

            if (!string.IsNullOrEmpty(tooltip))
            {
                tag.Attributes.Add("rel", "tooltip");
                tag.Attributes.Add("title", tooltip);
            }

            var icon = new TagBuilder("i");
            icon.Attributes.Add("class", "icon-time");

            tag.InnerHtml = icon.ToString(TagRenderMode.Normal);

            return new HtmlString(tag.ToString(TagRenderMode.Normal));
        }
    }
}