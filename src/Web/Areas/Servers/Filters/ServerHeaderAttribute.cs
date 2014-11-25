// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerHeaderAttribute.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Servers.Models;

namespace CGI.Reflex.Web.Areas.Servers.Filters
{
    [ExcludeFromCodeCoverage]
    public class ServerHeaderAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if ((filterContext.Controller.ViewBag.ServerHeader == null)
            && (!string.IsNullOrEmpty((string)filterContext.RouteData.Values["serverId"])))
            {
                int serverId;
                if (!int.TryParse(filterContext.RouteData.Values["serverId"].ToString(), out serverId))
                {
                    filterContext.Result = new HttpNotFoundResult();
                    return;
                }

                var session = References.NHSession;
                var server = session.QueryOver<Server>()
                                         .Where(a => a.Id == serverId)
                                         .Fetch(a => a.Environment).Eager
                                         .Cacheable()
                                         .SingleOrDefault();

                if (server == null)
                {
                    filterContext.Result = new HttpNotFoundResult();
                    return;
                }

                filterContext.Controller.ViewBag.ServerHeader = new ServerHeader
                {
                    Id = server.Id, 
                    Name = server.Name, 
                    Alias = server.Alias, 
                    EnvironmentId = server.Environment.ToId()
                };
            }
        }
    }
}