// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentUserActionFilterAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Configuration;

namespace CGI.Reflex.Web.Infra.Filters
{
    public class CurrentUserActionFilterAttribute : ActionFilterAttribute
    {
        private ReflexConfigurationSection _configuration;

        public CurrentUserActionFilterAttribute()
        {
        }

        public CurrentUserActionFilterAttribute(ReflexConfigurationSection configuration)
        {
            _configuration = configuration;
        }

        private ReflexConfigurationSection Configuration
        {
            get { return _configuration ?? (_configuration = ReflexConfigurationSection.GetConfigurationSection()); }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
                return;

            User user = null;

            switch (Configuration.AuthenticationMode)
            {
                case AuthenticationMode.None:
                    break;
                case AuthenticationMode.Windows:
                    user = new UserQuery { UserName = filterContext.RequestContext.HttpContext.User.Identity.Name, IsLockedOut = false }
                                        .JoinQueryOver(u => u.Role)
                                        .Cacheable()
                                        .SingleOrDefault();
                    break;
                case AuthenticationMode.Forms:
                    int userId;
                    if (!int.TryParse(filterContext.RequestContext.HttpContext.User.Identity.Name, out userId))
                        return;

                    user = new UserQuery { Id = userId, IsLockedOut = false }
                                        .JoinQueryOver(u => u.Role)
                                        .Cacheable()
                                        .SingleOrDefault();
                    break;
                default:
                    throw new NotSupportedException(_configuration.AuthenticationMode.ToString());
            }
            
            if (user != null)
            {
                References.NHSession.SetReadOnly(user, true);
                References.NHSession.SetReadOnly(user.Role, true);
                filterContext.RequestContext.HttpContext.Items[MvcApplication.CurrentUserContextKey] = user;
            }
        }
    }
}