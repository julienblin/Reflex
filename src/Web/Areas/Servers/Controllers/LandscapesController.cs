// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapesController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Models.Landscapes;
using CGI.Reflex.Web.Commands;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.Servers.Controllers
{
    public class LandscapesController : BaseController
    {
        [IsAllowed("/Servers/Landscapes")]
        public ActionResult Index(LandscapesIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Name";
                model.OrderType = OrderType.Asc;
            }

            model.SearchResults = new LandscapeQuery
            {
                NameLike = model.Name
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .Paginate(model.Page);
            return View(model);
        }

        [IsAllowed("/Servers/Landscapes/Create")]
        public ActionResult Create()
        {
            var model = new LandscapesEdit { LandscapeServers = new LandscapesServersDisplay() };
            return View(model);
        }

        [IsAllowed("/Servers/Landscapes/Create")]
        [HttpPost]
        public ActionResult Create(LandscapesEdit model, IEnumerable<int> serverIds)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var landscape = new Landscape();
            BindFromModel(model, landscape);

            if (serverIds != null)
            {
                var serversList = serverIds.Select(serverId => NHSession.Load<Server>(serverId)).ToList();
                landscape.SetServers(serversList);
            }

            NHSession.Save(landscape);

            Flash(FlashLevel.Success, string.Format("Le landscape {0} a bien été créé", landscape.Name));
            return RedirectToAction("Index");
        }

        [IsAllowed("/Servers/Landscapes/Update")]
        public ActionResult Edit(int id)
        {
            var landscape = NHSession.Get<Landscape>(id);
            if (landscape == null)
                return HttpNotFound();

            var servers = landscape.Servers.Select(i => i.Id);

            var model = new LandscapesEdit { LandscapeServers = GetLandscapeServersDisplay(servers) };
            BindToModel(landscape, model);
            return View(model);
        }

        [IsAllowed("/Servers/Landscapes/Update")]
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(LandscapesEdit model, IEnumerable<int> serverIds)
        {
            var landscape = NHSession.Get<Landscape>(model.Id);
            if (landscape == null)
                return HttpNotFound();

            if (landscape.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>Le landscape {0} a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs du landscape et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(landscape.Name),
                        Url.Action("Edit", new { id = landscape.Id })), 
                    disableHtmlEscaping: true);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (NHSession.IsReadOnly(landscape))
                NHSession.SetReadOnly(landscape, false);

            BindFromModel(model, landscape);

            var serversList = new List<Server>();
            
            if (serverIds != null)
                serversList = serverIds.Select(serverId => NHSession.Load<Server>(serverId)).ToList();

            landscape.SetServers(serversList);

            Flash(FlashLevel.Success, string.Format("Le landscape {0} a bien été mis à jour.", landscape.Name));
            return RedirectToAction("Index");
        }

        [IsAllowed("/Servers/Landscapes/Delete")]
        public ActionResult Delete(int id)
        {
            var landscape = NHSession.Get<Landscape>(id);
            if (landscape == null)
                return HttpNotFound();

            var model = new LandscapesEdit();
            BindToModel(landscape, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Servers/Landscapes/Delete")]
        [HttpPost]
        public ActionResult Delete(LandscapesEdit model)
        {
            var landscape = NHSession.Get<Landscape>(model.Id);
            if (landscape == null)
                return HttpNotFound();

            foreach (var server in landscape.Servers)
                server.Landscape = null;

            NHSession.Delete(landscape);

            Flash(FlashLevel.Success, string.Format("Le landscape {0} a bien été supprimé.", landscape.Name));

            return RedirectToAction("Index");
        }

        [IsAllowed("/Servers/Landscapes")]
        public ActionResult GetServers(IEnumerable<int> selectedIds)
        {
            var model = GetLandscapeServersDisplay(selectedIds);

            return PartialView("_LandscapesServersDisplay", model);
        }

        [IsAllowed("/Servers/Landscapes")]
        public ActionResult GetLandscapeInfo(int landscapeId)
        {
            var landscape = NHSession.Get<Landscape>(landscapeId);
            var model = new LandscapeInfo
            {
                ServersDetails =
                    new LandscapesServersDisplay
                        {
                            AllowAddServer = false,
                            AllowRemoveLandscape = false,
                            AllowRemoveServer = false,
                            ShowLandscape = false,
                            ShowLinkToLandscape = false
                        }
            };

            model.ServersDetails.AddServers(landscape.Servers, landscape);

            return PartialView("_LandscapeInfo", model);
        }

        private void BindToModel(Landscape landscape, LandscapesEdit model)
        {
            model.Id = landscape.Id;
            model.ConcurrencyVersion = landscape.ConcurrencyVersion;
            model.Name = landscape.Name;
            model.Description = landscape.Description;
        }

        private void BindFromModel(LandscapesEdit model, Landscape landscape)
        {
            landscape.Name = model.Name;
            landscape.Description = model.Description;
        }

        private LandscapesServersDisplay GetLandscapeServersDisplay(IEnumerable<int> servers)
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
                                      .WhereRestrictionOn(s => s.Id).IsIn(servers.ToArray())
                                      .List();

            results.ShowLandscape = false;
            results.ShowLinkToLandscape = false;
            results.AllowAddServer = true;
            results.AllowRemoveServer = true;
            results.AllowRemoveLandscape = false;

            results.AddServers(serverList, null);

            return results;
        }
    }
}
