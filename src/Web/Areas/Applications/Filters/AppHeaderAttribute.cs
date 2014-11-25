// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppHeaderAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Areas.Applications.Models;

namespace CGI.Reflex.Web.Areas.Applications.Filters
{
    [ExcludeFromCodeCoverage]
    public class AppHeaderAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if ((filterContext.Controller.ViewBag.AppHeader == null)
            && (!string.IsNullOrEmpty((string)filterContext.RouteData.Values["appId"])))
            {
                int appId;
                if (!int.TryParse(filterContext.RouteData.Values["appId"].ToString(), out appId))
                {
                    filterContext.Result = new HttpNotFoundResult();
                    return;
                }

                var session = References.NHSession;
                var application = session.QueryOver<Application>()
                                         .Where(a => a.Id == appId)
                                         .Fetch(a => a.ApplicationType).Eager
                                         .Cacheable()
                                         .SingleOrDefault();

                if (application == null)
                {
                    filterContext.Result = new HttpNotFoundResult();
                    return;
                }
                
                filterContext.Controller.ViewBag.AppHeader = new AppHeader
                {
                    Id = application.Id, 
                    Code = application.Code, 
                    Name = application.Name, 
                    ApplicationTypeId = application.ApplicationType.ToId()
                };
            }
        }
    }
}