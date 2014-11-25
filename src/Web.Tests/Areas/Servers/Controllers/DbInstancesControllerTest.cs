// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesControllerTest.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Controllers;
using CGI.Reflex.Web.Areas.Servers.Models.DbInstances;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Servers.Controllers
{
    public class DbInstancesControllerTest : BaseControllerTest<DbInstancesController>
    {
        [Test]
        public void Details_should_return_view()
        {
            Controller.Details(Rand.Int(int.MaxValue), new DbInstanceDetails()).Should().BeHttpNotFound();

            var server = Factories.Server.Save();
            StubAjaxRequest();

            var result = Controller.Details(server.Id, new DbInstanceDetails());

            result.Should().BeDefaultView();
            result.Model<DbInstanceDetails>().SearchDefined.Should().BeFalse();
        }

        [Test]
        public void Create_should_return_view()
        {
            var server = Factories.Server.Save();

            var result = Controller.Create(server.Id);
            result.Should().BeDefaultView();
            result.Model<DbInstanceEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_instance()
        {
            var server = Factories.Server.Save();
            var techno1 = Factories.Technology.Save();

            var dbInstanceEdit = new DbInstanceEdit
            {
                Name = Rand.String(),
                TechnologyId = techno1.Id
            };

            StubStandardRequest();
            Controller.Create(server.Id, dbInstanceEdit);

            var instance = NHSession.QueryOver<DbInstance>().SingleOrDefault();
            instance.Should().NotBeNull();
            instance.Name.Should().Be(dbInstanceEdit.Name);
            instance.TechnologyLinks.Should().Contain(tl => tl.Technology.Id == dbInstanceEdit.TechnologyId);
        }

        [Test]
        public void Edit_should_return_model_based_on_id()
        {
            // Arrange
            var instance = Factories.DbInstance.Save();

            NHSession.Flush();

            // Act          
            var result = Controller.Edit(instance.Id);

            // Assert
            result.Should().BeDefaultView();
            result.Model<DbInstanceEdit>().Should().NotBeNull();
            result.Model<DbInstanceEdit>().Id.Should().Be(instance.Id);
            result.Model<DbInstanceEdit>().Name.Should().Be(instance.Name);
            result.Model<DbInstanceEdit>().FormAction.Should().Be("Edit");
        }

        [Test]
        public void Edit_should_return_model_based_on_instance()
        {
            // Arrange
            var instance = Factories.DbInstance.Save();
            var techno1 = Factories.Technology.Save();

            NHSession.Refresh(instance);

            var dbInstanceEdit = new DbInstanceEdit
            {
                Id = instance.Id,
                Name = Rand.String(),
                TechnologyId = techno1.Id
            };

            // Act        
            Controller.Edit(instance.Id, dbInstanceEdit);

            // Assert
            instance.Name.Should().Be(dbInstanceEdit.Name);
            instance.TechnologyLinks.Should().OnlyContain(tl => tl.Technology.Id == dbInstanceEdit.TechnologyId);
        }

        [Test]
        public void Delete_based_on_id()
        {
            var instance = Factories.DbInstance.Save();
            var result = Controller.Delete(instance.Id);

            result.Should().BePartialView("_DeleteModal");
        }

        [Test]
        public void Delete_based_on_instance()
        {
            var instance = Factories.DbInstance.Save();

            var dbInstanceEdit = new DbInstanceEdit
            {
                Id = instance.Id,
            };

            var result = Controller.Delete(dbInstanceEdit);
            result.Should().BeOfType<RedirectToRouteResult>();
            NHSession.Get<DbInstance>(instance.Id).Should().BeNull();
        }
    }
}
