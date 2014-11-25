// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventQueryTest.cs" company="CGI">
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
    public class EventQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new EventQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_ApplicationId()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();

            Factories.Event.Save(a => a.Application = application1);
            Factories.Event.Save(a => a.Application = application1);
            Factories.Event.Save(a => a.Application = application2);

            new EventQuery { ApplicationId = application1.Id }.Count().Should().Be(2);
            new EventQuery { ApplicationId = application2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_SourceLike()
        {
            var source1 = Rand.String();
            var source2 = source1 + Rand.String();
            var source3 = Rand.String();

            Factories.Event.Save(a => a.Source = source1);
            Factories.Event.Save(a => a.Source = source2);
            Factories.Event.Save(a => a.Source = source3);

            new EventQuery { SourceLike = source1 }.Count().Should().Be(2);
            new EventQuery { SourceLike = source2 }.Count().Should().Be(1);
            new EventQuery { SourceLike = source3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_DateFrom()
        {
            var date1 = Rand.DateTime();
            var date2 = Rand.DateTime(future: true);
            var date3 = date1.AddDays((Rand.Int(50) * -1) - 10);

            Factories.Event.Save(a => a.Date = date1);
            Factories.Event.Save(a => a.Date = date2);
            Factories.Event.Save(a => a.Date = date3);

            new EventQuery { DateFrom = date1 }.Count().Should().Be(2);
            new EventQuery { DateFrom = date2 }.Count().Should().Be(1);
            new EventQuery { DateFrom = date3 }.Count().Should().Be(3);
        }

        [Test]
        public void It_should_filter_by_DateTo()
        {
            var date1 = Rand.DateTime();
            var date2 = Rand.DateTime(future: true);
            var date3 = date1.AddDays((Rand.Int(50) * -1) - 10);

            Factories.Event.Save(a => a.Date = date1);
            Factories.Event.Save(a => a.Date = date2);
            Factories.Event.Save(a => a.Date = date3);

            new EventQuery { DateTo = date1 }.Count().Should().Be(2);
            new EventQuery { DateTo = date2 }.Count().Should().Be(3);
            new EventQuery { DateTo = date3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_TypeId()
        {
            var eventType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType);
            var eventType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType);

            Factories.Event.Save(a => a.Type = eventType1);
            Factories.Event.Save(a => a.Type = eventType1);
            Factories.Event.Save(a => a.Type = eventType2);

            new EventQuery { TypeId = eventType1.Id }.Count().Should().Be(2);
            new EventQuery { TypeId = eventType2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var event1 = Factories.Event.Save();
            Factories.Event.Save();
            Factories.Event.Save();

            new EventQuery
            {
                ApplicationId = event1.Application.Id,
                SourceLike = event1.Source,
                DateFrom = event1.Date,
                DateTo = event1.Date,
                TypeId = event1.Type.Id
            }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_EventType_Name()
        {
            var eventType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType);
            var eventType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType);
            var eventType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType);

            var orderedEventTypes =
                new[] { eventType1, eventType2, eventType3 }.OrderBy(dv => dv.Name).ToList();

            var event3 = Factories.Event.Save(a => a.Type = orderedEventTypes[2]);
            var event2 = Factories.Event.Save(a => a.Type = orderedEventTypes[1]);
            var event1 = Factories.Event.Save(a => a.Type = orderedEventTypes[0]);

            var events = new EventQuery().OrderBy("EventType.Name").List().ToList();
            events.Should().ContainInOrder(new[] { event1, event2, event3 });
        }
    }
}
