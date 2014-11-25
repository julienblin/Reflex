// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueTest.cs" company="CGI">
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
    public class DomainValueTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<DomainValue>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Category, Rand.Enum<DomainValueCategory>())
                .CheckProperty(x => x.DisplayOrder, Rand.Int(10))
                .CheckProperty(x => x.Name, Rand.String(10))
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .CheckProperty(x => x.Color, Rand.Color())
                .VerifyTheMappings();
        }
    }
}
