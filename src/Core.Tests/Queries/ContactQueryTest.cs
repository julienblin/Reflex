// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactQueryTest.cs" company="CGI">
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
    public class ContactQueryTest : BaseDbTest
    {
        [Test]
        public void It_Should_Work_With_No_Entity()
        {
            new ContactQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_LastName_Like()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Contact.Save(a => a.LastName = name1);
            Factories.Contact.Save(a => a.LastName = name2);
            Factories.Contact.Save(a => a.LastName = name3);

            new ContactQuery { LastNameLike = name1 }.Count().Should().Be(2);
            new ContactQuery { LastNameLike = name2 }.Count().Should().Be(1);
            new ContactQuery { LastNameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_FirstName_Like()
        {
            var name1 = Rand.String(10);
            var name2 = name1 + Rand.String(10);
            var name3 = Rand.String(10);

            Factories.Contact.Save(a => a.FirstName = name1);
            Factories.Contact.Save(a => a.FirstName = name2);
            Factories.Contact.Save(a => a.FirstName = name3);

            new ContactQuery { FirstNameLike = name1 }.Count().Should().Be(2);
            new ContactQuery { FirstNameLike = name2 }.Count().Should().Be(1);
            new ContactQuery { FirstNameLike = name3 }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_filter_by_ContactTypeId()
        {
            var dv1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);
            var dv2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);
            var dv3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);

            Factories.Contact.Save(c => c.Type = dv1);
            Factories.Contact.Save(c => c.Type = dv1);
            Factories.Contact.Save(c => c.Type = dv2);

            new ContactQuery { TypeId = dv1.Id }.Count().Should().Be(2);
            new ContactQuery { TypeId = dv2.Id }.Count().Should().Be(1);
            new ContactQuery { TypeId = dv3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_SectorId()
        {
            var sector1 = Factories.Sector.Save();
            var sector2 = Factories.Sector.Save();
            var sector3 = Factories.Sector.Save();

            Factories.Contact.Save(c => c.Sector = sector1);
            Factories.Contact.Save(c => c.Sector = sector1);
            Factories.Contact.Save(c => c.Sector = sector2);

            new ContactQuery { SectorId = sector1.Id }.Count().Should().Be(2);
            new ContactQuery { SectorId = sector2.Id }.Count().Should().Be(1);
            new ContactQuery { SectorId = sector3.Id }.Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_All_Criteria()
        {
            var sector = Factories.Sector.Save();
            var contact1 = Factories.Contact.Save(c => c.Sector = sector);
            Factories.Contact.Save();
            Factories.Contact.Save();

            new ContactQuery
                {
                    FirstNameLike = contact1.FirstName,
                    LastNameLike = contact1.LastName,
                    TypeId = contact1.Type.Id,
                    SectorId = sector.Id
                }.Count().Should().Be(1);
        }

        [Test]
        public void It_should_order_by_Type_Name()
        {
            var contactType1 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);
            var contactType2 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);
            var contactType3 = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ContactType);

            var orderedContactTypes =
                new[] { contactType1, contactType2, contactType3 }.OrderBy(dv => dv.Name).ToList();

            var contact3 = Factories.Contact.Save(a => a.Type = orderedContactTypes[2]);
            var contact2 = Factories.Contact.Save(a => a.Type = orderedContactTypes[1]);
            var contact1 = Factories.Contact.Save(a => a.Type = orderedContactTypes[0]);

            var applications = new ContactQuery().OrderBy("Type.Name").List().ToList();
            applications.Should().ContainInOrder(new[] { contact1, contact2, contact3 });
        }

        [Test]
        public void It_should_order_by_Sector_Name()
        {
            var sector1 = Factories.Sector.Save();
            var sector2 = Factories.Sector.Save();
            var sector3 = Factories.Sector.Save();

            var orderedSectors =
                new[] { sector1, sector2, sector3 }.OrderBy(s => s.Name).ToList();

            var contact3 = Factories.Contact.Save(a => a.Sector = orderedSectors[2]);
            var contact2 = Factories.Contact.Save(a => a.Sector = orderedSectors[1]);
            var contact1 = Factories.Contact.Save(a => a.Sector = orderedSectors[0]);

            var applications = new ContactQuery().OrderBy("Sector.Name").List().ToList();
            applications.Should().ContainInOrder(new[] { contact1, contact2, contact3 });
        }
    }
}
