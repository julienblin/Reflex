// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Applications.Models;
using CGI.Reflex.Web.Areas.Applications.Models.Applications;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class ApplicationsController : BaseController
    {
        [IsAllowed("/Applications")]
        public ActionResult Index(ApplicationsIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Name";
                model.OrderType = OrderType.Asc;
            }

            model.SearchResults = new ApplicationQuery
                                    {
                                        NameOrCodeLike = model.QuickNameOrCode,
                                        ApplicationTypeIds = model.ApplicationTypes,
                                        StatusIds = model.Statuses,
                                        CriticityIds = model.Criticities,
                                        LinkedContactId = model.Contact,
                                        LinkedSectorId = model.Sector,
                                        LinkedTechnologyId = model.Technology,
                                        LinkedServerId = model.Server,
                                        RoleId = model.Role
                                    }
                                    .OrderBy(model.OrderBy, model.OrderType)
                                    .Fetch(a => a.AppInfo)
                                    .Paginate(model.Page);

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);

            if (!string.IsNullOrEmpty(model.QuickNameOrCode) && model.SearchResults.TotalItems == 1)
                return RedirectToAction("Details", "Info", new { appId = model.SearchResults.Items.First().Id });

            return View(model);
        }

        [IsAllowed("/Applications/Create")]
        public ActionResult Create()
        {
            return PartialView("_CreateModal", new AppHeader());
        }

        [HttpPost]
        [IsAllowed("/Applications/Create")]
        public ActionResult Create(AppHeader model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateModal", model);

            var application = new Application
            {
                Code = model.Code, 
                Name = model.Name, 
                ApplicationType = model.ApplicationTypeId.ToDomainValue()
            };
            NHSession.Save(application);

            Flash(FlashLevel.Success, string.Format("L'application {0} a été créée avec succès.", application.Name));

            return JSRedirect(Url.Action("Details", "Info", new { appId = application.Id }));
        }
    }
}
