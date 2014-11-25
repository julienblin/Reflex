// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorPalette.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public static class ColorPalette
    {
        public static readonly Color NeutralGrey = ColorTranslator.FromHtml("#B5B2B9");

        public static readonly Color StrongGreen = ColorTranslator.FromHtml("#68D93E");

        public static readonly Color LightGreen = ColorTranslator.FromHtml("#B5E40C");

        public static readonly Color Yellow = ColorTranslator.FromHtml("#F6E100");

        public static readonly Color Orange = ColorTranslator.FromHtml("#F6A500");

        public static readonly Color DarkOrange = ColorTranslator.FromHtml("#EF6627");

        public static readonly Color Red = ColorTranslator.FromHtml("#E21538");

        public static readonly Color DarkRed = ColorTranslator.FromHtml("#B4002D");

        public static readonly Color Pink = ColorTranslator.FromHtml("#D93F8E");

        public static readonly Color Violet = ColorTranslator.FromHtml("#B63ED9");

        public static readonly Color DarkViolet = ColorTranslator.FromHtml("#513ED9");

        public static readonly Color DarkBlue = ColorTranslator.FromHtml("#3E84D9");

        public static readonly Color LightBlue = ColorTranslator.FromHtml("#3ECDD9");

        public static readonly Color Green = ColorTranslator.FromHtml("#37BE93");

        public static readonly Color DarkGrey = ColorTranslator.FromHtml("#7A7482");

        public static Color[] Palette
        {
            get
            {
                return new[]
                    {
                        NeutralGrey,
                        StrongGreen,
                        LightGreen,
                        Yellow,
                        Orange,
                        DarkOrange,
                        Red,
                        DarkRed,
                        Pink,
                        Violet,
                        DarkViolet,
                        DarkBlue,
                        LightBlue,
                        Green,
                        DarkGrey
                    };
            }
        }
    }
}
