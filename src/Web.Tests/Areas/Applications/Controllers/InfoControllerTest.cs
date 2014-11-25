// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.Info;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class InfoControllerTest : BaseControllerTest<InfoController>
    {
        [Test]
        public void Details_should_return_view()
        {
            Controller.Details(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
            
            var application = Factories.Application.Save();

            var result = Controller.Details(application.Id);
            result.Should().BeDefaultView();
            result.Model<InfoEdit>().Id.Should().Be(application.AppInfo.Id);
        }

        [Test]
        public void Edit_should_return_view()
        {
            Controller.Edit(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
            var application = Factories.Application.Save();

            var result = Controller.Edit(application.Id);
            result.Should().BeDefaultView();
            result.Model<InfoEdit>().Id.Should().Be(application.AppInfo.Id);
        }

        [Test]
        public void Edit_should_update_Info()
        {
            var application = Factories.Application.Save();
           
            var infoEdit = new InfoEdit
            {
                Id = application.AppInfo.Id,
                Name = Rand.String(),
                Code = Rand.String(),
                TypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType).Id,
                StatusId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus).Id,
                CriticityId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity).Id,
                Description = Rand.String(),
                UserRange = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationUserRange).Id,
                Category = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCategory).Id,
                Certification = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCertification).Id,
                MaintenanceWindow = Rand.String(),
                Notes = Rand.String(),
                ConcurrencyVersion = application.AppInfo.ConcurrencyVersion
            };

            StubStandardRequest();
            Controller.Edit(Rand.Int(int.MaxValue), infoEdit).Should().BeHttpNotFound();

            var result = Controller.Edit(application.Id, infoEdit);
            result.Should().BeRedirectToActionName("Details");

            application.Name.Should().Be(infoEdit.Name);
            application.Code.Should().Be(infoEdit.Code);
            application.ApplicationType.Id.Should().Be(infoEdit.TypeId.Value);

            application.AppInfo.Status.Id.Should().Be(infoEdit.StatusId.Value);
            application.AppInfo.Criticity.Id.Should().Be(infoEdit.CriticityId.Value);
            application.AppInfo.Description.Should().Be(infoEdit.Description);
            application.AppInfo.UserRange.Id.Should().Be(infoEdit.UserRange.Value);
            application.AppInfo.Category.Id.Should().Be(infoEdit.Category.Value);
            application.AppInfo.Certification.Id.Should().Be(infoEdit.Certification.Value);
            application.AppInfo.MaintenanceWindow.Should().Be(infoEdit.MaintenanceWindow);
            application.AppInfo.Notes.Should().Be(infoEdit.Notes);
        }

       [Test]
        public void Edit_should_not_update_Info_when_concurrency_doesnt_match()
        {
            var application = Factories.Application.Save();

            var infoEdit = new InfoEdit
            {
                Id = application.AppInfo.Id,
                StatusId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus).Id,
                CriticityId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity).Id,
                Description = Rand.String(),
                UserRange = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationUserRange).Id,
                Category = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCategory).Id,
                Certification = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCertification).Id,
                MaintenanceWindow = Rand.String(),
                Notes = Rand.String(),
                ConcurrencyVersion = application.AppInfo.ConcurrencyVersion - 1
            };

            StubStandardRequest();
            Controller.Edit(application.Id, infoEdit);
           
            application.AppInfo.Status.Id.Should().NotBe(infoEdit.StatusId.Value);
            application.AppInfo.Criticity.Id.Should().NotBe(infoEdit.CriticityId.Value);
            application.AppInfo.Description.Should().NotBe(infoEdit.Description);
            application.AppInfo.UserRange.Id.Should().NotBe(infoEdit.UserRange.Value);
            application.AppInfo.Certification.Id.Should().NotBe(infoEdit.Certification.Value);
            application.AppInfo.MaintenanceWindow.Should().NotBe(infoEdit.MaintenanceWindow);
            application.AppInfo.Notes.Should().NotBe(infoEdit.Notes);
        }
    }
}
