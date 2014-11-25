// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedTechnologiesBinder.cs" company="CGI">
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

namespace CGI.Reflex.Web.Binders
{
    [ExcludeFromCodeCoverage]
    public class CheckedTechnologiesBinder : BaseModelBinder<CheckedTechnologies>
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");

            var technoIds = new List<int>();
            foreach (var formKey in controllerContext.HttpContext.Request.Form.AllKeys)
            {
                if (formKey.StartsWith("check_techno-") && controllerContext.HttpContext.Request.Form[formKey].Equals("1"))
                {
                    int technoId;
                    if (int.TryParse(formKey.Substring(13), out technoId))
                        technoIds.Add(technoId);
                }
            }

            return new CheckedTechnologies { TechnologyIds = technoIds };
        }
    }
}