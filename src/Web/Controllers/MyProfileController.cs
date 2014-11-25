// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyProfileController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models.MyProfile;

namespace CGI.Reflex.Web.Controllers
{
    public class MyProfileController : BaseController
    {
        [IsAllowed("/")]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Index()
        {
            var model = new MyProfileEdit
            {
                Id = References.CurrentUser.Id, 
                UserName = References.CurrentUser.UserName, 
                Email = References.CurrentUser.Email, 
                CompanyId = References.CurrentUser.Company.ToId(), 
                Role = References.CurrentUser.Role
            };
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/")]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Edit(MyProfileEdit model)
        {
            if (!ModelState.IsValid)
            {
                model.Role = References.CurrentUser.Role;
                return View("Index", model);
            }

            var user = References.CurrentUser;
            NHSession.SetReadOnly(user, false);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Company = model.CompanyId.ToDomainValue();

            if (!string.IsNullOrEmpty(model.Password))
            {
                var userAuth = NHSession.QueryOver<UserAuthentication>().Where(a => a.User.Id == user.Id).SingleOrDefault();
                userAuth.SetPassword(model.Password);
            }

            Flash(FlashLevel.Success, "Votre profil a bien été mis à jour.");
            return RedirectToAction("Index", "Home");
        }
    }
}