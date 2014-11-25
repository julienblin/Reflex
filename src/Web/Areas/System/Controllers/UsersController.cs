// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.Users;
using CGI.Reflex.Web.Commands;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class UsersController : BaseController
    {
        [IsAllowed("/System/Users")]
        public ActionResult Index(UsersIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "UserName";
                model.OrderType = OrderType.Asc;
            }

            model.SearchResults = new UserQuery
            {
                NameOrEmailLike = model.NameOrEmail,
                RoleIds = model.Roles,
                CompanyIds = model.Companies,
                IsLockedOut = model.IncludeLockedOut ? null : new bool?(false)
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .Fetch(u => u.Role)
            .Paginate(model.Page);

            model.AvailableRoles = new RoleQuery().OrderBy(r => r.Name).List();
            model.AuthorizedRoles = GetAuthorizedRoles(model.AvailableRoles);

            return View(model);
        }

        [IsAllowed("/System/Users/Create")]
        public ActionResult Create()
        {
            var model = new UserEdit { Roles = GetAuthorizedRoles(new RoleQuery().List()) };
            if (!model.Roles.Any()) return new HttpUnauthorizedResult();
            return View(model);
        }

        [IsAllowed("/System/Users/Create")]
        [HttpPost]
        public ActionResult Create(UserEdit model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetAuthorizedRoles(new RoleQuery().List());
                if (!model.Roles.Any()) return new HttpUnauthorizedResult();
                return View(model);
            }

            var targetRole = NHSession.Load<Role>(model.RoleId.Value);
            if (!IsAllowed(GetAllOperationsCommand.UserRoleAssignmentPrefix + targetRole.Name))
                return new HttpUnauthorizedResult();

            var user = new User();
            BindFromModel(model, user);

            NHSession.Save(user);

            var successMessage = string.Format("L'utilisateur {0} a bien été créé.", user.UserName);
            if (Config.AuthenticationMode == AuthenticationMode.Forms)
            {
                var userAuth = new UserAuthentication { User = user };
                NHSession.Save(userAuth);
                Execute<SendUserResetTokenCommand, string>(cmd =>
                {
                    cmd.UserAuthentication = userAuth;
                    cmd.UrlHelper = Url;
                });
                successMessage += Environment.NewLine + string.Format("Un courriel permettant la connexion a été envoyé à l'adresse {0}.", user.Email);
            }

            Flash(FlashLevel.Success, successMessage);
            return RedirectToAction("Index");
        }

        [IsAllowed("/System/Users/Update")]
        public ActionResult Edit(int id)
        {
            var user = NHSession.Get<User>(id);
            var model = new UserEdit { Roles = GetAuthorizedRoles(new RoleQuery().List()) };

            if (!model.Roles.Any(x => x.Id == user.Role.Id))
                return new HttpUnauthorizedResult();

            BindToModel(user, model);

            if (!model.Roles.Any()) return new HttpUnauthorizedResult();
            return View(model);
        }

        [IsAllowed("/System/Users/Update")]
        [HttpPost]
        public ActionResult Edit(int id, UserEdit model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetAuthorizedRoles(new RoleQuery().List());
                if (!model.Roles.Any()) return new HttpUnauthorizedResult();
                return View(model);
            }

            var targetRole = NHSession.Load<Role>(model.RoleId.Value);
            if (!IsAllowed(GetAllOperationsCommand.UserRoleAssignmentPrefix + targetRole.Name))
                return new HttpUnauthorizedResult();

            var user = NHSession.Get<User>(id);

            // We might get the current user, which is read-only!
            if (NHSession.IsReadOnly(user))
                NHSession.SetReadOnly(user, false);

            BindFromModel(model, user);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [IsAllowed("/System/Users/Update")]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult ReinitPassword(int id)
        {
            var userAuth = NHSession.QueryOver<UserAuthentication>().Where(a => a.User.Id == id).SingleOrDefault();
            if (userAuth == null)
                return HttpNotFound();

            userAuth.ClearPassword();
            userAuth.User.IsLockedOut = false;
            Execute<SendUserResetTokenCommand, string>(cmd =>
            {
                cmd.UserAuthentication = userAuth;
                cmd.UrlHelper = Url;
            });

            Flash(FlashLevel.Success, string.Format("Un courriel permettant la connexion a été envoyé à l'adresse {0}.", userAuth.User.Email));
            return RedirectToAction("Index");
        }

        private void BindFromModel(UserEdit model, User user)
        {
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Company = model.Company.ToDomainValue();
            if (model.RoleId != null)
                user.Role = NHSession.Load<Role>(model.RoleId.Value);
            user.IsLockedOut = model.IsLockedOut;
        }

        private void BindToModel(User user, UserEdit model)
        {
            model.Id = user.Id;
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.Company = user.Company.ToId();
            model.RoleId = user.Role.Id;
            model.IsLockedOut = user.IsLockedOut;
        }

        private IEnumerable<Role> GetAuthorizedRoles(IEnumerable<Role> roles)
        {
            return roles.Where(x => IsAllowed(GetAllOperationsCommand.UserRoleAssignmentPrefix + x.Name));
        }
    }
}
