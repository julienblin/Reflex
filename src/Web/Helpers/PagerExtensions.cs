// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagerExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class PagerExtensions
    {
        // Parameters that will be excluded when copying route values
        private static readonly string[] ExcludedRouteValues = new[] { @"X-Requested-With", @"_" };

        public static HtmlString Pager<TModel>(this HtmlHelper<TModel> htmlHelper, bool centered = true)
            where TModel : AbstractSearchResultModel
        {
            return Pager(htmlHelper, htmlHelper.ViewData.Model.GetPaginationResults(), centered);
        }

        public static HtmlString Pager<TModel>(this HtmlHelper<TModel> htmlHelper, string ajaxTarget, bool centered = true)
           where TModel : AbstractSearchResultModel
        {
            return Pager(htmlHelper, htmlHelper.ViewData.Model.GetPaginationResults(), ajaxTarget, centered);
        }

        public static HtmlString Pager<TModel>(this HtmlHelper<TModel> htmlHelper, IPaginationResult paginationResult, bool centered = true)
            where TModel : AbstractSearchResultModel
        {
            return Pager(htmlHelper, paginationResult, m => m.Page, null, centered);
        }

        public static HtmlString Pager<TModel>(this HtmlHelper<TModel> htmlHelper, IPaginationResult paginationResult, string ajaxTarget, bool centered = true)
            where TModel : AbstractSearchResultModel
        {
            return Pager(htmlHelper, paginationResult, m => m.Page, ajaxTarget, centered);
        }

        public static HtmlString Pager<TModel, TPageValue>(this HtmlHelper<TModel> htmlHelper, IPaginationResult paginationResult, Expression<Func<TModel, TPageValue>> pageExpression, string ajaxTarget, bool centered = true)
        {
            if (paginationResult.PageCount < 2)
                return null;

            var builder = new TagBuilder("div");
            builder.AddCssClass("pagination");

            if (centered)
                builder.AddCssClass("pagination-centered");

            var ul = new TagBuilder("ul");

            var sb = new StringBuilder();
            if (paginationResult.PageCount < 8)
            {
                for (var i = 1; i <= paginationResult.PageCount; ++i)
                {
                    var liTag = new TagBuilder("li");
                    if (i == paginationResult.CurrentPage)
                        liTag.AddCssClass("active");

                    liTag.InnerHtml = GeneratePageLink(htmlHelper, i, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget);

                    sb.Append(liTag.ToString());
                }
            }
            else
            {
                if (paginationResult.CurrentPage > 1)
                {
                    sb.Append(new TagBuilder("li")
                    {
                        InnerHtml = GeneratePageLink(htmlHelper, "&lt;&lt;", 1, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget)
                    });
                }
                else
                {
                    sb.Append(DisabledLi("&lt;&lt;"));
                }

                if (paginationResult.CurrentPage > 1)
                {
                    sb.Append(new TagBuilder("li")
                    {
                        InnerHtml = GeneratePageLink(htmlHelper, "&lt;", paginationResult.CurrentPage - 1, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget)
                    });
                }
                else
                {
                    sb.Append(DisabledLi("&lt;"));
                }

                var firstPageNumber = 1;
                if (paginationResult.CurrentPage > 2)
                {
                    firstPageNumber = paginationResult.CurrentPage - 1;
                    if (paginationResult.CurrentPage == paginationResult.PageCount)
                        firstPageNumber -= 1;
                }

                var lastPageNumber = paginationResult.PageCount;
                if (paginationResult.CurrentPage < (paginationResult.PageCount - 1))
                {
                    lastPageNumber = paginationResult.CurrentPage + 1;
                    if (paginationResult.CurrentPage == 1)
                        lastPageNumber += 1;
                }

                for (var i = firstPageNumber; i <= lastPageNumber; i++)
                {
                    var liTag = new TagBuilder("li");
                    if (i == paginationResult.CurrentPage)
                    {
                        liTag.AddCssClass("active");
                        liTag.InnerHtml = GeneratePageLink(htmlHelper, string.Format("{0} / {1}", i, paginationResult.PageCount), i, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget);
                    }
                    else
                    {
                        liTag.InnerHtml = GeneratePageLink(htmlHelper, i, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget);
                    }

                    sb.Append(liTag.ToString());
                }

                if (paginationResult.CurrentPage < paginationResult.PageCount)
                {
                    sb.Append(new TagBuilder("li")
                    {
                        InnerHtml = GeneratePageLink(htmlHelper, "&gt;", paginationResult.CurrentPage + 1, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget)
                    });
                }
                else
                {
                    sb.Append(DisabledLi("&gt;"));
                }

                if (paginationResult.CurrentPage < paginationResult.PageCount)
                {
                    sb.Append(new TagBuilder("li")
                    {
                        InnerHtml = GeneratePageLink(htmlHelper, "&gt;&gt;", paginationResult.PageCount, ExpressionHelper.GetPropertyName(pageExpression), ajaxTarget)
                    });
                }
                else
                {
                    sb.Append(DisabledLi("&gt;&gt;"));
                }
            }

            ul.InnerHtml = sb.ToString();
            builder.InnerHtml = ul.ToString();

            return new HtmlString(builder.ToString());
        }

        public static string DisabledLi(string linkText)
        {
            var tag = new TagBuilder("li")
            {
                InnerHtml = string.Format("<a href='#'>{0}</a>", linkText)
            };
            tag.AddCssClass("disabled");
            return tag.ToString();
        }

        private static string GeneratePageLink(HtmlHelper htmlHelper, int pageNumber, string pagePropertyName, string ajaxTarget)
        {
            return GeneratePageLink(htmlHelper, pageNumber.ToString(CultureInfo.InvariantCulture), pageNumber, pagePropertyName, ajaxTarget);
        }

        private static string GeneratePageLink(HtmlHelper htmlHelper, string linkText, int pageNumber, string pagePropertyName, string ajaxTarget)
        {
            var uriBuilder = new UriBuilder(HttpContext.Current.Request.Url);
            var queryStringValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryStringValues[pagePropertyName] = pageNumber.ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = queryStringValues.ToString();

            return string.Format(
                "<a href=\"{0}\" {1}>{2}</a>",
                uriBuilder,
                string.IsNullOrEmpty(ajaxTarget) ? string.Empty : "data-ajax-target='" + ajaxTarget + "'",
                linkText);
        }
    }
}