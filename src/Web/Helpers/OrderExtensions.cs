// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class OrderExtensions
    {
        public static HtmlString OrderLink<TModel>(this HtmlHelper<TModel> htmlHelper, string display, string propertyName)
            where TModel : AbstractSearchResultModel
        {
            return OrderLink(htmlHelper, display, propertyName, m => m.OrderBy, m => m.OrderType);
        }

        public static HtmlString OrderLink<TModel>(this HtmlHelper<TModel> htmlHelper, string display, string propertyName, Expression<Func<TModel, object>> orderExpression, Expression<Func<TModel, object>> orderTypeExpression)
        {
            return OrderLink(htmlHelper, display, propertyName, ExpressionHelper.GetPropertyName(orderExpression), ExpressionHelper.GetPropertyName(orderTypeExpression));
        }

        public static HtmlString OrderLink(this HtmlHelper htmlHelper, string display, string propertyName, string propertyOrderName, string propertyOrderTypeName)
        {
            var uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
            var queryStringValues = HttpUtility.ParseQueryString(uriBuilder.Query);

            queryStringValues[propertyOrderName] = propertyName;

            var modelType = htmlHelper.ViewData.Model.GetType();
            var currentModelOrder = (string)modelType.GetProperty(propertyOrderName).GetValue(htmlHelper.ViewData.Model, new object[0]);
            var currentModelOrderType = (OrderType)modelType.GetProperty(propertyOrderTypeName).GetValue(htmlHelper.ViewData.Model, new object[0]);

            var targetOrder = currentModelOrderType == OrderType.Asc ? OrderType.Desc : OrderType.Asc;
            if (!string.IsNullOrEmpty(currentModelOrder)
                && !currentModelOrder.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                targetOrder = OrderType.Asc;

            queryStringValues[propertyOrderTypeName] = targetOrder.ToString();
            uriBuilder.Query = queryStringValues.ToString();

            var builder = new TagBuilder("a");
            builder.Attributes["href"] = uriBuilder.ToString();

            if (!string.IsNullOrEmpty(currentModelOrder)
                && currentModelOrder.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
            {
                switch (currentModelOrderType)
                {
                    case OrderType.Asc:
                        builder.InnerHtml = string.Format("<i class=\"icon-chevron-up\"></i>&nbsp;{0}", display);
                        break;
                    case OrderType.Desc:
                        builder.InnerHtml = string.Format("<i class=\"icon-chevron-down\"></i>&nbsp;{0}", display);
                        break;
                }
            }
            else
            {
                builder.SetInnerText(display);
            }

            return new HtmlString(builder.ToString());
        }
    }
}