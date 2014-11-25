// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class LogEntryQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new LogEntryQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_CorrelationId()
        {
            var correlationId = Rand.String();
            Factories.LogEntry.Save(le => le.CorrelationId = correlationId);
            Factories.LogEntry.Save();
            Factories.LogEntry.Save();

            new LogEntryQuery { CorrelationId = correlationId }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_Level()
        {
            var level1 = log4net.Core.Level.Warn.Name;
            Factories.LogEntry.Save(le => le.Level = level1);
            Factories.LogEntry.Save();
            Factories.LogEntry.Save();

            new LogEntryQuery { Level = level1 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_DateFrom()
        {
            var date1 = Rand.DateTime();
            var date2 = Rand.DateTime(future: true);
            var date3 = date1.AddDays((Rand.Int(50) * -1) - 10);

            Factories.LogEntry.Save(le => le.Date = date1);
            Factories.LogEntry.Save(le => le.Date = date2);
            Factories.LogEntry.Save(le => le.Date = date3);

            new LogEntryQuery { DateFrom = date1 }.Count().Should().Be(2);
            new LogEntryQuery { DateFrom = date2 }.Count().Should().Be(1);
            new LogEntryQuery { DateFrom = date3 }.Count().Should().Be(3);
        }

        [Test]
        public void It_should_filter_by_DateTo()
        {
            var date1 = Rand.DateTime();
            var date2 = Rand.DateTime(future: true);
            var date3 = date1.AddDays((Rand.Int(50) * -1) - 10);

            Factories.LogEntry.Save(le => le.Date = date1);
            Factories.LogEntry.Save(le => le.Date = date2);
            Factories.LogEntry.Save(le => le.Date = date3);

            new LogEntryQuery { DateTo = date1 }.Count().Should().Be(2);
            new LogEntryQuery { DateTo = date2 }.Count().Should().Be(3);
            new LogEntryQuery { DateTo = date3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_Logger()
        {
            var logger = Rand.String();
            Factories.LogEntry.Save(le => le.Logger = logger);
            Factories.LogEntry.Save();
            Factories.LogEntry.Save();

            new LogEntryQuery { Logger = logger }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_MessageLike()
        {
            var message = Rand.LoremIpsum();
            Factories.LogEntry.Save(le => le.Message = message);
            Factories.LogEntry.Save(le => le.Message = Rand.String());
            Factories.LogEntry.Save(le => le.Message = Rand.String());

            new LogEntryQuery { MessageLike = message }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_ExceptionLike()
        {
            var exception = Rand.LoremIpsum();
            Factories.LogEntry.Save(le => le.Exception = exception);
            Factories.LogEntry.Save(le => le.Exception = Rand.String());
            Factories.LogEntry.Save(le => le.Exception = Rand.String());

            new LogEntryQuery { ExceptionLike = exception }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var le = Factories.LogEntry.Save();

            new LogEntryQuery
            {
                CorrelationId = le.CorrelationId,
                Level = le.Level,
                DateFrom = le.Date,
                DateTo = le.Date,
                Logger = le.Logger,
                MessageLike = le.Message,
                ExceptionLike = le.Exception
            }.Count().Should().Be(1);
        }
    }
}
