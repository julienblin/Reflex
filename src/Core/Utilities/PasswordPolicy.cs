// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordPolicy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CGI.Reflex.Core.Utilities
{
    public static class PasswordPolicy
    {
        private static readonly Regex ReValidate = new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static bool Validate(string clearPassword)
        {
            if (string.IsNullOrEmpty(clearPassword))
                return false;

            return ReValidate.IsMatch(clearPassword);
        }
    }
}
