// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class UserTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<User>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.UserName, Rand.String(15))
                .CheckProperty(x => x.Email, Rand.String(40))
                .CheckReference(x => x.Role, Factories.Role.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_allowed()
        {
            var user = Factories.User.Save();

            user.IsAllowed("/*").Should().Be(user.Role.IsAllowed("/*"));
        }
    }
}
