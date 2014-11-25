// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Info;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class InfoController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Info")]
        public ActionResult Details(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.AppInfo).Eager
                                       .Fetch(a => a.AppInfo.Category).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var model = new InfoEdit();
            BindToModel(application, model);

            model.CategoryDescription = application.AppInfo.Category != null ? application.AppInfo.Category.Description : string.Empty;

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Info/Update")]
        public ActionResult Edit(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.AppInfo).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var model = new InfoEdit();
            BindToModel(application, model);
            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Info/Update")]
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(int appId, InfoEdit model)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.AppInfo).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            if (application.AppInfo.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>L'application {0} a été modifiée par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs de l'application et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(application.Name),
                        Url.Action("Edit", new { appId = application.Id })),
                    disableHtmlEscaping: true);
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            BindFromModel(model, application);
            Flash(FlashLevel.Success, string.Format("L'application {0} a bien été mise à jour.", application.Name));
            return RedirectToAction("Details", new { appId = application.Id });
        }

        private void BindToModel(Application application, InfoEdit model)
        {
            model.Id = application.AppInfo.Id;
            model.ConcurrencyVersion = application.AppInfo.ConcurrencyVersion;

            model.Name = application.Name;
            model.Code = application.Code;
            model.TypeId = application.ApplicationType.ToId();

            model.StatusId = application.AppInfo.Status.ToId();
            model.CriticityId = application.AppInfo.Criticity.ToId();
            model.Description = application.AppInfo.Description;
            model.UserRange = application.AppInfo.UserRange.ToId();
            model.Category = application.AppInfo.Category.ToId();
            model.Certification = application.AppInfo.Certification.ToId();
            model.MaintenanceWindow = application.AppInfo.MaintenanceWindow;
            model.Notes = application.AppInfo.Notes;
            model.SectorId = application.AppInfo.Sector.ToId();
        }

        private void BindFromModel(InfoEdit model, Application application)
        {
            application.Name = model.Name;
            application.Code = model.Code;
            application.ApplicationType = model.TypeId.ToDomainValue();

            application.AppInfo.Status = model.StatusId.ToDomainValue();
            application.AppInfo.Criticity = model.CriticityId.ToDomainValue();
            application.AppInfo.Description = model.Description;
            application.AppInfo.UserRange = model.UserRange.ToDomainValue();
            application.AppInfo.Category = model.Category.ToDomainValue();
            application.AppInfo.Certification = model.Certification.ToDomainValue();
            application.AppInfo.MaintenanceWindow = model.MaintenanceWindow;
            application.AppInfo.Notes = model.Notes;
            application.AppInfo.Sector = model.SectorId.HasValue ? NHSession.Load<Sector>(model.SectorId.Value) : null;
        }
    }
}
