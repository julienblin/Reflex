// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesControllerTest.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Models.Technologies;
using CGI.Reflex.Web.Binders;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Servers.Controllers
{
    public class TechnologiesControllerTest : BaseControllerTest<TechnologiesController>
    {
        [Test]
        public void Details_should_return_view()
        {
            Controller.Details(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();

            var server = Factories.Server.Save();
            server.AddTechnologyLink(Factories.Technology.Save());
            server.AddTechnologyLink(Factories.Technology.Save());

            var result = Controller.Details(server.Id);
            result.Should().BeDefaultView();
            result.Model<TechnologiesList>().ServerName.Should().Be(server.Name);
            result.Model<TechnologiesList>().ServerTechnologies.Should().HaveCount(2);
        }

        [Test]
        public void Add_should_add_children_technologies()
        {
            Controller.Add(Rand.Int(int.MaxValue), new CheckedTechnologies()).Should().BeHttpNotFound();

            var server = Factories.Server.Save();
            var techno1 = Factories.Technology.Save();
            techno1.AddChild(Factories.Technology.Save());
            var techno2 = Factories.Technology.Save();

            var result = Controller.Add(server.Id, new CheckedTechnologies { TechnologyIds = new[] { techno1.Id, techno2.Id } });
            result.Should().BeRedirectToActionName("Details");

            server.TechnologyLinks.Should().OnlyContain(atl => atl.Technology == techno2);
        }

        [Test]
        public void Delete_should_return_modal()
        {
            var techno = Factories.Technology.Save();
            Controller.Delete(Rand.Int(int.MaxValue), techno.Id).Should().BeHttpNotFound();

            var server = Factories.Server.Save();
            server.AddTechnologyLink(techno);
            Controller.Delete(server.Id, techno.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Delete(server.Id, techno.Id);
            result.Should().BePartialView("_DeleteModal");
            result.Model<TechnologyDelete>().TechnologyId.Should().Be(techno.Id);
            result.Model<TechnologyDelete>().TechnologyName.Should().Be(techno.Name);
        }

        [Test]
        public void Delete_should_delete_association()
        {
            var techno = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();

            Controller.Delete(Rand.Int(int.MaxValue), new TechnologyDelete { TechnologyId = techno.Id }).Should().BeHttpNotFound();
            
            var server = Factories.Server.Save();
            server.AddTechnologyLink(techno);
            server.AddTechnologyLink(techno2);

            var result = Controller.Delete(server.Id, new TechnologyDelete { TechnologyId = techno.Id });
            result.Should().BeRedirectToActionName("Details");

            server.TechnologyLinks.Should().HaveCount(1);
        }
    }
}
