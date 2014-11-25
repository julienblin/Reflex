// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAuthenticationTest.cs" company="CGI">
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
    public class UserAuthenticationTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<UserAuthentication>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.LastLoginDate, Rand.DateTime())
                .CheckProperty(x => x.FailedPasswordAttemptCount, Rand.Int(5))
                .CheckReference(x => x.User, Factories.User.Save())
                .VerifyTheMappings();
        }

        [Test]
        public void Password_should_be_stored_as_hash()
        {
            var user = Factories.User.Save();
            var auth = new UserAuthentication { User = user };
            NHSession.Save(auth);
            auth.LastPasswordChangedAt.HasValue.Should().BeFalse();

            var password = Rand.String(10);

            auth.SetPassword(password);
            NHSession.Flush();
            NHSession.Clear();

            auth = NHSession.QueryOver<UserAuthentication>().Where(x => x.User.Id == user.Id).SingleOrDefault();
            auth.Should().NotBeNull();
            auth.VerifyPassword(password).Should().BeTrue();
            auth.LastPasswordChangedAt.Value.Should().BeWithin(TimeSpan.FromMinutes(1)).Before(DateTime.Now);
        }

        [Test]
        public void It_should_generate_single_access_token()
        {
            var user = Factories.User.Save();
            var auth = new UserAuthentication { User = user };
            NHSession.Save(auth);
            var sat = auth.GenerateSingleAccessToken(TimeSpan.FromDays(2));

            NHSession.Flush();
            NHSession.Clear();

            auth = NHSession.QueryOver<UserAuthentication>()
                            .Where(x => x.SingleAccessToken == sat)
                            .And(x => x.SingleAccessTokenValidUntil > DateTime.Now)
                            .SingleOrDefault();
            auth.Should().NotBeNull();
        }

        [Test]
        public void It_should_authenticate()
        {
            var user = Factories.User.Save();
            var auth = new UserAuthentication { User = user, FailedPasswordAttemptCount = 2 };
            var password = Rand.String();
            auth.SetPassword(password);
            NHSession.Save(auth);

            var authenticatedUser = UserAuthentication.Authenticate(user.UserName, password);
            authenticatedUser.User.Should().Be(user);
            authenticatedUser.LastLoginDate.Should().BeWithin(TimeSpan.FromSeconds(1)).Before(DateTime.Now);
            authenticatedUser.FailedPasswordAttemptCount.Should().Be(0);

            authenticatedUser = UserAuthentication.Authenticate(user.Email, password);
            authenticatedUser.User.Should().Be(user);
            authenticatedUser.LastLoginDate.Should().BeWithin(TimeSpan.FromSeconds(1)).Before(DateTime.Now);
            authenticatedUser.FailedPasswordAttemptCount.Should().Be(0);
        }

        [Test]
        public void It_should_not_authenticate_if_not_found()
        {
            UserAuthentication.Authenticate(Rand.String(), Rand.String()).Should().BeNull();
        }

        [Test]
        public void It_should_not_authenticate_if_user_locked_out()
        {
            var user = Factories.User.Save(u => u.IsLockedOut = true);
            var auth = new UserAuthentication { User = user, FailedPasswordAttemptCount = 2 };
            var password = Rand.String();
            auth.SetPassword(password);
            NHSession.Save(auth);

            UserAuthentication.Authenticate(user.UserName, password).Should().BeNull();
        }

        [Test]
        public void It_should_not_authenticate_if_password_empty()
        {
            var user = Factories.User.Save(u => u.IsLockedOut = true);
            var auth = new UserAuthentication { User = user };
            NHSession.Save(auth);

            UserAuthentication.Authenticate(user.UserName, Rand.String()).Should().BeNull();
        }

        [Test]
        public void It_should_not_authenticate_if_password_doesnt_match()
        {
            var user = Factories.User.Save();
            var auth = new UserAuthentication { User = user, FailedPasswordAttemptCount = 2 };
            var password = Rand.String();
            auth.SetPassword(password);
            NHSession.Save(auth);

            UserAuthentication.Authenticate(user.UserName, Rand.String()).Should().BeNull();
            auth.FailedPasswordAttemptCount.Should().Be(1);
        }

        [Test]
        public void It_should_locked_out_users()
        {
            var user = Factories.User.Save();
            var auth = new UserAuthentication { User = user, FailedPasswordAttemptCount = 2 };
            var password = Rand.String();
            auth.SetPassword(password);
            NHSession.Save(auth);

            for (var i = 0; i < UserAuthentication.MaxFailedAttemptPassword + 1; i++)
            {
                UserAuthentication.Authenticate(user.UserName, Rand.String()).Should().BeNull();
            }

            user.IsLockedOut.Should().BeTrue();
        }
    }
}
