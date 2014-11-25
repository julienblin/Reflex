// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Models.Technologies;
using CGI.Reflex.Web.Binders;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Servers.Controllers
{
    public class TechnologiesController : BaseController
    {
        [ServerHeader]
        [IsAllowed("/Servers/Technologies")]
        public ActionResult Details(int serverId)
        {
            var server = NHSession.QueryOver<Server>()
                        .Where(a => a.Id == serverId)
                        .SingleOrDefault();

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            var serverTechnoResult = new ServerTechnoQuery { ServerId = server.Id }.Execute();

            var model = new TechnologiesList
            {
                ServerName = server.Name, 
                ServerId = server.Id, 
                ServerTechnologies = serverTechnoResult.ServersTechnologies.OrderBy(t => t.FullName), 
                ApplicationTechnologies = serverTechnoResult.ApplicationTechnologies.OrderBy(t => t.Name), 
                DbInstanceTechnologies = serverTechnoResult.DbInstanceTechnologies.OrderBy(t => t.Name)
            };

            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/Technologies/Add")]
        [HttpPost]
        public ActionResult Add(int serverId, CheckedTechnologies checkedTechnologies)
        {
            var server = NHSession.QueryOver<Server>()
                        .Where(a => a.Id == serverId)
                        .Fetch(a => a.TechnologyLinks).Eager
                        .SingleOrDefault();

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;

            var numberOfTechnologiesAdded = 0;
            if ((checkedTechnologies.TechnologyIds != null) && (checkedTechnologies.TechnologyIds.Count() > 0))
            {
                var technologies = new TechnologyHierarchicalQuery { RootIds = checkedTechnologies.TechnologyIds }.List();
                numberOfTechnologiesAdded += technologies.Where(technology => !technology.HasChildren()).Count(server.AddTechnologyLink);
            }

            if (numberOfTechnologiesAdded == 0)
                Flash(FlashLevel.Warning, string.Format("Aucune technologie ajoutée au serveur {0}.", server.Name));

            if (numberOfTechnologiesAdded == 1)
                Flash(FlashLevel.Success, string.Format("Une technologie a été ajoutée au serveur {0}.", server.Name));

            if (numberOfTechnologiesAdded > 1)
                Flash(FlashLevel.Success, string.Format("{0} technologies ont été ajoutées au serveur {1}.", numberOfTechnologiesAdded, server.Name));

            return RedirectToAction("Details", new { serverId = server.Id });
        }

        [ServerHeader]
        [IsAllowed("/Servers/Technologies/Remove")]
        public ActionResult Delete(int serverId, int technoId)
        {
            var server = NHSession.Get<Server>(serverId);

            if (server == null)
                return HttpNotFound();

            var techno = NHSession.Get<Technology>(technoId);
            if (techno == null)
                return HttpNotFound();

            var model = new TechnologyDelete
            {
                ServerId = server.Id,
                TechnologyId = techno.Id, 
                TechnologyName = techno.FullName
            };

            return PartialView("_DeleteModal", model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/Technologies/Remove")]
        [HttpPost]
        public ActionResult Delete(int serverId, TechnologyDelete technoDelete)
        {
            var server = NHSession.QueryOver<Server>()
                       .Where(a => a.Id == serverId)
                       .Fetch(a => a.TechnologyLinks).Eager
                       .SingleOrDefault();

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;

            var techno = NHSession.Load<Technology>(technoDelete.TechnologyId);

            server.RemoveTechnologyLink(techno);

            Flash(FlashLevel.Success, string.Format("L'assocation avec la technologie {0} a bien été supprimée.", techno.FullName));
            return RedirectToAction("Details", "Technologies", new { serverId = server.Id });
        }
    }
}