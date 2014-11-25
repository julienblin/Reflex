// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.Servers;
using CGI.Reflex.Web.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class ServersControllerTest : BaseControllerTest<ServersController>
    {
        [Test]
        public void Details_should_return_not_found()
        {
            Controller.Details(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
        }

        [Test]
        public void Details_should_return_empty_view()
        {
            var application = Factories.Application.Save();

            var resulsts = Controller.Details(application.Id);

            resulsts.Should().BeDefaultView();
            resulsts.Model<ServersDetails>().Results.SelectedServer.Count().Should().Be(0);
            resulsts.Model<ServersDetails>().Results.Environments.Count().Should().Be(0);
            resulsts.Model<ServersDetails>().Results.Landscapes.Count().Should().Be(0);
            resulsts.Model<ServersDetails>().Results.OrderedEnvironments.Count().Should().Be(0);
            resulsts.Model<ServersDetails>().Results.OrderedLandscapes.Count().Should().Be(0);
        }

        [Test]
        public void Details_should_return_view()
        {
            var application1 = Factories.Application.Save();
            var application2 = Factories.Application.Save();

            var landscape = Factories.Landscape.Save();

            var env1 = Factories.DomainValue.Save(x => x.Category = DomainValueCategory.Environment);
            var env2 = Factories.DomainValue.Save(x => x.Category = DomainValueCategory.Environment);
            var env3 = Factories.DomainValue.Save(x => x.Category = DomainValueCategory.Environment);

            var server1 = Factories.Server.Save(x => x.Environment = env1);
            var server2 = Factories.Server.Save(x => { x.Environment = env1; x.Landscape = landscape; });
            var server3 = Factories.Server.Save(x => x.Environment = env1);
            var server4 = Factories.Server.Save(x => x.Environment = env2);
            var server5 = Factories.Server.Save(x => x.Environment = env2);
            var server6 = Factories.Server.Save(x => { x.Environment = env3; x.Landscape = landscape; });

            application1.AddServerLink(server1);
            application1.AddServerLink(server2);
            application2.AddServerLink(server2);
            application2.AddServerLink(server3);
            application2.AddServerLink(server4);
            application2.AddServerLink(server5);
            application2.AddServerLink(server6);

            var resulsts = Controller.Details(application2.Id);

            resulsts.Model<ServersDetails>().Results.SelectedServer.Count().Should().Be(5);
            resulsts.Model<ServersDetails>().Results.Environments.Count().Should().Be(3);
            resulsts.Model<ServersDetails>().Results.Landscapes.Count().Should().Be(4);
            resulsts.Model<ServersDetails>().Results.OrderedEnvironments.Count().Should().Be(3);
            resulsts.Model<ServersDetails>().Results.OrderedLandscapes.Count().Should().Be(4);
        }

        [Test]
        public void AddServers_should_return_not_found()
        {
            var server1 = Factories.Server.Save();
            Controller.AddServers(Rand.Int(int.MaxValue), new[] { server1.Id }).Should().BeHttpNotFound();
        }

        [Test]
        public void AddServers_should_add_servers()
        {
            var application = Factories.Application.Save();

            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();
            var server3 = Factories.Server.Save();

            var results = Controller.AddServers(application.Id, new[] { server1.Id, server2.Id, server3.Id });

            results.Should().BeRedirectToActionName("Details");

            application.ServerLinks.Count().Should().Be(3);
        }

        [Test]
        public void AddServer_should_return_not_found()
        {
            var server1 = Factories.Server.Save();
            Controller.AddServer(Rand.Int(int.MaxValue), server1.Id).Should().BeHttpNotFound();
        }

        [Test]
        public void AddServer_should_add_server()
        {
            var application = Factories.Application.Save();

            var server = Factories.Server.Save();

            var results = Controller.AddServer(application.Id, server.Id);

            results.Should().BePartialView("_LandscapesServersDisplay");

            application.ServerLinks.Count().Should().Be(1);
        }

        [Test]
        public void RemoveServer_should_return_not_found()
        {
            Controller.RemoveServer(Rand.Int(int.MaxValue), Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
        }

        [Test]
        public void RemoveServer_should_return_view()
        {
            var application = Factories.Application.Save();
            var server = Factories.Server.Save();

            application.AddServerLink(server);

            var results = Controller.RemoveServer(application.Id, server.Id);

            results.Should().BePartialView("_RemoveServerModal");

            results.Model<ServerRemove>().ApplicationName.Should().Be(application.Name);
            results.Model<ServerRemove>().Id.Should().Be(server.Id);
            results.Model<ServerRemove>().ServerName.Should().Be(server.Name);
        }

        [Test]
        public void RemoveServer_post_should_return_not_found()
        {
            Controller.RemoveServer(Rand.Int(int.MaxValue), new ServerRemove { Id = Rand.Int(int.MaxValue) }).Should().BeHttpNotFound();
        }

        [Test]
        public void RemoveServer_post_should_remove_server()
        {
            var application = Factories.Application.Save();

            var server1 = Factories.Server.Save();
            var server2 = Factories.Server.Save();
            var server3 = Factories.Server.Save();

            application.AddServerLinks(new[] { server1, server2, server3 });

            var model = new ServerRemove
            {
                ApplicationName = application.Name,
                Id = server2.Id,
                ServerName = server2.Name
            };

            var results = Controller.RemoveServer(application.Id, model);

            results.Should().BePartialView("_LandscapesServersDisplay");

            application.ServerLinks.Count().Should().Be(2);

            results.Model<LandscapesServersDisplay>().SelectedServer.Count().Should().Be(2);
        }

        [Test]
        public void RemoveServerByLandscape_should_return_not_found()
        {
            Controller.RemoveServerByLandscape(Rand.Int(int.MaxValue), Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
        }

        [Test]
        public void RemoveServerByLandscape_should_return_view()
        {
            var landscape1 = Factories.Landscape.Save();
            var landscape2 = Factories.Landscape.Save();

            var application = Factories.Application.Save();

            var server1 = Factories.Server.Save(x => x.Landscape = landscape1);
            var server2 = Factories.Server.Save(x => x.Landscape = landscape1);
            var server3 = Factories.Server.Save(x => x.Landscape = landscape2);
            var server4 = Factories.Server.Save(x => x.Landscape = landscape2);

            application.AddServerLinks(new[] { server1, server2, server3, server4 });

            var results = Controller.RemoveServerByLandscape(application.Id, landscape1.Id);

            results.Should().BePartialView("_RemoveLandscapeModal");

            results.Model<LandscapeRemove>().Id.Should().Be(landscape1.Id);
            results.Model<LandscapeRemove>().ApplicationName.Should().Be(application.Name);
            results.Model<LandscapeRemove>().LandscapeName.Should().Be(landscape1.Name);
        }

        [Test]
        public void RemoveServerByLandscape_post_should_return_not_found()
        {
            Controller.RemoveServerByLandscape(Rand.Int(int.MaxValue), new LandscapeRemove { Id = Rand.Int(int.MaxValue) }).Should().BeHttpNotFound();
        }

        [Test]
        public void RemoveServerByLandscape_post_should_remove_servers_in_landscape()
        {
            var landscape1 = Factories.Landscape.Save();
            var landscape2 = Factories.Landscape.Save();

            var application = Factories.Application.Save();

            var server1 = Factories.Server.Save(x => x.Landscape = landscape1);
            var server2 = Factories.Server.Save(x => x.Landscape = landscape1);
            var server3 = Factories.Server.Save(x => x.Landscape = landscape2);
            var server4 = Factories.Server.Save(x => x.Landscape = landscape2);

            NHSession.Refresh(landscape1);
            NHSession.Refresh(landscape2);

            application.AddServerLinks(new[] { server1, server2, server3, server4 });

            var model = new LandscapeRemove
            {
                Id = landscape1.Id,
                ApplicationName = application.Name,
                LandscapeName = landscape1.Name
            };

            var results = Controller.RemoveServerByLandscape(application.Id, model);

            results.Should().BePartialView("_LandscapesServersDisplay");

            application.ServerLinks.Count().Should().Be(2);

            results.Model<LandscapesServersDisplay>().Landscapes.Count().Should().Be(1);
            results.Model<LandscapesServersDisplay>().SelectedServer.Count().Should().Be(2);
        }
    }
}
