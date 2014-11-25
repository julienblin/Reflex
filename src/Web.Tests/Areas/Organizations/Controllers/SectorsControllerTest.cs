// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Organizations.Controllers;
using CGI.Reflex.Web.Areas.Organizations.Models.Sectors;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Organizations.Controllers
{
    public class SectorsControllerTest : BaseControllerTest<SectorsController>
    {
        [Test]
        public void Index_should_return_view_with_sector_hierarchy()
        {
            var rootSector1 = Factories.Sector.Save();
            var rootSector2 = Factories.Sector.Save();

            var subSector1 = Factories.Sector.Save();
            rootSector1.AddChild(subSector1);

            var subSector11 = Factories.Sector.Save();
            subSector1.AddChild(subSector11);

            var result = Controller.Index();

            result.Should().BeDefaultView();
            result.Model<SectorHierarchy>().RootSectors.Should().HaveCount(2);
        }

        [Test]
        public void TreeData_should_return_tree_info()
        {
            var childSector = Factories.Sector.Save();
            var parentSector = Factories.Sector.Create();
            parentSector.AddChild(childSector);

            NHSession.Save(parentSector);

            var result = Controller.TreeData(parentSector.Id);
            result.Should().BeJsonResult();
            var jsonResultData = result.As<JsonResult>().Data as IEnumerable<dynamic>;
            jsonResultData.Should().HaveCount(1);
        }

        [Test]
        public void TreeData_should_return_empty_array_on_not_found()
        {
            var result = Controller.TreeData(0);
            result.Should().BeJsonResult();
            var jsonResultData = result.As<JsonResult>().Data as IEnumerable<dynamic>;
            jsonResultData.Should().HaveCount(0);
        }

        [Test]
        public void Create_should_create_sector()
        {
            var parentSector = Factories.Sector.Save();
            var name = Rand.String();

            Controller.Create(new SectorEdit { ParentId = parentSector.Id + 1, Name = Rand.String() }).Should().BeHttpNotFound();

            var model = new SectorEdit
            {
                ParentId = parentSector.Id,
                Name = name
            };

            var result = Controller.Create(model);
            result.Should().BeJsonResult();

            parentSector.Children.Should().HaveCount(1);
            parentSector.Children.First().Name.Should().Be(name);
        }

        [Test]
        public void Edit_should_update_sector()
        {
            var sector = Factories.Sector.Save();
            var newName = Rand.String();

            Controller.Edit(new SectorEdit { Id = sector.Id + 1, Name = Rand.String() }).Should().BeHttpNotFound();

            var model = new SectorEdit
            {
                Id = sector.Id,
                Name = newName
            };

            var result = Controller.Edit(model);
            result.Should().BeHttpStatusCodeResult(HttpStatusCode.OK);
            sector.Name.Should().Be(newName);
        }

        [Test]
        public void Delete_should_delete_sector()
        {
            var sector = Factories.Sector.Save();

            Controller.Delete(sector.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Delete(sector.Id);
            result.Should().BeHttpStatusCodeResult(HttpStatusCode.OK);
            NHSession.Get<Sector>(sector.Id).Should().BeNull();
        }

        [Test]
        public void Move_should_change_parent()
        {
            var sector = Factories.Sector.Save();
            var newParent = Factories.Sector.Save();

            Controller.Move(newParent.Id + 1, newParent.Id).Should().BeHttpNotFound();
            Controller.Move(sector.Id, newParent.Id + 1).Should().BeHttpNotFound();

            var result = Controller.Move(sector.Id, newParent.Id);
            
            result.Should().BeHttpStatusCodeResult(HttpStatusCode.OK);
            sector.Parent.Should().Be(newParent);
        }
    }
}
