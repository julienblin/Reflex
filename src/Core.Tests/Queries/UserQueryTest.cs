// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class UserQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new UserQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_Id()
        {
            var user = Factories.User.Save();

            new UserQuery { Id = user.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_NameOrEmail()
        {
            var name1 = Rand.String();
            var email1 = Rand.Email();
            var name2 = Rand.String();
            var email2 = Rand.Email();

            Factories.User.Save(u => { u.UserName = name1; u.Email = email1; });
            Factories.User.Save(u => { u.UserName = name2; u.Email = email2; });
            Factories.User.Save();

            new UserQuery { NameOrEmail = name1 }.Count().Should().Be(1);
            new UserQuery { NameOrEmail = email1 }.Count().Should().Be(1);
            new UserQuery { NameOrEmail = name2 }.Count().Should().Be(1);
            new UserQuery { NameOrEmail = email2 }.Count().Should().Be(1);
            new UserQuery { NameOrEmail = Rand.Email() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_UserNameLike()
        {
            var name1 = Rand.String();
            var name2 = name1 + Rand.String();

            Factories.User.Save(u => u.UserName = name1);
            Factories.User.Save(u => u.UserName = name2);

            new UserQuery { UserNameLike = name1 }.Count().Should().Be(2);
            new UserQuery { UserNameLike = name2 }.Count().Should().Be(1);
            new UserQuery { UserNameLike = Rand.String() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NameOrEmailLike()
        {
            var name1 = Rand.String();
            var email1 = Rand.Email();
            var name2 = Rand.String();
            var email2 = Rand.Email();

            Factories.User.Save(u => { u.UserName = name1; u.Email = email1; });
            Factories.User.Save(u => { u.UserName = name2; u.Email = email2; });
            Factories.User.Save();

            new UserQuery { NameOrEmailLike = name1 }.Count().Should().Be(1);
            new UserQuery { NameOrEmailLike = email1 }.Count().Should().Be(1);
            new UserQuery { NameOrEmailLike = name2 }.Count().Should().Be(1);
            new UserQuery { NameOrEmailLike = email2 }.Count().Should().Be(1);
            new UserQuery { NameOrEmailLike = Rand.Email() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_UserName()
        {
            var name1 = Rand.String();
            var name2 = name1 + Rand.String();

            Factories.User.Save(u => u.UserName = name1);
            Factories.User.Save(u => u.UserName = name2);

            new UserQuery { UserName = name1 }.Count().Should().Be(1);
            new UserQuery { UserName = name2 }.Count().Should().Be(1);
            new UserQuery { UserName = Rand.String() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_IsLockedOut()
        {
            Factories.User.Save(u => u.IsLockedOut = true);
            Factories.User.Save(u => u.IsLockedOut = true);
            Factories.User.Save(u => u.IsLockedOut = false);

            new UserQuery { IsLockedOut = true }.Count().Should().Be(2);
            new UserQuery { IsLockedOut = false }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_RoleId()
        {
            var role1 = Factories.Role.Save();
            var role2 = Factories.Role.Save();

            Factories.User.Save(u => u.Role = role1);
            Factories.User.Save(u => u.Role = role1);
            Factories.User.Save(u => u.Role = role2);

            new UserQuery { RoleId = role1.Id }.Count().Should().Be(2);
            new UserQuery { RoleId = role2.Id }.Count().Should().Be(1);
            new UserQuery { RoleId = Rand.Int(5000) }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_RoleIds()
        {
            var role1 = Factories.Role.Save();
            var role2 = Factories.Role.Save();
            var role3 = Factories.Role.Save();

            Factories.User.Save(u => u.Role = role1);
            Factories.User.Save(u => u.Role = role1);
            Factories.User.Save(u => u.Role = role2);

            new UserQuery { RoleIds = new[] { role1.Id, role2.Id } }.Count().Should().Be(3);
            new UserQuery { RoleIds = new[] { role3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_CompanyIds()
        {
            var company1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany);
            var company2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany);
            var company3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.UserCompany);

            Factories.User.Save(u => u.Company = company1);
            Factories.User.Save(u => u.Company = company1);
            Factories.User.Save(u => u.Company = company2);

            new UserQuery { CompanyIds = new[] { company1.Id, company2.Id } }.Count().Should().Be(3);
            new UserQuery { CompanyIds = new[] { company3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var user = Factories.User.Save();
            Factories.User.Save();
            Factories.User.Save();

            new UserQuery
            {
                NameOrEmail = user.Email,
                UserNameLike = user.UserName,
                NameOrEmailLike = user.Email,
                RoleId = user.Role.Id,
                RoleIds = new[] { user.Role.Id },
                CompanyIds = new[] { user.Company.Id },
            }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_Role_Name()
        {
            var role1 = Factories.Role.Save();
            var role2 = Factories.Role.Save();
            var role3 = Factories.Role.Save();

            var orderedRoles =
                new[] { role1, role2, role3 }.OrderBy(a => a.Name).ToList();

            var user3 = Factories.User.Save(u => u.Role = orderedRoles[2]);
            var user2 = Factories.User.Save(u => u.Role = orderedRoles[1]);
            var user1 = Factories.User.Save(u => u.Role = orderedRoles[0]);

            var events = new UserQuery().OrderBy("Role.Name").List().ToList();
            events.Should().ContainInOrder(new[] { user1, user2, user3 });
        }
    }
}
