// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Servers.Controllers;
using CGI.Reflex.Web.Areas.Servers.Models;
using CGI.Reflex.Web.Areas.Servers.Models.Servers;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Servers.Controllers
{
    public class ServersControllerTest : BaseControllerTest<ServersController>
    {
        [Test]
        public void Index_should_return_view()
        {
            StubStandardRequest();
            var result = Controller.Index(new ServersIndex());
            result.Should().BeDefaultView();
            result.Model<ServersIndex>().Should().NotBeNull();
            result.Model<ServersIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Index_should_return_partial_view_when_ajax()
        {
            StubAjaxRequest();
            var result = Controller.Index(new ServersIndex());
            result.Should().BePartialView("_List");
            result.Model<ServersIndex>().Should().NotBeNull();
        }

        [Test]
        public void Create_should_return_view()
        {
            var result = Controller.Create();
            result.Should().BePartialView("_CreateModal");
            result.Model<ServerHeader>().Should().NotBeNull();
        }

        [Test]
        public void Create_should_create_server()
        {
            var serverHeader = new ServerHeader
            {
                Name = Rand.String(10),
                Alias = Rand.String(10),
                EnvironmentId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment).Id
            };

            StubStandardRequest();
            var result = Controller.Create(serverHeader);
            result.Should().BeJSRedirect();

            var server = NHSession.QueryOver<Server>().SingleOrDefault();
            server.Should().NotBeNull();
            server.Name.Should().Be(serverHeader.Name);
            server.Alias.Should().Be(serverHeader.Alias);
            server.Environment.Id.Should().Be(serverHeader.EnvironmentId.Value);
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            Controller.Edit(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();

            // Arrange
            var server = Factories.Server.Save();

            // Act   
            var result = Controller.Edit(server.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<ServerEdit>().Id.Should().Be(server.Id);
        }

        [Test]
        public void Edit_should_update_Server()
        {
            var serverEdit = new ServerEdit
            {
                Name = Rand.String(),
                Alias = Rand.String(),
                Comments = Rand.String(),
                EvergreenDate = Rand.DateTime(),
                EnvironmentId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.Environment).Id,
                RoleId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerRole).Id,
                StatusId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerStatus).Id,
                TypeId = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ServerType).Id,
                LandscapeId = Factories.Landscape.Save().Id
            };

            Controller.Edit(Rand.Int(int.MaxValue), serverEdit).Should().BeHttpNotFound();
            var server = Factories.Server.Save();

            NHSession.Refresh(server);
            serverEdit.Id = server.Id;
            serverEdit.ConcurrencyVersion = server.ConcurrencyVersion;

            StubStandardRequest();

            var result = Controller.Edit(server.Id, serverEdit);
            result.Should().BeRedirectToActionName("Details");

            server.Name.Should().Be(serverEdit.Name);
            server.Alias.Should().Be(serverEdit.Alias);
            server.Comments.Should().Be(serverEdit.Comments);
            server.EvergreenDate.Should().Be(serverEdit.EvergreenDate);
            server.Environment.Should().Be(serverEdit.EnvironmentId.ToDomainValue());
            server.Role.Should().Be(serverEdit.RoleId.ToDomainValue());
            server.Status.Should().Be(serverEdit.StatusId.ToDomainValue());
            server.Type.Should().Be(serverEdit.TypeId.ToDomainValue());
            server.Landscape.Id.Should().Be(serverEdit.LandscapeId.Value);
        }

        [Test]
        public void Delete_should_return_view()
        {
            var server = Factories.Server.Save();

            Controller.Delete(server.Id + 1).Should().BeHttpNotFound();
            var result = Controller.Delete(server.Id);

            result.Should().BePartialView("_DeleteModal");
            NHSession.Get<Server>(server.Id).Should().NotBeNull();
        }

        [Test]
        public void Delete_should_delete_server()
        {
            var server = Factories.Server.Save();

            Controller.Delete(new ServerEdit { Id = server.Id + 1 }).Should().BeHttpNotFound();
            var result = Controller.Delete(new ServerEdit { Id = server.Id });

            result.Should().BeRedirectToActionName("Index");
            NHSession.Get<Server>(server.Id).Should().BeNull();
        }
    }
}
