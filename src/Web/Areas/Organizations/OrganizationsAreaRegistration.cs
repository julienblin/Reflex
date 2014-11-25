// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationsAreaRegistration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Areas.Organizations
{
    [ExcludeFromCodeCoverage]
    public class OrganizationsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Organizations";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Organizations_default", 
                "Organizations/{controller}/{action}/{id}", 
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
