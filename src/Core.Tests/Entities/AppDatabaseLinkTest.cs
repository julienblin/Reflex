using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class AppDatabaseLinkTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<AppDatabaseLink>(NHSession, new PersistenceEqualityComparer())
                .CheckReference(x => x.Application, Factories.Application.Save())
                .CheckReference(x => x.Database, Factories.Database.Save())
                .VerifyTheMappings();
        }
    }
}
