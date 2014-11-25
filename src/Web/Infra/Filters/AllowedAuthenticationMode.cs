// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllowedAuthenticationMode.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CGI.Reflex.Web.Configuration;

namespace CGI.Reflex.Web.Infra.Filters
{
    public class AllowedAuthenticationMode : AuthorizeAttribute
    {
        private readonly AuthenticationMode _authMode;

        public AllowedAuthenticationMode(AuthenticationMode authMode)
        {
            _authMode = authMode;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return ReflexConfigurationSection.GetConfigurationSection().AuthenticationMode == _authMode;
        }
    }
}