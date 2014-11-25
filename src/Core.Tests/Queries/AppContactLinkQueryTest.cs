// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContactLinkQueryTest.cs" company="CGI">
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
    public class AppContactLinkQueryTest : BaseDbTest
    {
        [Test]
        public void It_Should_Work_With_No_Entity()
        {
            var app = Factories.Application.Save();
            new AppContactLinkQuery() { ApplicationId = app.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_only_return_contact_for_current_Application()
        {
            var app1 = Factories.Application.Save();
            var app2 = Factories.Application.Save();
            var app3 = Factories.Application.Save();
            var app4 = Factories.Application.Save();

            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save();
            var contact3 = Factories.Contact.Save();
            var contact4 = Factories.Contact.Save();

            app1.AddContactLink(contact1);
            app1.AddContactLink(contact2);

            app2.AddContactLink(contact3);

            app3.AddContactLink(contact1);
            app3.AddContactLink(contact2);
            app3.AddContactLink(contact3);
            app3.AddContactLink(contact4);

            new AppContactLinkQuery() { ApplicationId = app1.Id }.Count().Should().Be(2);
            new AppContactLinkQuery() { ApplicationId = app2.Id }.Count().Should().Be(1);
            new AppContactLinkQuery() { ApplicationId = app3.Id }.Count().Should().Be(4);
            new AppContactLinkQuery() { ApplicationId = app4.Id }.Count().Should().Be(0);
        }
    }
}
