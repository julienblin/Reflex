// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.Integrations;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class IntegrationsControllerTest : BaseControllerTest<IntegrationsController>
    {
        [Test]
        public void Details()
        {
            Controller.Details(Rand.Int(int.MaxValue), new IntegrationsDetails()).Should().BeHttpNotFound();

            var application = Factories.Application.Save();
            Factories.Integration.Save(i => i.AppSource = application);
            Factories.Integration.Save(i => i.AppDest = application);
            Factories.Integration.Save();

            var result = Controller.Details(application.Id, new IntegrationsDetails());
            result.Should().BeDefaultView();
            result.Model<IntegrationsDetails>().SearchResults.TotalItems.Should().Be(2);
            result.Model<IntegrationsDetails>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Create_should_return_view()
        {
            var application = Factories.Application.Save();

            var result = Controller.Create(application.Id);
            result.Should().BeDefaultView();
            result.Model<IntegrationEdit>().AppSourceId.Should().Be(application.Id);
            result.Model<IntegrationEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_integration()
        {
            var appsource = Factories.Application.Save();
            var appdest = Factories.Application.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            var integEdit = new IntegrationEdit
            {
                AppSourceId = appsource.Id,
                AppDestId = appdest.Id,
                NatureId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature).Id,
                Name = Rand.String(),
                Description = Rand.String(),
                DataDescription = Rand.String(),
                Frequency = Rand.String(),
                Comments = Rand.String(),
                TechnoIds = new[] { techno1.Id, techno2.Id }
            };

            var result = Controller.Create(appsource.Id, integEdit);
            result.Should().BeRedirectToActionName("Details");

            var integration = NHSession.QueryOver<Integration>().SingleOrDefault();
            integration.Should().NotBeNull();
            integration.AppSource.Id.Should().Be(integEdit.AppSourceId);
            integration.AppDest.Id.Should().Be(integEdit.AppDestId);
            integration.Nature.Id.Should().Be(integEdit.NatureId.Value);
            integration.Name.Should().Be(integEdit.Name);
            integration.Description.Should().Be(integEdit.Description);
            integration.DataDescription.Should().Be(integEdit.DataDescription);
            integration.Frequency.Should().Be(integEdit.Frequency);
            integration.Comments.Should().Be(integEdit.Comments);
            integration.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno1.Id || tl.Technology.Id == techno2.Id);
        }

        [Test]
        public void Create_should_not_create_integration_when_current_application_not_match_with_appsource_or_appdest()
        {
            var application = Factories.Application.Save();
            var appsource = Factories.Application.Save();
            var appdest = Factories.Application.Save();

            var integEdit = new IntegrationEdit
            {
                AppSourceId = appsource.Id,
                AppDestId = appdest.Id,
                NatureId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature).Id,
                Name = Rand.String(),
                Description = Rand.String(),
                DataDescription = Rand.String(),
                Frequency = Rand.String(),
                Comments = Rand.String()
            };

            var result = Controller.Create(application.Id, integEdit);
            result.Should().BeDefaultView();

            var integration = NHSession.QueryOver<Integration>().SingleOrDefault();
            integration.Should().BeNull();
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            // Arrange
            var integration = Factories.Integration.Save();

            // Act
            Controller.Edit(integration.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Edit(integration.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<IntegrationEdit>().Should().NotBeNull();
            result.Model<IntegrationEdit>().AppSourceId.Should().Be(integration.AppSource.Id);
            result.Model<IntegrationEdit>().AppDestId.Should().Be(integration.AppDest.Id);
            result.Model<IntegrationEdit>().NatureId.Should().Be(integration.Nature.Id);
            result.Model<IntegrationEdit>().Name.Should().Be(integration.Name);
            result.Model<IntegrationEdit>().Description.Should().Be(integration.Description);
            result.Model<IntegrationEdit>().DataDescription.Should().Be(integration.DataDescription);
            result.Model<IntegrationEdit>().Frequency.Should().Be(integration.Frequency);
            result.Model<IntegrationEdit>().Comments.Should().Be(integration.Comments);
            result.Model<IntegrationEdit>().ConcurrencyVersion.Should().Be(integration.ConcurrencyVersion);
            result.Model<IntegrationEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_update_Integration()
        {
            // Arrange
            var application = Factories.Application.Save();
            var targetApplication = Factories.Application.Save();
            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();
            var integration = Factories.Integration.Save(i => i.AppSource = application);
            integration.AddTechnologyLink(techno1);

            // Act
            Controller.Edit(application.Id, integration.Id + 1, new IntegrationEdit()).Should().BeHttpNotFound();

            var integEdit = new IntegrationEdit
            {
                AppSourceId = application.Id,
                AppDestId = targetApplication.Id,
                NatureId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature).Id,
                Name = Rand.String(),
                Description = Rand.String(),
                DataDescription = Rand.String(),
                Frequency = Rand.String(),
                Comments = Rand.String(),
                TechnoIds = new[] { techno2.Id },
                ConcurrencyVersion = integration.ConcurrencyVersion
            };

            var result = Controller.Edit(application.Id, integration.Id, integEdit);
            result.Should().BeRedirectToActionName("Details");
           
            // Assert
            integration.AppDest.Id.Should().Be(integEdit.AppDestId);
            integration.AppSource.Id.Should().Be(integEdit.AppSourceId);
            integration.Nature.Id.Should().Be(integEdit.NatureId.Value);
            integration.Description.Should().Be(integEdit.Description);
            integration.DataDescription.Should().Be(integEdit.DataDescription);
            integration.Frequency.Should().Be(integEdit.Frequency);
            integration.Comments.Should().Be(integEdit.Comments);
            integration.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == techno2.Id);
        }

        [Test]
        public void Edit_should_not_update_when_concurrency_version_doesnt_match()
        {
            // Arrange
            var application = Factories.Application.Save();
            var targetApplication = Factories.Application.Save();
            var integration = Factories.Integration.Save(i => i.AppSource = application);

            var integEdit = new IntegrationEdit
            {
                AppSourceId = application.Id,
                AppDestId = targetApplication.Id,
                NatureId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature).Id,
                Name = Rand.String(),
                Description = Rand.String(),
                DataDescription = Rand.String(),
                Frequency = Rand.String(),
                Comments = Rand.String(),
                ConcurrencyVersion = integration.ConcurrencyVersion - 1
            };

            // Act
            StubStandardRequest();
            Controller.Edit(application.Id, integration.Id + 1, integEdit).Should().BeHttpNotFound();
            var result = Controller.Edit(application.Id, integration.Id, integEdit);

            // Assert
            integration.Nature.Id.Should().NotBe(integEdit.NatureId.Value);
            integration.Description.Should().NotBe(integEdit.Description);
            integration.DataDescription.Should().NotBe(integEdit.DataDescription);
            integration.Frequency.Should().NotBe(integEdit.Frequency);
            integration.Comments.Should().NotBe(integEdit.Comments);
        }

        [Test]
        public void Edit_should_not_update_integration_when_current_application_not_match_with_appsource_or_appdest()
        {
            // Arrange
            var application = Factories.Application.Save();
            var integration = Factories.Integration.Save(i => i.AppSource = application);
            var targetAppSource = Factories.Application.Save();

            var integEdit = new IntegrationEdit
            {
                AppSourceId = targetAppSource.Id,
                AppDestId = integration.AppDest.Id,
                NatureId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.IntegrationNature).Id,
                Name = Rand.String(),
                Description = Rand.String(),
                DataDescription = Rand.String(),
                Frequency = Rand.String(),
                Comments = Rand.String(),
                ConcurrencyVersion = integration.ConcurrencyVersion
            };

            // Act   
            var result = Controller.Edit(application.Id, integration.Id, integEdit);
           
            // Assert
            integration.AppSource.Id.Should().NotBe(integEdit.AppSourceId);
            integration.Nature.Id.Should().NotBe(integEdit.NatureId.Value);
            integration.Description.Should().NotBe(integEdit.Description);
            integration.DataDescription.Should().NotBe(integEdit.DataDescription);
            integration.Frequency.Should().NotBe(integEdit.Frequency);
            integration.Comments.Should().NotBe(integEdit.Comments);
        }

        [Test]
        public void Delete_should_return_view()
        {
            var integration = Factories.Integration.Save();

            Controller.Delete(integration.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(integration.Id);

            result.Should().BePartialView("_DeleteModal");
            result.Model<IntegrationEdit>().Id.Should().Be(integration.Id);
        }

        [Test]
        public void Delete_should_delete_Integration()
        {
            var integration = Factories.Integration.Save();

            var integEdit = new IntegrationEdit
            {
                Id = integration.Id,
            };

            Controller.Delete(new IntegrationEdit { Id = integration.Id + 1 }).Should().BeHttpNotFound();
            var result = Controller.Delete(integEdit);
            result.Should().BeOfType<RedirectToRouteResult>();
            NHSession.Get<Event>(integration.Id).Should().BeNull();
        }
    }
}
