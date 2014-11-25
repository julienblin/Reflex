// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class DisplayNameExtensions
    {
        public static HtmlString PropertyDisplayName(this HtmlHelper htmlHelper, object target, string propertyName)
        {
            return htmlHelper.PropertyDisplayName(target.GetType(), propertyName);
        }

        public static HtmlString PropertyDisplayName(this HtmlHelper htmlHelper, Type targetType, string propertyName)
        {
            var propertyInfo = targetType.GetProperty(propertyName);
            if (propertyInfo == null)
                return new HtmlString(propertyName);

            var displayAttr = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().FirstOrDefault();
            if (displayAttr == null)
                return new HtmlString(propertyName);

            return new HtmlString(displayAttr.GetName());
        }

        public static MvcHtmlString PropertyDisplayNameFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = metaData.DisplayName ?? (metaData.PropertyName ?? System.Web.Mvc.ExpressionHelper.GetExpressionText(expression));
            return MvcHtmlString.Create(value);
        }

        public static HtmlString EnumDisplayName<T>(this HtmlHelper htmlHelper, T value)
        {
            return new HtmlString(EnumDisplayName(value));
        }

        public static string EnumDisplayName<T>(T value)
        {
            var memberInfo = typeof(T).GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo == null)
                return value.ToString();

            var displayAttr = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().FirstOrDefault();
            if (displayAttr == null)
                return value.ToString();

            return displayAttr.GetName();
        }

        public static HtmlString AuditableDisplayName(this HtmlHelper htmlHelper, object entity)
        {
            return htmlHelper.AuditableDisplayName(entity.GetType());
        }

        public static HtmlString AuditableDisplayName(this HtmlHelper htmlHelper, Type entityType)
        {
            var auditableAttr = entityType.GetCustomAttributes(typeof(AuditableAttribute), false).Cast<AuditableAttribute>().FirstOrDefault();
            if (auditableAttr == null)
                return new HtmlString(entityType.Name);

            return new HtmlString(auditableAttr.GetName());
        }
    }
}