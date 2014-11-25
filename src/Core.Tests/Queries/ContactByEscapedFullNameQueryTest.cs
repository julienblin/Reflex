// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactByEscapedFullNameQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class ContactByEscapedFullNameQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_result()
        {
            new ContactByEscapedFullNameQuery { EscapedFullName = Rand.String() }.Execute().Should().BeNull();
        }

        [Test]
        public void It_should_execute()
        {
            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save(c => { c.FirstName = Rand.String() + " " + Rand.String(); });

            new ContactByEscapedFullNameQuery { EscapedFullName = contact1.GetEscapedFullName() }.Execute().Should().Be(contact1);
            new ContactByEscapedFullNameQuery { EscapedFullName = contact2.GetEscapedFullName() }.Execute().Should().Be(contact2);
        }
    }
}
