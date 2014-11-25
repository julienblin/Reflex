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
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.DbInstances;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class DbInstancesControllerTest : BaseControllerTest<DbInstancesController>
    {
        [Test]
        public void Details_should_return_view()
        {
            Controller.Details(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();

            var application = Factories.Application.Save();
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            application.AddDbInstanceLinks(new[] { db1, db2 });

            var result = Controller.Details(application.Id);

            result.Should().BeDefaultView();
            result.Model<DbInstancesList>().Should().NotBeNull();
            result.Model<DbInstancesList>().AppName.Should().Be(application.Name);
            result.Model<DbInstancesList>().DbInstances.Should().HaveCount(2);
        }

        [Test]
        public void Add_should_return_view()
        {
            Controller.Add(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();

            var application = Factories.Application.Save();
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            application.AddDbInstanceLink(db1);

            var result = Controller.Add(application.Id);

            result.Should().BeDefaultView();
            result.Model<AddDbInstance>().Should().NotBeNull();
            result.Model<AddDbInstance>().AppName.Should().Be(application.Name);
            result.Model<AddDbInstance>().ApplicationId.Should().Be(application.Id);
        }

        [Test]
        public void Delete_should_return_view()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();

            Controller.Delete(Rand.Int(0), db2.Id).Should().BeHttpNotFound();

            var app1 = Factories.Application.Save();
            Controller.Delete(app1.Id, db2.Id + 1).Should().BeHttpNotFound();

            app1.AddDbInstanceLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLinks(new[] { db1, db2 });

            var resultWithAssociation = Controller.Delete(app1.Id, db1.Id);
            resultWithAssociation.Should().BePartialView("_DeleteModal");
            resultWithAssociation.Model<DeleteDbInstance>().DbInstances.Should().Be(db1);

            var resultWithoutAssociation = Controller.Delete(app2.Id, db2.Id);
            resultWithoutAssociation.Should().BePartialView("_DeleteModal");
            resultWithoutAssociation.Model<DeleteDbInstance>().DbInstances.Should().Be(db2);
        }

        [Test]
        public void Delete_should_delete_association_and_dbInstance_if_nothing_linked()
        {
            var db1 = Factories.DbInstance.Save();
            var db2 = Factories.DbInstance.Save();
            Controller.Delete(Rand.Int(int.MaxValue), new DeleteDbInstance { Id = db2.Id }).Should().BeHttpNotFound();

            var app1 = Factories.Application.Save();
            Controller.Delete(app1.Id, new DeleteDbInstance { Id = db2.Id + 1 }).Should().BeHttpNotFound();

            app1.AddDbInstanceLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDbInstanceLinks(new[] { db1, db2 });

            var resultWithAssociation = Controller.Delete(app1.Id, new DeleteDbInstance { Id = db1.Id });
            resultWithAssociation.Should().BeRedirectToActionName("Details");
            app1.DbInstanceLinks.Should().BeEmpty();
           
            var resultWithoutAssociation = Controller.Delete(app2.Id, new DeleteDbInstance { Id = db2.Id });
            resultWithoutAssociation.Should().BeRedirectToActionName("Details");
            app2.DbInstanceLinks.Should().OnlyContain(dl => dl.DbInstances == db1);
        }
    }
}
