// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTechnoLinkTest.cs" company="CGI">
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
    public class ServerTechnoLinkTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<ServerTechnoLink>(NHSession, new PersistenceEqualityComparer())
                .CheckReference(x => x.Server, Factories.Server.Save())
                .CheckReference(x => x.Technology, Factories.Technology.Save())
                .VerifyTheMappings();
        }
    }
}
