// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventTest.cs" company="CGI">
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
    public class EventTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Event>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .CheckProperty(x => x.Source, Rand.String(20))
                .CheckProperty(x => x.Date, Rand.DateTime())
                .CheckReference(x => x.Application, Factories.Application.Save())
                .CheckReference(x => x.Type, Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType))
                .VerifyTheMappings();
        }
    }
}
