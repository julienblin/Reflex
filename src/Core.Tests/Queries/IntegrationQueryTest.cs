// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationQueryTest.cs" company="CGI">
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
    public class IntegrationQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new IntegrationQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_AppSourceId()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();

            Factories.Integration.Save(i => i.AppSource = application1);
            Factories.Integration.Save(i => i.AppSource = application1);
            Factories.Integration.Save(i => i.AppSource = application2);

            new IntegrationQuery { AppSourceId = application1.Id }.Count().Should().Be(2);
            new IntegrationQuery { AppSourceId = application2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_AppDestId()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();

            Factories.Integration.Save(i => i.AppDest = application1);
            Factories.Integration.Save(i => i.AppDest = application1);
            Factories.Integration.Save(i => i.AppDest = application2);

            new IntegrationQuery { AppDestId = application1.Id }.Count().Should().Be(2);
            new IntegrationQuery { AppDestId = application2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_ApplicationId()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();
            var application3 = Factories.Application.Save();

            Factories.Integration.Save(i => { i.AppSource = application1; i.AppDest = application2; });
            Factories.Integration.Save(i => { i.AppSource = application2; i.AppDest = application3; });

            new IntegrationQuery { ApplicationId = application1.Id }.Count().Should().Be(1);
            new IntegrationQuery { ApplicationId = application2.Id }.Count().Should().Be(2);
            new IntegrationQuery { ApplicationId = application3.Id }.Count().Should().Be(1);
            new IntegrationQuery { ApplicationId = Factories.Application.Save().Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NatureId()
        {
            var nature1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature);
            var nature2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature);

            Factories.Integration.Save(i => i.Nature = nature1);
            Factories.Integration.Save(i => i.Nature = nature1);
            Factories.Integration.Save(i => i.Nature = nature2);

            new IntegrationQuery { NatureId = nature1.Id }.Count().Should().Be(2);
            new IntegrationQuery { NatureId = nature2.Id }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String();
            var name2 = name1 + Rand.String();
            var name3 = Rand.String();

            Factories.Integration.Save(i => i.Name = name1);
            Factories.Integration.Save(i => i.Name = name2);

            new IntegrationQuery { NameLike = name1 }.Count().Should().Be(2);
            new IntegrationQuery { NameLike = name2 }.Count().Should().Be(1);
            new IntegrationQuery { NameLike = Rand.String() }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var integration = Factories.Integration.Save();
            Factories.Integration.Save();
            Factories.Integration.Save();

            new IntegrationQuery
            {
                AppDestId = integration.AppDest.Id,
                AppSourceId = integration.AppSource.Id,
                NameLike = integration.Name,
                NatureId = integration.Nature.Id
            }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_AppSource_Name()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();
            var application3 = Factories.Application.Save();

            var orderedApplications =
                new[] { application1, application2, application3 }.OrderBy(a => a.Name).ToList();

            var integration3 = Factories.Integration.Save(a => a.AppSource = orderedApplications[2]);
            var integration2 = Factories.Integration.Save(a => a.AppSource = orderedApplications[1]);
            var integration1 = Factories.Integration.Save(a => a.AppSource = orderedApplications[0]);

            var events = new IntegrationQuery().OrderBy("AppSource.Name").List().ToList();
            events.Should().ContainInOrder(new[] { integration1, integration2, integration3 });
        }

        [Test]
        public void It_should_order_by_AppDest_Name()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();
            var application3 = Factories.Application.Save();

            var orderedApplications =
                new[] { application1, application2, application3 }.OrderBy(a => a.Name).ToList();

            var integration3 = Factories.Integration.Save(a => a.AppDest = orderedApplications[2]);
            var integration2 = Factories.Integration.Save(a => a.AppDest = orderedApplications[1]);
            var integration1 = Factories.Integration.Save(a => a.AppDest = orderedApplications[0]);

            var events = new IntegrationQuery().OrderBy("AppDest.Name").List().ToList();
            events.Should().ContainInOrder(new[] { integration1, integration2, integration3 });
        }

        [Test]
        public void It_should_order_by_Nature_Name()
        {
            var nature1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature);
            var nature2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature);
            var nature3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature);

            var orderedNatures =
                new[] { nature1, nature2, nature3 }.OrderBy(a => a.Name).ToList();

            var integration3 = Factories.Integration.Save(a => a.Nature = orderedNatures[2]);
            var integration2 = Factories.Integration.Save(a => a.Nature = orderedNatures[1]);
            var integration1 = Factories.Integration.Save(a => a.Nature = orderedNatures[0]);

            var events = new IntegrationQuery().OrderBy("Nature.Name").List().ToList();
            events.Should().ContainInOrder(new[] { integration1, integration2, integration3 });
        }
    }
}
