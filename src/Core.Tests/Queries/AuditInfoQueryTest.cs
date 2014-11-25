// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoQueryTest.cs" company="CGI">
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
    public class AuditInfoQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new AuditInfoQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_EntityType()
        {
            var entityType1 = Rand.String();
            var entityType2 = Rand.String();

            var auditInfo1 = Factories.AuditInfo.Save(ai => ai.EntityType = entityType1);
            var auditInfo2 = Factories.AuditInfo.Save(ai => ai.EntityType = entityType1);
            var auditInfo3 = Factories.AuditInfo.Save(ai => ai.EntityType = entityType2);

            new AuditInfoQuery { EntityType = entityType1 }.Count().Should().Be(2);
            new AuditInfoQuery { EntityType = entityType2 }.Count().Should().Be(1);
            new AuditInfoQuery { EntityType = Rand.String() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_EntityId()
        {
            var entityId1 = Rand.Int(10000);
            var entityId2 = Rand.Int(10000);

            var auditInfo1 = Factories.AuditInfo.Save(ai => ai.EntityId = entityId1);
            var auditInfo2 = Factories.AuditInfo.Save(ai => ai.EntityId = entityId1);
            var auditInfo3 = Factories.AuditInfo.Save(ai => ai.EntityId = entityId2);

            new AuditInfoQuery { EntityId = entityId1 }.Count().Should().Be(2);
            new AuditInfoQuery { EntityId = entityId2 }.Count().Should().Be(1);
            new AuditInfoQuery { EntityId = Rand.Int(10000) }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_PropertyName()
        {
            var propName1 = Rand.String();
            var propName2 = Rand.String();

            var auditInfo1 = Factories.AuditInfo.Create();
            auditInfo1.Add(new AuditInfoProperty
            {
                AuditInfo = auditInfo1,
                PropertyName = propName1,
                PropertyType = Rand.String()
            });
            NHSession.Save(auditInfo1);
            var auditInfo2 = Factories.AuditInfo.Create();
            auditInfo1.Add(new AuditInfoProperty
            {
                AuditInfo = auditInfo2,
                PropertyName = propName1,
                PropertyType = Rand.String()
            });
            NHSession.Save(auditInfo2);

            var auditInfo3 = Factories.AuditInfo.Create();
            auditInfo1.Add(new AuditInfoProperty
            {
                AuditInfo = auditInfo3,
                PropertyName = propName2,
                PropertyType = Rand.String()
            });
            NHSession.Save(auditInfo3);

            new AuditInfoQuery { PropertyName = propName1 }.Count().Should().Be(2);
            new AuditInfoQuery { PropertyName = propName2 }.Count().Should().Be(1);
            new AuditInfoQuery { PropertyName = Rand.String() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var auditInfo = Factories.AuditInfo.Save();
            Factories.AuditInfo.Save();
            Factories.AuditInfo.Save();

            new AuditInfoQuery
            {
                EntityType = auditInfo.EntityType,
                EntityId = auditInfo.EntityId
            }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_User_UserName()
        {
            var user1 = Factories.User.Save();
            var user2 = Factories.User.Save();
            var user3 = Factories.User.Save();

            var orderUsers = new[] { user1, user2, user3 }.OrderBy(u => u.UserName).ToList();

            var auditInfo3 = Factories.AuditInfo.Save(ai => ai.User = orderUsers[2]);
            var auditInfo2 = Factories.AuditInfo.Save(ai => ai.User = orderUsers[1]);
            var auditInfo1 = Factories.AuditInfo.Save(ai => ai.User = orderUsers[0]);

            var result = new AuditInfoQuery().OrderBy("User.UserName").List().ToList();
            result.Should().Contain(auditInfo1);
            result.Should().Contain(auditInfo2);
            result.Should().Contain(auditInfo3);

            var indexOfAuditInfo1 = result.IndexOf(auditInfo1);
            var indexOfAuditInfo2 = result.IndexOf(auditInfo2);
            var indexOfAuditInfo3 = result.IndexOf(auditInfo3);

            indexOfAuditInfo1.Should().BeLessThan(indexOfAuditInfo2);
            indexOfAuditInfo2.Should().BeLessThan(indexOfAuditInfo3);
        }
    }
}
