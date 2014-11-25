// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace CGI.Reflex.Core.Tests
{
    public static class Rand
    {
        private const string LoremIpsumConst = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static readonly Random Random = new Random();

        public static int Int(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static DateTime DateTime(bool future = false)
        {
            if (future)
                return System.DateTime.Now.AddDays(Int(200));

            return System.DateTime.Now.AddDays(Int(200) * -1);
        }

        public static string String(int length = 8)
        {
            var builder = new StringBuilder(length);
            while (builder.Length < length)
            {
                builder.Append(Path.GetRandomFileName().Replace(".", string.Empty));
            }

            return builder.ToString().Substring(0, length);
        }

        public static string Email()
        {
            return String(10) + "@" + String(5) + ".com";
        }

        public static string LoremIpsum(int length = 2000)
        {
            var builder = new StringBuilder(length);
            while (builder.Length < length)
            {
                builder.AppendLine(LoremIpsumConst);
            }

            return builder.ToString().Substring(0, length);
        }

        public static T Enum<T>()
        {
            var values = System.Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Next(values.Length));
        }

        public static bool Bool()
        {
            return Int(1) == 1 ? true : false;
        }

        public static string Url(bool local = false)
        {
            return string.Format(local ? "/{0}/{1}" : "http://{0}/{1}", String(), String());
        }

        public static Color Color()
        {
            return System.Drawing.Color.FromArgb(Int(255), Int(255), Int(255));
        }
    }
}
