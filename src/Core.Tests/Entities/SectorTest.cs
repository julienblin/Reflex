// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorTest.cs" company="CGI">
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
    public class SectorTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Sector>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(30))
                .VerifyTheMappings();
        }
    }
}
