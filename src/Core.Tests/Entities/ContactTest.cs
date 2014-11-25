// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests.Factories;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class ContactTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Contact>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.FirstName, Rand.String(30))
                .CheckProperty(x => x.LastName, Rand.String(30))
                .CheckProperty(x => x.Company, Rand.String(30))
                .CheckProperty(x => x.Email, Rand.Email())
                .CheckReference(x => x.Type, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType))
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_return_FullName()
        {
            var contact = Factories.Contact.Create();
            contact.FullName.Should().Be(string.Format("{0}, {1}", contact.LastName, contact.FirstName));

            contact.FirstName = string.Empty;
            contact.FullName.Should().Be(contact.LastName);

            contact.LastName = string.Empty;
            contact.FullName.Should().Be(contact.Company);
        }
    }
}
