// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models;
using CGI.Reflex.Web.Areas.Applications.Models.Integrations;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class IntegrationsController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Integrations")]
        public ActionResult Details(int appId, IntegrationsDetails model)
        {
            var application = NHSession.QueryOver<Application>()
                                               .Where(a => a.Id == appId)
                                               .Fetch(a => a.ApplicationType).Eager
                                               .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "AppSource.Name";
                model.OrderType = OrderType.Asc;
            }

            model.AppName = application.Name;
            model.SearchResults = new IntegrationQuery
                                      {
                                          AppSourceId = model.AppSourceId, 
                                          AppDestId = model.AppDestId, 
                                          NatureId = model.NatureId, 
                                          NameLike = model.IntegrationName, 
                                          ApplicationId = application.Id, 
                                          LinkedTechnologyId = model.Technology
                                      }
                                      .OrderBy(model.OrderBy, model.OrderType)
                                      .Paginate(model.Page);

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Integrations/Create")]
        public ActionResult Create(int appId)
        {
            var model = new IntegrationEdit { AppSourceId = appId };
            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Integrations/Create")]
        [HttpPost]
        public ActionResult Create(int appId, IntegrationEdit model)
        {
            var currentApp = NHSession.QueryOver<Application>()
                                              .Where(a => a.Id == appId)
                                              .Fetch(a => a.ApplicationType).Eager
                                              .SingleOrDefault();
            
            model.Technologies = model.TechnoIds.Select(technoId => NHSession.Load<Technology>(technoId)).ToList();

            if (currentApp.Id != model.AppSourceId && currentApp.Id != model.AppDestId)
            {
                ModelState.AddModelError("AppSourceId", string.Format("L'application source ou de destination doit être l'application courante ({0}).", currentApp.Name));
                ModelState.AddModelError("AppDestId", string.Empty);
            }

            if (!ModelState.IsValid)
                return View(model);

            var integration = new Integration();

            BindFromModel(model, integration);

            NHSession.Save(integration);

            Flash(FlashLevel.Success, string.Format("L'intégration {0} a été créée avec succès.", integration.Name));

            return RedirectToAction("Details");
        }

        [AppHeader]
        [IsAllowed("/Applications/Integrations/Update")]
        public ActionResult Edit(int id)
        {
            var integration = NHSession.Get<Integration>(id);
            if (integration == null)
                return HttpNotFound();

            var model = new IntegrationEdit();
            BindToModel(integration, model);

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Integrations/Update")]
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(int appId, int id, IntegrationEdit model)
        {
            Application currentApp = NHSession.QueryOver<Application>()
                                              .Where(a => a.Id == appId)
                                              .Fetch(a => a.ApplicationType).Eager
                                              .SingleOrDefault();
            
            model.Technologies = model.TechnoIds.Select(technoId => NHSession.Load<Technology>(technoId)).ToList();

            var integration = NHSession.QueryOver<Integration>()
                                        .Where(i => i.Id == id)
                                        .Fetch(i => i.AppSource).Eager
                                        .Fetch(i => i.AppDest).Eager
                                        .SingleOrDefault();
            if (integration == null)
                return HttpNotFound();

            if (currentApp.Id != model.AppSourceId && currentApp.Id != model.AppDestId)
            {
                ModelState.AddModelError("AppSourceId", string.Format("L'application source ou de destination doit être l'application courante ({0}).", currentApp.Name));
                ModelState.AddModelError("AppDestId", string.Empty);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (integration.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>L'intégration {0} a été modifiée par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(integration.Name),
                        Url.Action("Edit", new { id = integration.Id })), 
                    disableHtmlEscaping: true);
                return View(model);
            }

            BindFromModel(model, integration);

            Flash(FlashLevel.Success, string.Format("L'intégration {0} a été modifiée avec succès.", integration.Name));

            return RedirectToAction("Details");
        }

        [IsAllowed("/Applications/Integrations/Delete")]
        public ActionResult Delete(int id)
        {
            var integration = NHSession.Get<Integration>(id);
            if (integration == null)
                return HttpNotFound();

            var model = new IntegrationEdit();
            BindToModel(integration, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Applications/Integrations/Delete")]
        [HttpPost]
        public ActionResult Delete(IntegrationEdit model)
        {
            var integration = NHSession.Get<Integration>(model.Id);
            if (integration == null)
                return HttpNotFound();

            // Using sub-session / sub-transaction to catch ConstraintViolationException
            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(integration);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    Flash(FlashLevel.Error, string.Format("Impossible de supprimer l'intégration {0} parce que certaines entités sont encore reliées.", integration.Name));
                    return RedirectToAction("Details");
                }
            }

            Flash(FlashLevel.Success, string.Format("L'intégration {0} a bien été supprimée.", integration.Name));
            return RedirectToAction("Details");
        }

        [IsAllowed("/Technologies")]
        public ActionResult RenderTechno(IEnumerable<int> ids)
        {
            var technos = new TechnologyHierarchicalQuery { RootIds = ids }.FetchParents().List();
            return PartialView("_Technologies", technos);
        }

        protected void BindToModel(Integration integration, IntegrationEdit model)
        {
            model.Id = integration.Id;
            model.ConcurrencyVersion = integration.ConcurrencyVersion;

            model.AppSourceId = integration.AppSource.Id;
            model.AppDestId = integration.AppDest.Id;
            model.Name = integration.Name;
            model.NatureId = integration.Nature.Id;
            model.Description = integration.Description;
            model.DataDescription = integration.DataDescription;
            model.Frequency = integration.Frequency;
            model.Comments = integration.Comments;

            model.Technologies = integration.TechnologyLinks.Select(tl => tl.Technology).ToList();
        }

        private void BindFromModel(IntegrationEdit model, Integration integration)
        {
            integration.AppSource = NHSession.Load<Application>(model.AppSourceId);
            integration.AppDest = NHSession.Load<Application>(model.AppDestId);
            integration.Name = model.Name;
            integration.Nature = NHSession.Load<DomainValue>(model.NatureId);
            integration.Description = model.Description;
            integration.DataDescription = model.DataDescription;
            integration.Frequency = model.Frequency;
            integration.Comments = model.Comments;

            integration.SetTechnologyLinks(model.Technologies);
        }
    }
}
