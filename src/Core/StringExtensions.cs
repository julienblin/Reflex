// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core
{
    public static class StringExtensions
    {
        public static string TruncateWords(this string value, int length, string elipse = "...")
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length, StringComparison.Ordinal) == -1)
                return value;

            if (length > elipse.Length)
                length = length - elipse.Length;

            var nextSpace = value.LastIndexOf(" ", length, StringComparison.Ordinal);
            return string.Concat(value.Substring(0, value.IndexOf(" ", (nextSpace > 0) ? nextSpace : length, StringComparison.Ordinal)), elipse);
        }

        public static string MaxLength(this string value, int max)
        {
            if (value == null)
                return null;

            return value.Length > max ? value.Substring(0, max) : value;
        }
    }
}
