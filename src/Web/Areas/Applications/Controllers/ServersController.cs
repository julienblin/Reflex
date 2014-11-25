// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Servers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class ServersController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Servers")]
        public ActionResult Details(int appId)
        {
            var application = NHSession.Get<Application>(appId);

            if (application == null)
                return HttpNotFound();

            var model = new ServersDetails
            {
                Results = GetLandscapeServersDisplay(application.Id),
                AppName = application.Name,
                AppId = application.Id
            };

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Servers/Add")]
        public ActionResult AddServers(int appId, IEnumerable<int> servers)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            foreach (int serverId in servers)
            {
                var server = NHSession.Load<Server>(serverId);
                application.AddServerLink(server);
            }

            return RedirectToAction("Details");
        }

        [IsAllowed("/Applications/Servers/Add")]
        public ActionResult AddServer(int appId, int serverId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var server = NHSession.Load<Server>(serverId);
            application.AddServerLink(server);

            var model = new ServersDetails { Results = GetLandscapeServersDisplay(application.Id), AppName = application.Name };

            return PartialView("_LandscapesServersDisplay", model.Results);
        }

        [IsAllowed("/Applications/Servers/Remove")]
        public ActionResult RemoveServer(int appId, int serverId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.DbInstanceLinks).Eager
                                       .Fetch(a => a.DbInstanceLinks.First().DbInstances).Eager
                                       .Fetch(a => a.DbInstanceLinks.First().DbInstances.Server).Eager
                                       .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var server = NHSession.Get<Server>(serverId);

            var model = new ServerRemove
                {
                    ApplicationName = application.Name,
                    ApplicationId = application.Id,
                    ServerName = server.Name,
                    Id = server.Id,
                    DbInstancesToRemove =
                        application.DbInstanceLinks.Where(l => l.DbInstances.Server.Id == serverId).Select(l => l.DbInstances).ToList()
                };

            return PartialView("_RemoveServerModal", model);
        }

        [HttpPost]
        [IsAllowed("/Applications/Servers/Remove")]
        public ActionResult RemoveServer(int appId, ServerRemove model)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var server = NHSession.Load<Server>(model.Id);
            application.RemoveServerLink(server);

            var details = new ServersDetails { Results = GetLandscapeServersDisplay(application.Id), AppName = application.Name };
            return PartialView("_LandscapesServersDisplay", details.Results);
        }

        [IsAllowed("/Applications/Servers/Remove")]
        public ActionResult RemoveServerByLandscape(int appId, int landscapeId)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var landscape = NHSession.Load<Landscape>(landscapeId);
            var model = new LandscapeRemove();

            var serversToRemove = application.ServerLinks.Where(sl => sl.Server.Landscape != null && sl.Server.Landscape.Id == landscapeId).Select(sl => sl.Server.Id);

            model.Id = landscape.Id;
            model.LandscapeName = landscape.Name;
            model.ApplicationName = application.Name;
            model.ApplicationId = application.Id;
            model.DbInstancesToRemove = application.DbInstanceLinks.Where(dl => serversToRemove.Contains(dl.DbInstances.Server.Id)).Select(dl => dl.DbInstances).ToList();

            return PartialView("_RemoveLandscapeModal", model);
        }

        [HttpPost]
        [IsAllowed("/Applications/Servers/Remove")]
        public ActionResult RemoveServerByLandscape(int appId, LandscapeRemove model)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var landscape = NHSession.Load<Landscape>(model.Id);

            foreach (Server server in landscape.Servers)
            {
                application.RemoveServerLink(server);
            }

            var details = new ServersDetails { Results = GetLandscapeServersDisplay(application.Id), AppName = application.Name };

            return PartialView("_LandscapesServersDisplay", details.Results);
        }

        private LandscapesServersDisplay GetLandscapeServersDisplay(int applicationId)
        {
            var results = new LandscapesServersDisplay();

            var serverList = NHSession.QueryOver<Server>()
                                      .Fetch(s => s.Environment).Eager
                                      .Fetch(s => s.Status).Eager
                                      .Fetch(s => s.Role).Eager
                                      .Fetch(s => s.Landscape).Eager
                                      .Fetch(s => s.Type).Eager
                                      .Fetch(s => s.Landscape.Servers).Eager
                                      .Fetch(s => s.Landscape.Servers.First().Environment).Eager
                                      .Fetch(s => s.Landscape.Servers.First().Status).Eager
                                      .Fetch(s => s.Landscape.Servers.First().Role).Eager
                                      .Fetch(s => s.Landscape.Servers.First().Type).Eager
                                      .JoinQueryOver(s => s.ApplicationLinks)
                                      .Where(l => l.Application.Id == applicationId)
                                      .List();

            results.AllowAddServer = IsAllowed("/Applications/Servers/Add");
            results.AllowRemoveServer = IsAllowed("/Applications/Servers/Remove");
            results.AllowRemoveLandscape = IsAllowed("/Applications/Servers/Remove");

            results.AddServers(serverList);

            return results;
        }
    }
}