// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorsExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Web.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ColorsExtensions
    {
        public static HtmlString ColorButton(this HtmlHelper htmlHelper, Color color, string refColor)
        {
            return ColorButton(htmlHelper, color, color.Equals(ColorTranslator.FromHtml(refColor)));
        }

        public static HtmlString ColorButton(this HtmlHelper htmlHelper, Color color, bool active = false)
        {
            return new HtmlString(string.Format("<button class='btn-color btn {0}' style='background-color: {1};' data-color-value='{1}'>&nbsp;</button>", active ? "active" : string.Empty, ColorTranslator.ToHtml(color)));
        }

        public static HtmlString ColorSpan(this HtmlHelper htmlHelper, Color color)
        {
            if (color.IsEmpty)
                return new HtmlString("<span>&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;");
            
            return new HtmlString(string.Format("<span style='background-color: {0};'>&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;", ColorTranslator.ToHtml(color)));
        }
    }
}