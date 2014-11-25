// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationQueryTest.cs" company="CGI">
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
    public class ApplicationQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_entity()
        {
            new ApplicationQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_NameOrCodeLike()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            var code1 = Rand.String(10);
            var code2 = code1 + Rand.String(10);
            var code3 = Rand.String(10);

            Factories.Application.Save(a => { a.Name = name1; a.Code = code1; });
            Factories.Application.Save(a => { a.Name = name2; a.Code = code2; });
            Factories.Application.Save(a => { a.Name = name3; a.Code = code3; });

            new ApplicationQuery { NameOrCodeLike = name1 }.Count().Should().Be(2);
            new ApplicationQuery { NameOrCodeLike = name2 }.Count().Should().Be(1);
            new ApplicationQuery { NameOrCodeLike = name3 }.Count().Should().Be(1);

            new ApplicationQuery { NameOrCodeLike = code1 }.Count().Should().Be(2);
            new ApplicationQuery { NameOrCodeLike = code2 }.Count().Should().Be(1);
            new ApplicationQuery { NameOrCodeLike = code3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_NameLike()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Application.Save(a => a.Name = name1);
            Factories.Application.Save(a => a.Name = name2);
            Factories.Application.Save(a => a.Name = name3);

            new ApplicationQuery { NameLike = name1 }.Count().Should().Be(2);
            new ApplicationQuery { NameLike = name2 }.Count().Should().Be(1);
            new ApplicationQuery { NameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_CodeLike()
        {
            var code1 = Rand.String(10);
            var code2 = code1 + Rand.String(10);
            var code3 = Rand.String(10);

            Factories.Application.Save(a => a.Code = code1);
            Factories.Application.Save(a => a.Code = code2);
            Factories.Application.Save(a => a.Code = code3);

            new ApplicationQuery { CodeLike = code1 }.Count().Should().Be(2);
            new ApplicationQuery { CodeLike = code2 }.Count().Should().Be(1);
            new ApplicationQuery { CodeLike = code3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_ApplicationTypeId()
        {
            var applicationType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);

            Factories.Application.Save(a => a.ApplicationType = applicationType1);
            Factories.Application.Save(a => a.ApplicationType = applicationType1);
            Factories.Application.Save(a => a.ApplicationType = applicationType2);

            new ApplicationQuery { ApplicationTypeId = applicationType1.Id }.Count().Should().Be(2);
            new ApplicationQuery { ApplicationTypeId = applicationType2.Id }.Count().Should().Be(1);
            new ApplicationQuery { ApplicationTypeId = applicationType3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_ApplicationTypeIds()
        {
            var applicationType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);

            Factories.Application.Save(a => a.ApplicationType = applicationType1);
            Factories.Application.Save(a => a.ApplicationType = applicationType1);
            Factories.Application.Save(a => a.ApplicationType = applicationType2);

            new ApplicationQuery { ApplicationTypeIds = new[] { applicationType1.Id } }.Count().Should().Be(2);
            new ApplicationQuery { ApplicationTypeIds = new[] { applicationType1.Id, applicationType2.Id } }.Count().Should().Be(3);
            new ApplicationQuery { ApplicationTypeIds = new[] { applicationType3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_StatusId()
        {
            var applicationStatus1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);

            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus1);
            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus1);
            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus2);

            new ApplicationQuery { StatusId = applicationStatus1.Id }.Count().Should().Be(2);
            new ApplicationQuery { StatusId = applicationStatus2.Id }.Count().Should().Be(1);
            new ApplicationQuery { StatusId = applicationStatus3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_StatusIds()
        {
            var applicationStatus1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);

            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus1);
            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus1);
            Factories.Application.Save(a => a.AppInfo.Status = applicationStatus2);

            new ApplicationQuery { StatusIds = new[] { applicationStatus1.Id } }.Count().Should().Be(2);
            new ApplicationQuery { StatusIds = new[] { applicationStatus1.Id, applicationStatus2.Id } }.Count().Should().Be(3);
            new ApplicationQuery { StatusIds = new[] { applicationStatus3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_CriticityId()
        {
            var applicationCriticity1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);

            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity1);
            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity1);
            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity2);

            new ApplicationQuery { CriticityId = applicationCriticity1.Id }.Count().Should().Be(2);
            new ApplicationQuery { CriticityId = applicationCriticity2.Id }.Count().Should().Be(1);
            new ApplicationQuery { CriticityId = applicationCriticity3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_CriticityIds()
        {
            var applicationCriticity1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);

            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity1);
            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity1);
            Factories.Application.Save(a => a.AppInfo.Criticity = applicationCriticity2);

            new ApplicationQuery { CriticityIds = new[] { applicationCriticity1.Id } }.Count().Should().Be(2);
            new ApplicationQuery { CriticityIds = new[] { applicationCriticity1.Id, applicationCriticity2.Id } }.Count().Should().Be(3);
            new ApplicationQuery { CriticityIds = new[] { applicationCriticity3.Id } }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LinkedTechnologyId()
        {
            var techno1 = Factories.Technology.Save();
            var techno11 = Factories.Technology.Save(t => t.Parent = techno1);
            var techno2 = Factories.Technology.Save();
            NHSession.Refresh(techno1);

            var app1 = Factories.Application.Save();
            app1.AddTechnologyLinks(new[] { techno1, techno2 });
            var app2 = Factories.Application.Save();
            app2.AddTechnologyLink(techno1);
            Factories.Application.Save();

            var app3 = Factories.Application.Save();
            app3.AddTechnologyLink(techno11);

            new ApplicationQuery { LinkedTechnologyId = techno2.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedTechnologyId = techno1.Id }.Count().Should().Be(3);
            new ApplicationQuery { LinkedTechnologyId = techno11.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedTechnologyId = techno2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LinkedDbInstanceId()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();

            var app1 = Factories.Application.Save();
            app1.AddDbInstanceLinks(new[] { db1, db2 });
            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLink(db1);
            Factories.Application.Save();

            new ApplicationQuery { LinkedDbInstanceId = db2.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedDbInstanceId = db1.Id }.Count().Should().Be(2);
            new ApplicationQuery { LinkedDbInstanceId = db2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LinkedContactId()
        {
            var contact1 = Factories.Contact.Save();
            var contact2 = Factories.Contact.Save();

            var app1 = Factories.Application.Save();
            app1.AddContactLinks(new[] { contact1, contact2 });
            var app2 = Factories.Application.Save();
            app2.AddContactLink(contact1);
            Factories.Application.Save();

            new ApplicationQuery { LinkedContactId = contact2.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedContactId = contact1.Id }.Count().Should().Be(2);
            new ApplicationQuery { LinkedContactId = contact2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LinkedSectorId()
        {
            var sector1 = Factories.Sector.Save();
            var sector11 = Factories.Sector.Save(s => s.Parent = sector1);
            var sector2 = Factories.Sector.Save();
            
            NHSession.Refresh(sector1);

            Factories.Application.Save(a => a.AppInfo.Sector = sector11);
            Factories.Application.Save(a => a.AppInfo.Sector = sector1);
            Factories.Application.Save(a => a.AppInfo.Sector = sector2);

            new ApplicationQuery { LinkedSectorId = sector1.Id }.Count().Should().Be(2);
            new ApplicationQuery { LinkedSectorId = sector11.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedSectorId = sector2.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedSectorId = sector2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LinkedServerId()
        {
            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();

            var app1 = Factories.Application.Save();
            app1.AddServerLinks(new[] { server1, server2 });
            var app2 = Factories.Application.Save();
            app2.AddServerLink(server1);
            Factories.Application.Save();

            new ApplicationQuery { LinkedServerId = server2.Id }.Count().Should().Be(1);
            new ApplicationQuery { LinkedServerId = server1.Id }.Count().Should().Be(2);
            new ApplicationQuery { LinkedServerId = server2.Id + 10 }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_with_combined_criteria()
        {
            var techno1 = Factories.Technology.Save();
            var db1 = Factories.DbInstance.Save();
            var contact1 = Factories.Contact.Save();
            var sector1 = Factories.Sector.Save();
            var server1 = Factories.Server.Save();
            var application1 = Factories.Application.Save(a => a.AppInfo.Sector = sector1);
            application1.AddTechnologyLink(techno1);
            application1.AddDbInstanceLink(db1);
            application1.AddContactLink(contact1);
            application1.AddServerLink(server1);

            Factories.Application.Save();
            Factories.Application.Save();

            new ApplicationQuery
                {
                    NameOrCodeLike = application1.Name,
                    NameLike = application1.Name,
                    CodeLike = application1.Code,
                    ApplicationTypeId = application1.ApplicationType.Id,
                    CriticityId = application1.AppInfo.Criticity.Id,
                    StatusId = application1.AppInfo.Status.Id,
                    LinkedTechnologyId = techno1.Id,
                    LinkedDbInstanceId = db1.Id,
                    LinkedContactId = contact1.Id,
                    LinkedSectorId = sector1.Id,
                    LinkedServerId = server1.Id
                }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_ApplicationType_Name()
        {
            var applicationType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);
            var applicationType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType);

            var orderedApplicationTypes =
                new[] { applicationType1, applicationType2, applicationType3 }.OrderBy(dv => dv.Name).ToList();

            var application3 = Factories.Application.Save(a => a.ApplicationType = orderedApplicationTypes[2]);
            var application2 = Factories.Application.Save(a => a.ApplicationType = orderedApplicationTypes[1]);
            var application1 = Factories.Application.Save(a => a.ApplicationType = orderedApplicationTypes[0]);

            var applications = new ApplicationQuery().OrderBy("ApplicationType.Name").List().ToList();
            applications.Should().ContainInOrder(new[] { application1, application2, application3 });
        }

        [Test]
        public void It_should_order_by_Status_Name()
        {
            var applicationStatus1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);
            var applicationStatus3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus);

            var orderedApplicationStatuses =
                new[] { applicationStatus1, applicationStatus2, applicationStatus3 }.OrderBy(dv => dv.Name).ToList();

            var application3 = Factories.Application.Save(a => a.AppInfo.Status = orderedApplicationStatuses[2]);
            var application2 = Factories.Application.Save(a => a.AppInfo.Status = orderedApplicationStatuses[1]);
            var application1 = Factories.Application.Save(a => a.AppInfo.Status = orderedApplicationStatuses[0]);

            var applications = new ApplicationQuery().OrderBy("Status.Name").List().ToList();
            applications.Should().ContainInOrder(new[] { application1, application2, application3 });
        }

        [Test]
        public void It_should_order_by_Criticity_Name()
        {
            var applicationCriticity1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);
            var applicationCriticity3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity);

            var orderedApplicationCriticity =
                new[] { applicationCriticity1, applicationCriticity2, applicationCriticity3 }.OrderBy(dv => dv.Name).ToList();

            var application3 = Factories.Application.Save(a => a.AppInfo.Criticity = orderedApplicationCriticity[2]);
            var application2 = Factories.Application.Save(a => a.AppInfo.Criticity = orderedApplicationCriticity[1]);
            var application1 = Factories.Application.Save(a => a.AppInfo.Criticity = orderedApplicationCriticity[0]);

            var applications = new ApplicationQuery().OrderBy("Criticity.Name").List().ToList();
            applications.Should().ContainInOrder(new[] { application1, application2, application3 });
        }
    }
}
