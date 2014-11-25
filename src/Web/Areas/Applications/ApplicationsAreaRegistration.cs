// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsAreaRegistration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Areas.Applications
{
    [ExcludeFromCodeCoverage]
    public class ApplicationsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Applications";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Applications_default", 
                "Applications", 
                 new { action = "Index", controller = "Applications" });

            context.MapRoute(
                "Applications_Create", 
                "Applications/Create", 
                new { action = "Create", controller = "Applications" });

            context.MapRoute(
                "Applications_main", 
                "Applications/{appId}/{controller}/{action}", 
                new { action = "Details", controller = "Info" });
        }
    }
}
