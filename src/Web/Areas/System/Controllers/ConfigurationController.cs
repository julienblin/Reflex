// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.Configuration;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class ConfigurationController : BaseController
    {
        [IsAllowed("/System/Configuration")]
        public ActionResult Index()
        {
            var currentConfig = ReflexConfiguration.GetCurrent();
            var model = new ReflexConfig();
            BindToModel(currentConfig, model);
            model.ApplicationStatuses = new DomainValueQuery { Category = DomainValueCategory.ApplicationStatus }.List();
            return View(model);
        }

        [IsAllowed("/System/Configuration")]
        [HttpPost]
        public ActionResult Index(ReflexConfig model)
        {
            if (!ModelState.IsValid)
            {
                model.ApplicationStatuses = new DomainValueQuery { Category = DomainValueCategory.ApplicationStatus }.List();
                return View(model);
            }

            var config = ReflexConfiguration.GetCurrent();
            NHSession.SetReadOnly(config, false);
            BindFromModel(model, config);
            
            Flash(FlashLevel.Success, "La configuration a bien été sauvegardée.");
            return RedirectToAction("Index");
        }

        private void BindFromModel(ReflexConfig model, ReflexConfiguration config)
        {
            config.SetActiveAppStatusDVIds(model.ActiveAppStatusDVIds);
        }

        private void BindToModel(ReflexConfiguration config, ReflexConfig model)
        {
            model.ActiveAppStatusDVIds = config.ActiveAppStatusDVIds.ToList();
        }
    }
}