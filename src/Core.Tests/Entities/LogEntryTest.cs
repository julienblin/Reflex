// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests.Factories;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class LogEntryTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<LogEntry>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Date, Rand.DateTime())
                .CheckProperty(x => x.Thread, Rand.String())
                .CheckProperty(x => x.Level, Rand.String())
                .CheckProperty(x => x.Logger, Rand.String())
                .CheckProperty(x => x.CorrelationId, Rand.String())
                .CheckProperty(x => x.Message, Rand.LoremIpsum())
                .CheckProperty(x => x.Exception, Rand.LoremIpsum())
                .VerifyTheMappings();
        }
    }
}
