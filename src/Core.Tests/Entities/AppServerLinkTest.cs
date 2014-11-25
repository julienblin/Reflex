// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServerLinkTest.cs" company="CGI">
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
    public class AppServerLinkTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<AppServerLink>(NHSession, new PersistenceEqualityComparer())
                .CheckReference(x => x.Application, Factories.Application.Save())
                .CheckReference(x => x.Server, Factories.Server.Save())
                .VerifyTheMappings();
        }
    }
}
