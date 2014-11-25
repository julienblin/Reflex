using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Testing;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class DatabaseServerLinkTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<DatabaseServerLink>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.DatabaseName, Rand.String())
                .CheckProperty(x => x.DbInstanceName, Rand.String())
                .CheckReference(x => x.Database, Factories.Database.Save())
                .CheckReference(x => x.Server, Factories.Server.Save())
                .VerifyTheMappings();
        }
    }
}
