// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleTest.cs" company="CGI">
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
    public class RoleTest : BaseDbTest
    {
        public static IEnumerable<TestCaseData> IsAllowedCases
        {
            get
            {
                yield return new TestCaseData(false, "/Foo", null, null);
                yield return new TestCaseData(true, "/Foo", new[] { "/*" }, null);
                yield return new TestCaseData(true, "/Foo/Bar", new[] { "/*" }, new[] { "/Bar" });
                yield return new TestCaseData(false, "/Foo/Bar", new[] { "/*" }, new[] { "/Foo/Bar" });
                yield return new TestCaseData(true, "/Foo/Bar", new[] { "/*" }, new[] { "/Foo/Bar/SomethingElse" });
                yield return new TestCaseData(true, "/Foo", new[] { "/Foo" }, new[] { "/*" });
                yield return new TestCaseData(true, "/Foo", new[] { "/Foo/Bar" }, new[] { "/*" });
                yield return new TestCaseData(false, "/Foo", new[] { "/Foo/Bar" }, new[] { "/*", "/Foo" });

                yield return new TestCaseData(true, "/Server", new[] { "/Applications", "/Server/*" }, new[] { "/Server/Edit", "/Configuration/*" });
                yield return new TestCaseData(true, "/Server/Something", new[] { "/Applications", "/Server/*" }, new[] { "/Server/Edit", "/Configuration/*" });
                yield return new TestCaseData(false, "/Server/Edit", new[] { "/Applications", "/Server/*" }, new[] { "/Server/Edit", "/Configuration/*" });
                yield return new TestCaseData(false, "/Configuration/Something", new[] { "/Applications", "/Server/*" }, new[] { "/Server/Edit", "/Configuration/*" });
                yield return new TestCaseData(true, "/Configuration/Misc", new[] { "/Applications", "/Server/*", "/Configuration/Misc" }, new[] { "/Server/Edit", "/Configuration/*" });
            }
        }

        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Role>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(10))
                .CheckProperty(x => x.Description, Rand.String(255))
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_persist_operations()
        {
            var role = Factories.Role.Save();

            role.SetAllowedOperations(new[] { "/Applications", "/Servers" });
            role.SetDeniedOperations(new[] { "/Configuration", "/Organization" });
            NHSession.Flush();
            NHSession.Clear();

            role = NHSession.Get<Role>(role.Id);
            role.AllowedOperations.Should().OnlyContain(s => s == "/Applications" || s == "/Servers");
            role.DeniedOperations.Should().OnlyContain(s => s == "/Configuration" || s == "/Organization");
        }

        [Test]
        [TestCaseSource("IsAllowedCases")]
        public void It_should_check_is_allowed(bool expected, string operation, IEnumerable<string> allowedOperations, IEnumerable<string> deniedOperations)
        {
            var role = new Role();
            role.SetAllowedOperations(allowedOperations);
            role.SetDeniedOperations(deniedOperations);

            role.IsAllowed(operation).Should().Be(expected, string.Format("Operation {0} should be {1}.", operation, expected ? "allowed" : "denied"));
        }
    }
}
