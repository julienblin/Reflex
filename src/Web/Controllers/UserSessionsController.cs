// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSessionsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Commands;
using CGI.Reflex.Web.Configuration;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models.UserSessions;
using Postal;

namespace CGI.Reflex.Web.Controllers
{
    public class UserSessionsController : BaseController
    {
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Login(string returnUrl)
        {
            return View(new Login { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ExcludeFromCodeCoverage]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var userAuth = UserAuthentication.Authenticate(model.NameOrEmail, model.Password);

                if (userAuth == null && Config.AutoCreateUsers && Config.Environment != ConfigEnvironment.Production)
                {
                    var role = NHSession.QueryOver<Role>().Where(r => r.Name == model.Password).SingleOrDefault();
                    if (role != null)
                    {
                        var user = new User
                        {
                            UserName = model.NameOrEmail, 
                            Email = model.NameOrEmail, 
                            Role = role
                        };
                        NHSession.Save(user);

                        userAuth = new UserAuthentication
                        {
                            User = user, 
                            LastLoginDate = DateTime.Now
                        };
                        userAuth.SetPassword(model.Password);
                        NHSession.Save(userAuth);
                    }
                }

                if (userAuth != null)
                {
                    FormsAuthentication.SetAuthCookie(userAuth.User.Id.ToString(CultureInfo.InvariantCulture), false);
                    if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                        && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Home");
                }

                Flash(FlashLevel.Error, "Le nom d'utilisateur, courriel ou mot de passe est incorrect.");
            }

            return View(model);
        }

        [ExcludeFromCodeCoverage]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToRoute("Login");
        }

        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult Boarding(string key)
        {
            var userAuth = NHSession.QueryOver<UserAuthentication>()
                                    .Where(a => a.SingleAccessToken == key)
                                    .Fetch(a => a.User).Eager
                                    .SingleOrDefault();
            if (userAuth == null)
                return HttpNotFound();

            if (userAuth.SingleAccessTokenValidUntil < DateTime.Now)
                return View("BoardingPerished");

            if (userAuth.User.IsLockedOut)
                return View("BoardingPerished");

            var model = new Boarding
            {
                SingleAccessToken = key, 
                UserName = userAuth.User.UserName
            };

            return View(model);
        }

        [HttpPost]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        [ExcludeFromCodeCoverage]
        public ActionResult Boarding(Boarding model)
        {
            var userAuth = NHSession.QueryOver<UserAuthentication>()
                                    .Where(a => a.SingleAccessToken == model.SingleAccessToken)
                                    .Fetch(a => a.User).Eager
                                    .SingleOrDefault();
            if (userAuth == null)
                return HttpNotFound();

            if (userAuth.SingleAccessTokenValidUntil < DateTime.Now)
                return HttpNotFound();

            if (userAuth.User.IsLockedOut)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.UserName = userAuth.User.UserName;
                return View(model);
            }

            userAuth.SetPassword(model.Password);
            userAuth.ClearSingleAccessToken();

            FormsAuthentication.SetAuthCookie(userAuth.User.Id.ToString(CultureInfo.InvariantCulture), false);
            return RedirectToAction("Index", "Home");
        }

        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowedAuthenticationMode(AuthenticationMode.Forms)]
        [ExcludeFromCodeCoverage]
        public ActionResult ResetPassword(string userNameOrEmail)
        {
            var user = new UserQuery { NameOrEmail = userNameOrEmail }.SingleOrDefault();
            if (user == null)
            {
                Flash(FlashLevel.Warning, "Ce nom d'utilisateur ou courriel est introuvable.");
                return View();
            }

            var userAuth = NHSession.QueryOver<UserAuthentication>().Where(a => a.User.Id == user.Id).SingleOrDefault();
            userAuth.ClearPassword();
            Execute<SendUserResetTokenCommand, string>(cmd =>
            {
                cmd.UserAuthentication = userAuth;
                cmd.UrlHelper = Url;
            });
            Flash(FlashLevel.Success, string.Format("Un courriel permettant la connexion a été envoyé à l'adresse {0}.", user.Email));
            return View("ResetPasswordResult");
        }
    }
}
