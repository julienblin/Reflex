using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models.Databases;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class DatabasesControllerTest : BaseControllerTest<DatabasesController>
    {
        [Test]
        public void Details_should_return_view()
        {
            var application = Factories.Application.Save();
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            application.AddDatabaseLinks(new[] {db1, db2});

            Controller.Details(Rand.String()).Should().BeHttpNotFound();

            var result = Controller.Details(application.Name);

            result.Should().BeDefaultView();
            result.Model<DatabasesList>().Should().NotBeNull();
            result.Model<DatabasesList>().AppName.Should().Be(application.Name);
            result.Model<DatabasesList>().Databases.Should().HaveCount(2);
        }

        [Test]
        public void Add_should_return_view()
        {
            var application = Factories.Application.Save();
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            application.AddDatabaseLink(db1);

            Controller.Add(Rand.String()).Should().BeHttpNotFound();

            var result = Controller.Add(application.Name);

            result.Should().BeDefaultView();
            result.Model<AddDatabase>().Should().NotBeNull();
            result.Model<AddDatabase>().AppName.Should().Be(application.Name);
            result.Model<AddDatabase>().ApplicationId.Should().Be(application.Id);
        }

        [Test]
        public void Delete_should_return_view()
        {
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            var app1 = Factories.Application.Save();
            app1.AddDatabaseLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDatabaseLinks(new[] { db1, db2 });

            Controller.Delete(Rand.String(), db2.Id).Should().BeHttpNotFound();
            Controller.Delete(app1.Name, db2.Id + 1).Should().BeHttpNotFound();

            var resultWithAssociation = Controller.Delete(app1.Name, db1.Id);
            resultWithAssociation.Should().BePartialView("_DeleteModal");
            resultWithAssociation.Model<DeleteDatabase>().Database.Should().Be(db1);
            resultWithAssociation.Model<DeleteDatabase>().WillDeleteDatabaseAlso.Should().BeFalse();

            var resultWithoutAssociation = Controller.Delete(app2.Name, db2.Id);
            resultWithoutAssociation.Should().BePartialView("_DeleteModal");
            resultWithoutAssociation.Model<DeleteDatabase>().Database.Should().Be(db2);
            resultWithoutAssociation.Model<DeleteDatabase>().WillDeleteDatabaseAlso.Should().BeTrue();
        }

        [Test]
        public void Delete_should_delete_association_and_database_if_nothing_linked()
        {
            var db1 = Factories.Database.Save();
            var db2 = Factories.Database.Save();
            var app1 = Factories.Application.Save();
            app1.AddDatabaseLink(db1);

            var app2 = Factories.Application.Save();
            app2.AddDatabaseLinks(new[] { db1, db2 });

            Controller.Delete(Rand.String(), new DeleteDatabase { Id = db2.Id }).Should().BeHttpNotFound();
            Controller.Delete(app1.Name, new DeleteDatabase { Id = db2.Id + 1 }).Should().BeHttpNotFound();

            var resultWithAssociation = Controller.Delete(app1.Name, new DeleteDatabase { Id = db1.Id });
            resultWithAssociation.Should().BeRedirectToActionName("Details");
            app1.DatabaseLinks.Should().BeEmpty();
            NHSession.Get<Database>(db1.Id).Should().NotBeNull();

            var resultWithoutAssociation = Controller.Delete(app2.Name, new DeleteDatabase { Id = db2.Id });
            resultWithoutAssociation.Should().BeRedirectToActionName("Details");
            app2.DatabaseLinks.Should().OnlyContain(dl => dl.Database == db1);
            NHSession.Get<Database>(db2.Id).Should().BeNull();
        }
    }
}
