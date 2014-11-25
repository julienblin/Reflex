// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RolesController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;

using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.Roles;
using CGI.Reflex.Web.Commands;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class RolesController : BaseController
    {
        [IsAllowed("/System/Roles")]
        public ActionResult Index(RolesIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Name";
                model.OrderType = OrderType.Asc;
            }

            model.AllowedOperations = new[] { string.Empty }.Union(Execute<GetAllOperationsCommand, IEnumerable<string>>(c => { c.AddWildCards = false; c.AddUserRoleAssignement = true; }));

            var allResults = new RoleQuery
            {
                NameLike = model.Name
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .List();

            if (!string.IsNullOrEmpty(model.AllowedOperation))
                allResults = allResults.Where(r => r.IsAllowed(model.AllowedOperation));

            model.SearchResults = allResults.Paginate(model.Page);
            return View(model);
        }

        [IsAllowed("/System/Roles/Create")]
        public ActionResult Create()
        {
            var model = new RoleEdit { AvailableFunctions = Execute<GetAllOperationsCommand, IEnumerable<string>>() };
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/Roles/Create")]
        public ActionResult Create(RoleEdit model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableFunctions = Execute<GetAllOperationsCommand, IEnumerable<string>>().Except(model.AllowedOperations).Except(model.DeniedOperations).ToList();
                return View(model);
            }

            var role = new Role();
            BindFromModel(model, role);
            NHSession.Save(role);

            Flash(FlashLevel.Success, string.Format("Le rôle {0} a bien été créé.", role.Name));
            return RedirectToAction("Index");
        }

        [IsAllowed("/System/Roles/Update")]
        public ActionResult Edit(int id)
        {
            var role = NHSession.Get<Role>(id);
            if (role == null)
                return HttpNotFound();

            var model = new RoleEdit();
            BindToModel(role, model);
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/Roles/Update")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(RoleEdit model)
        {
            var role = NHSession.Get<Role>(model.Id);
            if (role == null)
                return HttpNotFound();

            if (role.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>Le rôle {0} a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs du rôle et perdre mes modifications</a>
                            </p>
                        ",
                       HttpContext.Server.HtmlEncode(role.Name),
                       Url.Action("Edit", new { id = role.Id })), 
                    disableHtmlEscaping: true);

                model.AvailableFunctions = Execute<GetAllOperationsCommand, IEnumerable<string>>().Except(model.AllowedOperations).Except(model.DeniedOperations).ToList();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.AvailableFunctions = Execute<GetAllOperationsCommand, IEnumerable<string>>().Except(model.AllowedOperations).Except(model.DeniedOperations).ToList();
                return View(model);
            }

            // We might get the current role, which is read-only!
            if (NHSession.IsReadOnly(role))
                NHSession.SetReadOnly(role, false);

            BindFromModel(model, role);

            Flash(FlashLevel.Success, string.Format("Le rôle {0} a bien été mis à jour.", role.Name));
            return RedirectToAction("Index");
        }

        [IsAllowed("/System/Roles/Delete")]
        public ActionResult Delete(int id)
        {
            var role = NHSession.Get<Role>(id);
            if (role == null)
                return HttpNotFound();

            var model = new RoleEdit();
            BindToModel(role, model);

            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [IsAllowed("/System/Roles/Delete")]
        public ActionResult Delete(RoleEdit model)
        {
            var role = NHSession.Get<Role>(model.Id);
            if (role == null)
                return HttpNotFound();

            var attachedUsersCount = new UserQuery { RoleId = role.Id }.Count();
            if (attachedUsersCount > 0)
            {
                Flash(FlashLevel.Error, string.Format("Impossible de supprimer le rôle {0} : il existe encore des utilisateurs avec ce rôle.", role.Name));
                return RedirectToAction("Index", new { id = role.Id });
            }

            NHSession.Delete(role);
            Flash(FlashLevel.Success, string.Format("Le rôle {0} a bien été supprimé.", role.Name));
            return RedirectToAction("Index");
        }

        private void BindToModel(Role role, RoleEdit model)
        {
            model.Id = role.Id;
            model.ConcurrencyVersion = role.ConcurrencyVersion;
            model.Name = role.Name;
            model.Description = role.Description;
            model.AllowedOperations = role.AllowedOperations;
            model.DeniedOperations = role.DeniedOperations;
            model.AvailableFunctions = Execute<GetAllOperationsCommand, IEnumerable<string>>().Except(model.AllowedOperations).Except(model.DeniedOperations).ToList();
        }

        private void BindFromModel(RoleEdit model, Role role)
        {
            role.Name = model.Name;
            role.Description = model.Description;
            role.SetAllowedOperations(model.AllowedOperations);
            role.SetDeniedOperations(model.DeniedOperations);
        }
    }
}
