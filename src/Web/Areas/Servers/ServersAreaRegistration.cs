// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServersAreaRegistration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Areas.Servers
{
    [ExcludeFromCodeCoverage]
    public class ServersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Servers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Servers_default", 
                "Servers", 
                new { action = "Index", controller = "Servers" });

            context.MapRoute(
                "Servers_Create", 
                "Servers/Create", 
                new { action = "Create", controller = "Servers" });

            context.MapRoute(
                "Landscapes_main", 
                "Servers/Landscapes/{action}/{id}", 
                new { action = "Index", controller = "Landscapes", id = UrlParameter.Optional });

            context.MapRoute(
                "Servers_main", 
                "Servers/{serverId}/{controller}/{action}", 
                new { action = "Details", controller = "Servers" });
        }
    }
}
