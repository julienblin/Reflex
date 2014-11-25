// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbInstancesController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.DbInstances;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class DbInstancesController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/DbInstances")]
        public ActionResult Details(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.DbInstanceLinks).Eager
                                       .Fetch(a => a.DbInstanceLinks.First().DbInstances).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var model = new DbInstancesList
            {
                AppName = application.Name,
                AppId = application.Id, 
                DbInstances = new DbInstanceQuery { AssociatedWithApplicationId = application.Id }.OrderBy("Name").List().ToList()
            };

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/DbInstances/Add")]
        public ActionResult Add(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var model = new AddDbInstance
            {
                AppName = application.Name,
                ApplicationId = application.Id
            };

            return View(model);
        }

        [HttpPost]
        [IsAllowed("/Applications/DbInstances/Add")]
        public ActionResult Add(int appId, IEnumerable<int> dbInstances)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ServerLinks).Eager
                                       .Fetch(a => a.DbInstanceLinks).Eager
                                       .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            ViewBag.appName = application.Name;

            var numberOfDbInstancesAdded = 0;
            var serversAdded = new List<Server>();

            if (dbInstances != null)
            {
                foreach (var dbId in dbInstances)
                {
                    var dbInstance = NHSession.Load<DbInstance>(dbId);
                    if (application.AddDbInstanceLink(dbInstance))
                    {
                        ++numberOfDbInstancesAdded;
                        if (application.AddServerLink(dbInstance.Server))
                            serversAdded.Add(dbInstance.Server);
                    }
                }
            }

            if (numberOfDbInstancesAdded == 0)
                Flash(FlashLevel.Warning, string.Format("Aucune instance ajoutée à l'application {0}.", application.Name));

            if (numberOfDbInstancesAdded == 1)
                Flash(FlashLevel.Success, string.Format("Une instance a été ajoutée à l'application {0}.", application.Name));

            if (numberOfDbInstancesAdded > 1)
                Flash(FlashLevel.Success, string.Format("{0} instances ont été ajoutées à l'application {1}.", numberOfDbInstancesAdded, application.Name));

            foreach (var server in serversAdded)
                Flash(FlashLevel.Success, string.Format("Une association avec le serveur {0} a été ajoutée à l'application {1}.", server.Name, application.Name));

            return RedirectToAction("Details", new { appId = application.Id });
        }

        [IsAllowed("/Applications/DbInstances/Remove")]
        public ActionResult Delete(int appId, int id)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var dbInstance = NHSession.Get<DbInstance>(id);
            if (dbInstance == null)
                return HttpNotFound();

            var model = new DeleteDbInstance
            {
                Id = dbInstance.Id, 
                DbInstances = dbInstance
            };
            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [IsAllowed("/Applications/DbInstances/Remove")]
        public ActionResult Delete(int appId, DeleteDbInstance model)
        {
            var application = NHSession.Get<Application>(appId);
            if (application == null)
                return HttpNotFound();

            var dbInstance = NHSession.Get<DbInstance>(model.Id);
            if (dbInstance == null)
                return HttpNotFound();

            application.RemoveDbInstanceLink(dbInstance);
            Flash(FlashLevel.Success, string.Format("L'association avec l'instance {0} a bien été supprimée.", dbInstance.Name));

            return RedirectToAction("Details");
        }
    }
}
