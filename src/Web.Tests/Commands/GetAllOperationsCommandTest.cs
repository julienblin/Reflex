// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAllOperationsCommandTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Commands
{
    [TestFixture]
    public class GetAllOperationsCommandTest : BaseDbTest
    {
        [Test]
        public void It_should_return_operations_without_wildcards()
        {
            var cmd = new GetAllOperationsCommand { AddWildCards = false };
            var result = cmd.Execute();
            result.Count().Should().BeGreaterThan(0);
            result.Any(s => s.Contains(Role.OperationWildCard)).Should().BeFalse();
        }

        [Test]
        public void It_should_return_operations_with_wildcards()
        {
            var cmd = new GetAllOperationsCommand { AddWildCards = true };
            var result = cmd.Execute();
            result.Count().Should().BeGreaterThan(0);
            result.Any(s => s.Contains(Role.OperationWildCard)).Should().BeTrue();
        }

        [Test]
        public void It_should_return_roles_assignements()
        {
            var role1 = Factories.Role.Save();
            var role2 = Factories.Role.Save();

            var cmd = new GetAllOperationsCommand { AddUserRoleAssignement = true };
            var result = cmd.Execute();

            result.Count().Should().BeGreaterThan(0);
            result.Any(s => s.Contains(GetAllOperationsCommand.UserRoleAssignmentPrefix + role1.Name)).Should().BeTrue();
            result.Any(s => s.Contains(GetAllOperationsCommand.UserRoleAssignmentPrefix + role2.Name)).Should().BeTrue();
        }
    }
}
