// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordPolicyTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Utilities;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Utilities
{
    [TestFixture]
    public class PasswordPolicyTest
    {
        [Test]
        public void It_should_reject_lowercase_only()
        {
            PasswordPolicy.Validate("abcdefgh1").Should().BeFalse();
        }

        [Test]
        public void It_should_reject_uppercase_only()
        {
            PasswordPolicy.Validate("ABCDEFGH1").Should().BeFalse();
        }

        [Test]
        public void It_should_reject_letters_only()
        {
            PasswordPolicy.Validate("AbcDefgh").Should().BeFalse();
        }

        [Test]
        public void It_should_reject_less_than_6_in_length()
        {
            PasswordPolicy.Validate("Abcd1").Should().BeFalse();
        }

        [Test]
        public void It_should_accept_valid_passwords()
        {
            PasswordPolicy.Validate("AbcDefgh1").Should().BeTrue();
        }
    }
}
