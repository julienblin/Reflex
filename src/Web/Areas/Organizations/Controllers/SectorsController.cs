// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Organizations.Models.Sectors;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Organizations.Controllers
{
    public class SectorsController : BaseController
    {
        [IsAllowed("/Organizations/Sectors")]
        public ActionResult Index()
        {
            var model = new SectorHierarchy
            {
                RootSectors = new SectorHierarchicalQuery { NumberOfEagerLoadingLevels = 2 }.List()
            };
            return View(model);
        }

        [IsAllowed("/Organizations/Sectors")]
        public ActionResult TreeData(int rootId)
        {
            var sector = new SectorHierarchicalQuery { RootId = rootId, NumberOfEagerLoadingLevels = 2 }.SingleOrDefault();

            if (sector == null)
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            return Json(
                sector.Children.Select(s => new
                {
                    attr = new { id = string.Format("sector-{0}", s.Id) }, 
                    metadata = new { id = s.Id }, 
                    data = new { title = s.Name }, 
                    state = s.HasChildren() ? "closed" : string.Empty
                }).ToList(), 
                JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Organizations/Sectors")]
        public ActionResult Search(string name)
        {
            var sectors = NHSession.QueryOver<Sector>()
                                   .Where(Restrictions.InsensitiveLike(Projections.Property<Sector>(s => s.Name), name, MatchMode.Anywhere))
                                   .Fetch(t => t.Parent).Eager
                                   .Fetch(t => t.Parent.Parent).Eager
                                   .Fetch(t => t.Parent.Parent.Parent).Eager
                                   .Fetch(t => t.Parent.Parent.Parent.Parent).Eager
                                   .List().OrderBy(t => t.HierarchicalLevel);

            return Json(sectors.SelectMany(t => t.AllParentIds).Distinct().Select(i => string.Format("#sector-{0}", i)).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Sectors/Create")]
        public ActionResult Create(SectorEdit model)
        {
            Sector parent = null;
            if (model.ParentId != 0)
            {
                parent = NHSession.Get<Sector>(model.ParentId);
                if (parent == null)
                    return HttpNotFound();
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotAcceptable);

            var sector = new Sector { Name = model.Name };

            if (parent != null)
                parent.AddChild(sector);

            NHSession.Save(sector);

            return Json(new { id = sector.Id });
        }

        [HttpPost]
        [IsAllowed("/Organizations/Sectors/Update")]
        public ActionResult Edit(SectorEdit model)
        {
            var sector = NHSession.Get<Sector>(model.Id);
            if (sector == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult((int)HttpStatusCode.NotAcceptable);

            sector.Name = model.Name;

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Sectors/Delete")]
        public ActionResult Delete(int id)
        {
            var sector = NHSession.Get<Sector>(id);
            if (sector == null)
                return HttpNotFound();

            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(sector);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    return new HttpStatusCodeResult((int)HttpStatusCode.NotAcceptable);
                }
            }

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Sectors/Update")]
        public ActionResult Move(int id, int parentId)
        {
            var sector = NHSession.Get<Sector>(id);
            if (sector == null)
                return HttpNotFound();

            Sector parentSector = null;
            if (parentId != 0)
            {
                parentSector = NHSession.Get<Sector>(parentId);
                if (parentSector == null)
                    return HttpNotFound();
            }

            sector.Parent = parentSector;

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
    }
}
