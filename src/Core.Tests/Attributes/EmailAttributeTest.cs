// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailAttributeTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;

using FluentAssertions;

using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Attributes
{
    [TestFixture]
    public class EmailAttributeTest
    {
        [TestCase("julien.blin@cgi.com", true)]
        [TestCase("julien.blin+tag@cgi.com", true)]
        [TestCase("f0o@SOMEWHERE.museum", true)]
        [TestCase("foo@[192.169.0.1]", true)]
        [TestCase("foo@SOMEWHERE.museum.com", true)]
        [TestCase("foo", false)]
        [TestCase("foo.@something.com", false)]
        public void It_should_validate_emails(string email, bool expectedResult)
        {
            var test = new TestEmail { Email = email };
            test.IsValid().Should().Be(expectedResult);
        }

        private class TestEmail : BaseEntity
        {
            [Email]
            public string Email { get; set; }
        }
    }
}
