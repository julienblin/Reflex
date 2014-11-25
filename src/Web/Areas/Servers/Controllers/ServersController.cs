// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Filters;
using CGI.Reflex.Web.Areas.Servers.Models;
using CGI.Reflex.Web.Areas.Servers.Models.Servers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Servers.Controllers
{
    public class ServersController : BaseController
    {
        [ServerHeader]
        [IsAllowed("/Servers")]
        public ActionResult Index(ServersIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Name";
                model.OrderType = OrderType.Asc;
            }

            model.SearchResults = new ServerQuery
            {
                NameOrAliasLike = model.QuickNameOrAlias, 
                NameLike = model.Name, 
                AliasLike = model.Alias, 
                EvergreenDateTo = model.EvergreenDate, 
                EnvironmentIds = model.Environments, 
                RoleIds = model.Roles, 
                StatusIds = model.Statuses, 
                TypeIds = model.Types, 
                LandscapeId = model.LandscapeId, 
                LinkedTechnologyId = model.Technology
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .Paginate(model.Page);

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);

            if (!string.IsNullOrEmpty(model.QuickNameOrAlias) && model.SearchResults.TotalItems == 1)
                return RedirectToAction("Details", "Servers", new { serverId = model.SearchResults.Items.First().Id });

            return View(model);
        }

        [IsAllowed("/Servers/Create")]
        public ActionResult Create()
        {
            return PartialView("_CreateModal", new ServerHeader());
        }

        [IsAllowed("/Servers/Create")]
        [HttpPost]
        public ActionResult Create(ServerHeader model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateModal", model);

            var server = new Server
            {
                Name = model.Name, 
                Alias = model.Alias, 
                Environment = model.EnvironmentId.ToDomainValue()
            };
            NHSession.Save(server);

            Flash(FlashLevel.Success, string.Format("Le serveur {0} a été créé avec succès.", server.Name));

            return JSRedirect(Url.Action("Edit", "Servers", new { serverId = server.Id }));
        }

        [ServerHeader]
        [IsAllowed("/Servers/Info")]
        public ActionResult Details(int serverId)
        {
            var server = NHSession.QueryOver<Server>()
                        .Where(s => s.Id == serverId)
                        .Fetch(s => s.Landscape).Eager
                        .SingleOrDefault();

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            var model = new ServerEdit();
            
            BindToModel(server, model);

            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/Info/Update")]
        public ActionResult Edit(int serverId)
        {
            var server = NHSession.Get<Server>(serverId);
            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            var model = new ServerEdit();
           
            BindToModel(server, model);
            ViewBag.LandscapeList = new LandscapeQuery().OrderBy(l => l.Name).List();
            return View(model);
        }

        [ServerHeader]
        [IsAllowed("/Servers/Info/Update")]
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(int serverId, ServerEdit model)
        {
            var server = NHSession.QueryOver<Server>()
                                  .Where(s => s.Id == serverId)
                                  .Fetch(s => s.Landscape).Eager
                                  .SingleOrDefault();

            if (server == null)
                return HttpNotFound();

            ViewBag.ServerName = server.Name;
            ViewBag.LandscapeList = new LandscapeQuery().OrderBy(l => l.Name).List();

            if (server.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>Le serveur {0} a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs du serveur et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(server.Name),
                        Url.Action("Edit", new { serverId = server.Id })),
                    disableHtmlEscaping: true);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (NHSession.IsReadOnly(server))
                NHSession.SetReadOnly(server, false);

            BindFromModel(model, server);

            Flash(FlashLevel.Success, string.Format("Le serveur {0} a bien été mis à jour.", server.Name));
            return RedirectToAction("Details", new { serverId = server.Id });
        }

        [ServerHeader]
        [IsAllowed("/Servers/Delete")]
        public ActionResult Delete(int id)
        {
            var server = NHSession.Get<Server>(id);
            if (server == null)
                return HttpNotFound();

            var model = new ServerEdit();
            BindToModel(server, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Servers/Delete")]
        [HttpPost]
        public ActionResult Delete(ServerEdit model)
        {
            var server = NHSession.Get<Server>(model.Id);
            if (server == null)
                return HttpNotFound();

            // Using sub-session / sub-transaction to catch ConstraintViolationException
            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(server);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    Flash(FlashLevel.Error, string.Format("Impossible de supprimer le serveur {0} parce que certaines entités sont encore reliées.", server.Name));
                    return RedirectToAction("Index");
                }
            }

            Flash(FlashLevel.Success, string.Format("Le serveur {0} a bien été supprimé.", server.Name));
            return RedirectToAction("Index");
        }

        [IsAllowed("/Servers/Technologies")]
        public ActionResult RenderTechno(IEnumerable<int> ids)
        {
            var technos = new TechnologyHierarchicalQuery { RootIds = ids }.FetchParents().List();
            return PartialView("_Technologies", technos);
        }
        
        private void BindFromModel(ServerEdit model, Server server)
        {
            server.Name = model.Name;
            server.Alias = model.Alias;
            server.Comments = model.Comments;
            server.EvergreenDate = model.EvergreenDate;
            server.Environment = model.EnvironmentId.ToDomainValue();
            server.Role = model.RoleId.ToDomainValue();
            server.Status = model.StatusId.ToDomainValue();
            server.Type = model.TypeId.ToDomainValue();
            server.Landscape = model.LandscapeId.HasValue ? NHSession.Load<Landscape>(model.LandscapeId.Value) : null;
        }

        private void BindToModel(Server server, ServerEdit model)
        {
            model.Id = server.Id;
            model.Name = server.Name;
            model.ConcurrencyVersion = server.ConcurrencyVersion;
            model.Alias = server.Alias;
            model.Comments = server.Comments;
            model.EvergreenDate = server.EvergreenDate;
            model.EnvironmentId = server.Environment.ToId();
            model.RoleId = server.Role.ToId();
            model.StatusId = server.Status.ToId();
            model.TypeId = server.Type.ToId();
            model.LandscapeId = server.Landscape.ToId();
            model.LandscapeName = server.Landscape == null ? string.Empty : server.Landscape.Name;
        }
    }
}
