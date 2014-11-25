// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using MarkdownSharp;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class TextExtensions
    {
        private static readonly Markdown Markdown = new Markdown(new MarkdownOptions { AutoNewLines = true, AutoHyperlink = true, LinkEmails = true });

        public static HtmlString FormattedText(this HtmlHelper htmlHelper, string value)
        {
            return new HtmlString(Markdown.Transform(value));
        }

        public static HtmlString MultilineText(this HtmlHelper htmlHelper, string value)
        {
            var builder = new StringBuilder();
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (var i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    builder.Append("<br/>");
                builder.Append(htmlHelper.Encode(lines[i]));
            }

            return new HtmlString(builder.ToString());
        }
    }
}