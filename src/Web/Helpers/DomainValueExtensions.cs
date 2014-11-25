// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class DomainValueExtensions
    {
        public static HtmlString DomainValuesListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string descriptionTarget, bool allowMultiples = false, IDictionary<string, object> htmlAttributes = null)
        {
            var containerType = htmlHelper.ViewData.ModelMetadata.ContainerType;
            var property = containerType.GetProperty(htmlHelper.ViewData.ModelMetadata.PropertyName);
            var attribute = property.GetCustomAttributes(typeof(DomainValueAttribute), true).Cast<DomainValueAttribute>().FirstOrDefault();

            if (attribute == null)
                throw new Exception(string.Format("Unable to generate DomainValuesList for {0}: no DomainValueAttribute was found.", htmlHelper.ViewData.ModelMetadata.PropertyName));

            var domainValues = new List<DomainValue>(new DomainValueQuery { Category = attribute.Category }.OrderBy(dvc => dvc.DisplayOrder).Cacheable().List());

            var dvItemList = new List<DomainValueItem>();

            foreach (var dv in domainValues)
            {
                var dvItem = new DomainValueItem
                {
                    Value = dv.Id.ToString(CultureInfo.InvariantCulture), 
                    Text = dv.Name, 
                    Selected = false, 
                    Description = dv.Description, 
                    Color = dv.Color
                };

                dvItemList.Add(dvItem);
            }

            var optionLabel = string.Empty;
            if (allowMultiples)
            {
                if (htmlAttributes == null)
                    htmlAttributes = new Dictionary<string, object>();

                htmlAttributes["multiple"] = null;
                htmlAttributes["data-select2-enabled"] = null;
                optionLabel = null;
            }

            return htmlHelper.SelectForDomainValue(optionLabel, System.Web.Mvc.ExpressionHelper.GetExpressionText(expression), dvItemList, htmlHelper.ViewData.Model, htmlAttributes, descriptionTarget);
        }

        public static HtmlString DomainValueDisplay(this HtmlHelper htmlHelper, int? value)
        {
            if (!value.HasValue)
                return null;

            var domainValue = References.NHSession.Load<DomainValue>(value.Value);

            return new HtmlString(domainValue.Name);
        }

        // Based on SelectExtensions.SelectInternal located in System.Web.MVC.Html
        private static HtmlString SelectForDomainValue(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<DomainValueItem> selectList, object selectedValue, IDictionary<string, object> htmlAttributes, string descriptionTarget)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrEmpty(fullHtmlFieldName))
                throw new ArgumentException("System.Web.Mvc.Resources.MvcResources.Common_NullOrEmpty", "name");

            bool flag = false;
            if (selectList == null)
            {
                selectList = GetSelectData(htmlHelper, fullHtmlFieldName);
                flag = true;
            }

            object obj = selectedValue;
            if (!flag && obj == null)
                obj = htmlHelper.ViewData.Eval(fullHtmlFieldName);
            if (obj != null)
            {
                IEnumerable source;
                if (!(obj is IEnumerable))
                    source = new[] { obj };
                else
                    source = (IEnumerable)obj;

                var hashSet = new HashSet<string>(source.Cast<object>().Select(value => Convert.ToString(value, CultureInfo.CurrentCulture)), StringComparer.OrdinalIgnoreCase);
                var list = new List<DomainValueItem>();
                foreach (DomainValueItem selectListItem in selectList)
                {
                    selectListItem.Selected = selectListItem.Value != null ? hashSet.Contains(selectListItem.Value) : hashSet.Contains(selectListItem.Text);
                    list.Add(selectListItem);
                }

                selectList = list;
            }

            var stringBuilder = new StringBuilder();
            if (optionLabel != null)
            {
                var emptyOption = new DomainValueItem
                {
                    Value = string.Empty, 
                    Text = optionLabel, 
                    Selected = false, 
                    Description = string.Empty
                };
                stringBuilder.AppendLine(DomainValueToOption(emptyOption, !string.IsNullOrEmpty(descriptionTarget)));
            }

            foreach (DomainValueItem option in selectList)
            {
                stringBuilder.AppendLine(DomainValueToOption(option, !string.IsNullOrEmpty(descriptionTarget)));
            }

            var tagBuilder = new TagBuilder("select") { InnerHtml = stringBuilder.ToString() };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullHtmlFieldName, true);
            if (!string.IsNullOrEmpty(descriptionTarget))
                tagBuilder.MergeAttribute("data-description-target", descriptionTarget);

            tagBuilder.GenerateId(fullHtmlFieldName);

            ModelState modelState;

            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));
            return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        // Based on SelectExtensions.ListItemToOption located in System.Web.MVC.Html
        private static string DomainValueToOption(DomainValueItem item, bool showDescription)
        {
            var tagBuilder = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(item.Text) };

            if (item.Value != null)
                tagBuilder.Attributes["value"] = item.Value;
            if (item.Selected)
                tagBuilder.Attributes["selected"] = "selected";
            if (showDescription)
                tagBuilder.Attributes["data-description"] = item.Description;

            return tagBuilder.ToString(TagRenderMode.Normal);
        }

        // Based on SelectExtensions.GetSelectData located in System.Web.MVC.Html
        private static IEnumerable<DomainValueItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            var obj = (object)null;
            if (htmlHelper.ViewData != null)
                obj = htmlHelper.ViewData.Eval(name);
            if (obj == null)
                throw new InvalidOperationException("obj should not be null");

            var enumerable = obj as IEnumerable<DomainValueItem>;
            if (enumerable != null)
                return enumerable;
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "MvcResources.HtmlHelper_WrongSelectDataType"));
        }

        private class DomainValueItem
        {
            public string Value { get; set; }

            public string Text { get; set; }

            public bool Selected { get; set; }

            public string Description { get; set; }

// ReSharper disable UnusedAutoPropertyAccessor.Local
            public Color Color { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
        }
    }
}