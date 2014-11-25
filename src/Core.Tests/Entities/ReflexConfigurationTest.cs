// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfigurationTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class ReflexConfigurationTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            var conf = ReflexConfiguration.GetCurrent();
            NHSession.SetReadOnly(conf, false);
            conf.SetActiveAppStatusDVIds(new[] { 5, 8, 10 });
            NHSession.Save(conf);

            NHSession.Flush();
            NHSession.Clear();

            conf = ReflexConfiguration.GetCurrent();
            conf.ActiveAppStatusDVIds.Should().OnlyContain(i => i == 5 || i == 8 || i == 10);
        }
    }
}
