// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerQueryTest.cs" company="CGI">
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
    public class ServerQueryTest : BaseDbTest
    {
        [Test]
        public void It_Should_Work_With_No_Entity()
        {
            new ServerQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NameOrAlias_Like()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Server.Save(a => a.Name = name1);
            Factories.Server.Save(a => a.Name = name2);
            Factories.Server.Save(a => a.Name = name3);

            new ServerQuery { NameOrAliasLike = name1 }.Count().Should().Be(2);
            new ServerQuery { NameOrAliasLike = name2 }.Count().Should().Be(1);
            new ServerQuery { NameOrAliasLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_Name_Like()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Server.Save(a => a.Name = name1);
            Factories.Server.Save(a => a.Name = name2);
            Factories.Server.Save(a => a.Name = name3);

            new ServerQuery { NameLike = name1 }.Count().Should().Be(2);
            new ServerQuery { NameLike = name2 }.Count().Should().Be(1);
            new ServerQuery { NameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_Alias_Like()
        {
            var alias1 = Rand.String(10);
            var alias2 = alias1 + Rand.String(10);
            var alias3 = Rand.String(10);

            Factories.Server.Save(a => a.Alias = alias1);
            Factories.Server.Save(a => a.Alias = alias2);
            Factories.Server.Save(a => a.Alias = alias3);

            new ServerQuery { AliasLike = alias1 }.Count().Should().Be(2);
            new ServerQuery { AliasLike = alias2 }.Count().Should().Be(1);
            new ServerQuery { AliasLike = alias3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_EvergreenDateTo()
        {
            var date1 = Rand.DateTime();
            var date2 = Rand.DateTime(future: true);
            var date3 = date1.AddDays((Rand.Int(50) * -1) - 10);

            Factories.Server.Save(a => a.EvergreenDate = date1);
            Factories.Server.Save(a => a.EvergreenDate = date2);
            Factories.Server.Save(a => a.EvergreenDate = date3);

            new ServerQuery { EvergreenDateTo = date1 }.Count().Should().Be(2);
            new ServerQuery { EvergreenDateTo = date2 }.Count().Should().Be(3);
            new ServerQuery { EvergreenDateTo = date3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_EnvironmentId()
        {
            var environment1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment);
            var environment2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment);

            Factories.Server.Save(a => a.Environment = environment1);
            Factories.Server.Save(a => a.Environment = environment1);
            Factories.Server.Save(a => a.Environment = environment2);

            new ServerQuery { EnvironmentId = environment1.Id }.Count().Should().Be(2);
            new ServerQuery { EnvironmentId = environment2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_EnvironmentIds()
        {
            var environment1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment);
            var environment2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment);

            Factories.Server.Save(a => a.Environment = environment1);
            Factories.Server.Save(a => a.Environment = environment1);
            Factories.Server.Save(a => a.Environment = environment2);

            new ServerQuery { EnvironmentIds = new[] { environment1.Id } }.Count().Should().Be(2);
            new ServerQuery { EnvironmentIds = new[] { environment1.Id, environment2.Id } }.Count().Should().Be(3);
        }

        [Test]
        public void It_should_filter_by_RoleId()
        {
            var serverRole1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);
            var serverRole2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);
            var serverRole3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);

            Factories.Server.Save(a => a.Role = serverRole1);
            Factories.Server.Save(a => a.Role = serverRole1);
            Factories.Server.Save(a => a.Role = serverRole2);

            new ServerQuery { RoleId = serverRole1.Id }.Count().Should().Be(2);
            new ServerQuery { RoleId = serverRole2.Id }.Count().Should().Be(1);
            new ServerQuery { RoleId = serverRole3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_RoleIds()
        {
            var serverRole1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);
            var serverRole2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);
            var serverRole3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole);

            Factories.Server.Save(a => a.Role = serverRole1);
            Factories.Server.Save(a => a.Role = serverRole1);
            Factories.Server.Save(a => a.Role = serverRole2);

            new ServerQuery { RoleIds = new[] { serverRole1.Id } }.Count().Should().Be(2);
            new ServerQuery { RoleIds = new[] { serverRole1.Id, serverRole2.Id } }.Count().Should().Be(3);
            new ServerQuery { RoleIds = new[] { serverRole3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_StatusId()
        {
            var serverStatus1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);
            var serverStatus2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);
            var serverStatus3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);

            Factories.Server.Save(a => a.Status = serverStatus2);
            Factories.Server.Save(a => a.Status = serverStatus2);
            Factories.Server.Save(a => a.Status = serverStatus3);

            new ServerQuery { StatusId = serverStatus1.Id }.Count().Should().Be(0);
            new ServerQuery { StatusId = serverStatus2.Id }.Count().Should().Be(2);
            new ServerQuery { StatusId = serverStatus3.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_StatusIds()
        {
            var serverStatus1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);
            var serverStatus2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);
            var serverStatus3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus);

            Factories.Server.Save(a => a.Status = serverStatus2);
            Factories.Server.Save(a => a.Status = serverStatus2);
            Factories.Server.Save(a => a.Status = serverStatus3);

            new ServerQuery { StatusIds = new[] { serverStatus1.Id } }.Count().Should().Be(0);
            new ServerQuery { StatusIds = new[] { serverStatus1.Id, serverStatus2.Id } }.Count().Should().Be(2);
            new ServerQuery { StatusIds = new[] { serverStatus3.Id } }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_TypeId()
        {
            var serverType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);
            var serverType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);
            var serverType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);

            Factories.Server.Save(a => a.Type = serverType1);
            Factories.Server.Save(a => a.Type = serverType1);
            Factories.Server.Save(a => a.Type = serverType1);

            new ServerQuery { TypeId = serverType1.Id }.Count().Should().Be(3);
            new ServerQuery { TypeId = serverType2.Id }.Count().Should().Be(0);
            new ServerQuery { TypeId = serverType3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_TypeIds()
        {
            var serverType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);
            var serverType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);
            var serverType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType);

            Factories.Server.Save(a => a.Type = serverType1);
            Factories.Server.Save(a => a.Type = serverType1);
            Factories.Server.Save(a => a.Type = serverType1);

            new ServerQuery { TypeIds = new[] { serverType1.Id } }.Count().Should().Be(3);
            new ServerQuery { TypeIds = new[] { serverType2.Id, serverType3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LandscapeId()
        {
            var landscape1 = Factories.Landscape.Save();
            var landscape2 = Factories.Landscape.Save();

            Factories.Server.Save(a => a.Landscape = landscape1);
            Factories.Server.Save(a => a.Landscape = landscape2);
            Factories.Server.Save(a => a.Landscape = landscape2);

            new ServerQuery { LandscapeId = landscape1.Id }.Count().Should().Be(1);
            new ServerQuery { LandscapeId = landscape2.Id }.Count().Should().Be(2);
        }

        [Test]
        public void It_should_filter_by_LinkedTechnologyId()
        {
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            var server1 = Factories.Server.Save();
            server1.AddTechnologyLinks(new[] { techno1, techno2 });
            var server2 = Factories.Server.Save();
            server2.AddTechnologyLink(techno1);
            Factories.Server.Save();

            new ServerQuery { LinkedTechnologyId = techno2.Id }.Count().Should().Be(1);
            new ServerQuery { LinkedTechnologyId = techno1.Id }.Count().Should().Be(2);
            new ServerQuery { LinkedTechnologyId = techno2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var techno1 = Factories.Technology.Save();
            var landscape1 = Factories.Landscape.Save();
            var server1 = Factories.Server.Save(s => s.Landscape = landscape1);
            server1.AddTechnologyLink(techno1);
            Factories.Server.Save();
            Factories.Server.Save();

            new ServerQuery
            {
                NameLike = server1.Name,
                AliasLike = server1.Alias,
                NameOrAliasLike = server1.Name,
                EvergreenDateTo = server1.EvergreenDate,
                EnvironmentId = server1.Environment.Id,
                RoleId = server1.Role.Id,
                StatusId = server1.Status.Id,
                TypeId = server1.Type.Id,
                LandscapeId = landscape1.Id,
                LinkedTechnologyId = techno1.Id
            }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_WithDbInstances()
        {
            Factories.DbInstance.Save();  // Create a linked server
            Factories.Server.Save();
            Factories.Server.Save();
            Factories.Server.Save();

            new ServerQuery { WithDbInstances = true }.Count().Should().Be(1);
            new ServerQuery { WithDbInstances = false }.Count().Should().Be(3);
        }
    }
}
