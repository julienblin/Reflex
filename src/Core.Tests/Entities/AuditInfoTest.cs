// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditInfoTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class AuditInfoTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<AuditInfo>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.EntityType, Rand.String(30))
                .CheckProperty(x => x.EntityId, Rand.Int(30))
                .CheckProperty(x => x.ConcurrencyVersion, Rand.Int(30))
                .CheckProperty(x => x.Timestamp, Rand.DateTime())
                .CheckProperty(x => x.Action, Rand.Enum<AuditInfoAction>())
                .CheckReference(x => x.User, Factories.User.Save())
                .VerifyTheMappings();
        }
    }
}
