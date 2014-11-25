// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewBagDataActionFilterAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Web.Configuration;

namespace CGI.Reflex.Web.Infra.Filters
{
    [ExcludeFromCodeCoverage]
    public class ViewBagDataActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ReflexConfigurationSection _config;
        private readonly string _appName;
        private readonly string _appVersion;
        private readonly string _copyrightInfo;

        public ViewBagDataActionFilterAttribute()
        {
            _config = ReflexConfigurationSection.GetConfigurationSection();
            _appName = _config.AppName;
            _appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _copyrightInfo = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                                                          .Cast<AssemblyCopyrightAttribute>()
                                                          .First()
                                                          .Copyright;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.Config = _config;
            filterContext.Controller.ViewBag.AppName = _appName;
            filterContext.Controller.ViewBag.AppVersion = _appVersion;
            filterContext.Controller.ViewBag.CopyrightInfo = _copyrightInfo;
            filterContext.Controller.ViewBag.CurrentUser = References.CurrentUser;
            base.OnResultExecuting(filterContext);
        }
    }
}