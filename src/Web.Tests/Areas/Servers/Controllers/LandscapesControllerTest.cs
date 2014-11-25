// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Servers.Controllers;
using CGI.Reflex.Web.Areas.Servers.Models.Landscapes;
using CGI.Reflex.Web.Models;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Servers.Controllers
{
    public class LandscapesControllerTest : BaseControllerTest<LandscapesController>
    {
        [Test]
        public void Index_should_return_view()
        {
            var result = Controller.Index(new LandscapesIndex());
            result.Should().BeDefaultView();
            result.Model<LandscapesIndex>().Should().NotBeNull();
            result.Model<LandscapesIndex>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Create_should_return_view()
        {
            var result = Controller.Create();
            result.Should().BeDefaultView();
            result.Model<LandscapesEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_Landscape()
        {
            var server1 = Factories.Server.Save(x => x.Landscape = null);
            var server2 = Factories.Server.Save(x => x.Landscape = null);

            var landscapeEdit = new LandscapesEdit
            {
                Name = Rand.String(),
                Description = Rand.String()
            };

            NHSession.Flush();
            NHSession.Clear();

            var result = Controller.Create(landscapeEdit, new[] { server1.Id, server2.Id });
            result.Should().BeRedirectToActionName("Index");

            NHSession.Flush();

            var landscape = NHSession.QueryOver<Landscape>().Where(x => x.Name == landscapeEdit.Name).SingleOrDefault();

            NHSession.Refresh(landscape);

            landscape.Should().NotBeNull();
            landscape.Name.Should().Be(landscapeEdit.Name);
            landscape.Description.Should().Be(landscapeEdit.Description);
            landscape.Servers.Count().Should().Be(2);
        }

         [Test]
        public void Edit_should_return_model_based_on_id()
        {
           var landscape = Factories.Landscape.Save();
   
           var result = Controller.Edit(landscape.Id);

           result.Should().BeDefaultView();
           result.Model<LandscapesEdit>().Id.Should().Be(landscape.Id);
           result.Model<LandscapesEdit>().FormAction.Should().Be("Edit");
        }

         [Test]
         public void Edit_should_update_Landscape()
         {
             var landscape = Factories.Landscape.Save();

             var server1 = Factories.Server.Save(x => x.Landscape = landscape);
             var server2 = Factories.Server.Save(x => x.Landscape = landscape);
             var server3 = Factories.Server.Save(x => x.Landscape = null);

             NHSession.Refresh(landscape);

             var landscapeEdit = new LandscapesEdit
             {
                 Id = landscape.Id,
                 ConcurrencyVersion = landscape.ConcurrencyVersion,
                 Name = Rand.String(),
                 Description = Rand.String()
             };

             var result = Controller.Edit(landscapeEdit, new[] { server1.Id, server3.Id });
             result.Should().BeRedirectToActionName("Index");

             NHSession.Flush();
             NHSession.Refresh(landscape);
             
             landscape.Name.Should().Be(landscapeEdit.Name);
             landscape.Description.Should().Be(landscapeEdit.Description);
             landscape.Servers.Count().Should().Be(2);
             landscape.Servers.Should().Contain(new[] { server1, server3 });
         }

         [Test]
         public void Edit_should_not_update_Info_when_concurrency_doesnt_match()
         {
             var landscape = Factories.Landscape.Save();

             var server1 = Factories.Server.Save(x => x.Landscape = landscape);

             var landscapeEdit = new LandscapesEdit
             {
                 Id = landscape.Id,
                 ConcurrencyVersion = landscape.ConcurrencyVersion - 1,
                 Name = Rand.String(),
                 Description = Rand.String()
             };

             StubStandardRequest();

             var result = Controller.Edit(landscapeEdit, new List<int>());

             NHSession.Refresh(landscape);

             result.Should().BeDefaultView();

             landscape.Name.Should().NotBe(landscapeEdit.Name);
             landscape.Description.Should().NotBe(landscapeEdit.Description);
             landscape.Servers.Count().Should().Be(1);
         }  

         [Test]
         public void Delete_should_return_view()
         {
             var landscape = Factories.Landscape.Save();

             Controller.Delete(landscape.Id + 1).Should().BeHttpNotFound();
             var result = Controller.Delete(landscape.Id);

             result.Should().BePartialView("_DeleteModal");
             NHSession.Get<Landscape>(landscape.Id).Should().NotBeNull();
         }

         [Test]
         public void Delete_should_delete_Landscape()
         {
             var landscape = Factories.Landscape.Save();

             var server1 = Factories.Server.Save(x => x.Landscape = landscape);
             var server2 = Factories.Server.Save(x => x.Landscape = landscape);

             NHSession.Refresh(landscape);

             Controller.Delete(new LandscapesEdit { Id = landscape.Id + 3 }).Should().BeHttpNotFound();
             var result = Controller.Delete(new LandscapesEdit { Id = landscape.Id });

             result.Should().BeRedirectToActionName("Index");
             NHSession.Get<Landscape>(landscape.Id).Should().BeNull();
         }

         [Test]
         public void GetServers_should_return_View()
         {
             var server1 = Factories.Server.Save();
             var server2 = Factories.Server.Save();
             var server3 = Factories.Server.Save();
             var server4 = Factories.Server.Save();
             var server5 = Factories.Server.Save();

             var results = Controller.GetServers(new[] { server2.Id, server3.Id, server4.Id, server5.Id });

             results.Should().BePartialView("_LandscapesServersDisplay");

             results.Model<LandscapesServersDisplay>().SelectedServer.Count().Should().Be(4);
         }
    }
}
