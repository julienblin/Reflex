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
using CGI.Reflex.Web.Areas.Servers.Filters;
using CGI.Reflex.Web.Areas.Servers.Models.Applications;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Servers.Controllers
{
    public class ApplicationsController : BaseController
    {
        [ServerHeader]
        [IsAllowed("/Servers/Applications")]
        public ActionResult Details(int serverId, ApplicationsIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Name";
                model.OrderType = OrderType.Asc;
            }

            var server = NHSession.Get<Server>(serverId);

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            model.ServerId = server.Id;
            model.SearchResults = new ApplicationQuery
                                    {
                                        LinkedServerId = server.Id
                                    }
                                    .OrderBy(model.OrderBy, model.OrderType)
                                    .Fetch(a => a.AppInfo)
                                    .Paginate(model.Page);
            
            if (Request.IsAjaxRequest())
                return PartialView("_List", model);

            return View(model);
        }
    }
}
