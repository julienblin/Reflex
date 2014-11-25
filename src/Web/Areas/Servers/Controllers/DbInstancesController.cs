// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Servers.Filters;
using CGI.Reflex.Web.Areas.Servers.Models.DbInstances;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Servers.Controllers
{
    public class DbInstancesController : BaseController
    {
        [ServerHeader]
        [IsAllowed("/Servers/DbInstances")]
        public ActionResult Details(int serverId, DbInstanceDetails model)
        {
            var server = NHSession.Get<Server>(serverId);
            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            model.ServerName = server.Name;
            model.ServerId = server.Id;
            model.SearchResults = new DbInstanceQuery
            {
                NameLike = model.DbInstanceName, 
                ServerId = server.Id
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .Paginate(model.Page);

            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/DbInstances/Create")]
        public ActionResult Create(int serverId)
        {
            var model = new DbInstanceEdit();
            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/DbInstances/Create")]
        [HttpPost]
        public ActionResult Create(int serverId, DbInstanceEdit model)
        {
            Server server = NHSession.QueryOver<Server>()
                                              .Where(s => s.Id == serverId)
                                              .Fetch(s => s.DbInstances).Eager
                                              .SingleOrDefault();
            if (server == null)
                return HttpNotFound();

            var dbInstanceNames = server.DbInstances.Select(db => db.Name);
            if (dbInstanceNames.Count(n => n.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase)) > 0)
                ModelState.AddModelError("Name", "Ce nom est déjà utilisé.");

            if (!ModelState.IsValid)
                return View(model);

            var dbInstance = new DbInstance { Server = server };
            BindFromModel(model, dbInstance);

            NHSession.Save(dbInstance);

            Flash(FlashLevel.Success, string.Format("L'instance {0} a été créée avec succès.", dbInstance.Name));

            return RedirectToAction("Details");
        }

        [ServerHeader]
        [IsAllowed("/Servers/DbInstances/Update")]
        public ActionResult Edit(int id)
        {
            var dbInstance = NHSession.Get<DbInstance>(id);
            if (dbInstance == null)
                return HttpNotFound();

            var model = new DbInstanceEdit();
            BindToModel(dbInstance, model);

            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/DbInstances/Update")]
        [HttpPost]
        public ActionResult Edit(int id, DbInstanceEdit model)
        {
            var dbInstance = NHSession.QueryOver<DbInstance>()
                                      .Where(db => db.Id == id)
                                      .Fetch(db => db.Server).Eager
                                      .Fetch(db => db.Server.DbInstances).Eager
                                      .SingleOrDefault();
            if (dbInstance == null)
                return HttpNotFound();

            var dbInstanceNames = dbInstance.Server.DbInstances.Where(db => db.Id != id).Select(db => db.Name);
            if (dbInstanceNames.Count(n => n.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase)) > 0)
                ModelState.AddModelError("Name", "Ce nom est déjà utilisé.");

            if (!ModelState.IsValid)
                return View(model);

            BindFromModel(model, dbInstance);

            Flash(FlashLevel.Success, "L'instance été modifié avec succès.");

            return RedirectToAction("Details");
        }

        [IsAllowed("/Servers/DbInstances/Delete")]
        public ActionResult Delete(int id)
        {
            var dbInstance = NHSession.Get<DbInstance>(id);
            if (dbInstance == null)
                return HttpNotFound();

            var model = new DbInstanceEdit();
            BindToModel(dbInstance, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Servers/DbInstances/Delete")]
        [HttpPost]
        public ActionResult Delete(DbInstanceEdit model)
        {
            var dbInstance = NHSession.Get<DbInstance>(model.Id);
            if (dbInstance == null)
                return HttpNotFound();

            var attachedToApp = new ApplicationQuery { LinkedDbInstanceId = dbInstance.Id }.Count();

            if (attachedToApp > 0)
            {
                Flash(FlashLevel.Error, string.Format("Impossible de supprimer l'instance {0} : il existe encore des applications liés avec cette instance .", dbInstance.Name));
                return RedirectToAction("Details", new { id = dbInstance.Id });
            }

            NHSession.Delete(dbInstance);

            Flash(FlashLevel.Success, string.Format("L'instance {0} a bien été supprimée.", dbInstance.Name));

            return RedirectToAction("Details");
        }

        [IsAllowed("/Servers/DbInstances")]
        public ActionResult RenderTechno(IEnumerable<int> ids)
        {
            var technos = new TechnologyHierarchicalQuery { RootIds = ids }.FetchParents().List();
            return PartialView("_Technologies", technos);
        }

        protected void BindToModel(DbInstance dbInstance, DbInstanceEdit model)
        {
            model.Id = dbInstance.Id;
            model.Name = dbInstance.Name;

            var technoLink = dbInstance.TechnologyLinks.FirstOrDefault();

            if (technoLink != null)
                model.TechnologyId = technoLink.Technology.Id;
            else
                model.TechnologyId = null;
        }

        private void BindFromModel(DbInstanceEdit model, DbInstance dbInstance)
        {
            dbInstance.Name = model.Name;

            var technoList = new List<Technology>();

            if (model.TechnologyId.HasValue)
                technoList.Add(NHSession.Load<Technology>(model.TechnologyId.Value));

            dbInstance.SetTechnologyLinks(technoList);
        }
    }
}
