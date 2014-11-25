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
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Technologies;
using CGI.Reflex.Web.Binders;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class TechnologiesController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Technologies")]
        public ActionResult Details(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var appTechnoResult = new AppTechnoQuery { ApplicationId = application.Id }.Execute();

            var model = new TechnologiesList
            {
                AppName = application.Name, 
                AppId = application.Id, 
                ApplicationTechnologies = appTechnoResult.ApplicationTechnologies.OrderBy(t => t.FullName), 
                IntegrationTechnologies = appTechnoResult.IntegrationTechnologies.OrderBy(t => t.FullName), 
                ServerTechnologies = appTechnoResult.ServerTechnologies.OrderBy(t => t.FullName), 
                DbInstanceTechnologies = appTechnoResult.DbInstanceTechnologies.OrderBy(t => t.FullName)
            };

            return View(model);
        }

        [IsAllowed("/Applications/Technologies/Add")]
        [HttpPost]
        public ActionResult Add(int appId, CheckedTechnologies checkedTechnologies)
        {
            var application = NHSession.QueryOver<Application>()
                .Where(a => a.Id == appId)
                .Fetch(a => a.TechnologyLinks).Eager
                .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var numberOfTechnologiesAdded = 0;
            if ((checkedTechnologies.TechnologyIds != null) && (checkedTechnologies.TechnologyIds.Count() > 0))
            {
                var technologies = new TechnologyHierarchicalQuery { RootIds = checkedTechnologies.TechnologyIds }.List();
                numberOfTechnologiesAdded += technologies.Where(technology => !technology.HasChildren()).Count(application.AddTechnologyLink);
            }

            if (numberOfTechnologiesAdded == 0)
                Flash(FlashLevel.Warning, string.Format("Aucune technologie ajoutée à l'application {0}.", application.Name));

            if (numberOfTechnologiesAdded == 1)
                Flash(FlashLevel.Success, string.Format("Une technologie a été ajoutée à l'application {0}.", application.Name));

            if (numberOfTechnologiesAdded > 1)
                Flash(FlashLevel.Success, string.Format("{0} technologies ont été ajoutées à l'application {1}.", numberOfTechnologiesAdded, application.Name));

            return RedirectToAction("Details");
        }

        [IsAllowed("/Applications/Technologies/Remove")]
        public ActionResult Delete(int appId, int technoId)
        {
            var application = NHSession.Get<Application>(appId);

            if (application == null)
                return HttpNotFound();

            var techno = NHSession.Get<Technology>(technoId);
            if (techno == null)
                return HttpNotFound();

            var model = new TechnologyDelete
            {
                TechnologyId = techno.Id, 
                TechnologyName = techno.FullName
            };

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Applications/Technologies/Remove")]
        [HttpPost]
        public ActionResult Delete(int appId, TechnologyDelete technoDelete)
        {
            var application = NHSession.QueryOver<Application>()
                .Where(a => a.Id == appId)
                .Fetch(a => a.TechnologyLinks).Eager
                .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var techno = NHSession.Load<Technology>(technoDelete.TechnologyId);

            application.RemoveTechnologyLink(techno);

            Flash(FlashLevel.Success, string.Format("L'assocation avec la technologie {0} a bien été supprimée.", techno.FullName));
            return RedirectToAction("Details");
        }
    }
}