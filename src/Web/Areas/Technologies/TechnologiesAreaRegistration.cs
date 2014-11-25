// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesAreaRegistration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Areas.Technologies
{
    [ExcludeFromCodeCoverage]
    public class TechnologiesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Technologies";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Technologies_default", 
                "Technologies/{controller}/{action}/{id}", 
                new { action = "Index", controller = "Technologies", id = UrlParameter.Optional });
        }
    }
}
