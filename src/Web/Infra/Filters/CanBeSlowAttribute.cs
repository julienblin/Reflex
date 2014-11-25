// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanBeSlowAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGI.Reflex.Web.Infra.Filters
{
    /// <summary>
    /// When applied to a controller action method, avoids the signaling by <see cref="SlowRequestAlertActionFilterAttribute"/> when above threshold.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CanBeSlowAttribute : Attribute
    {
    }
}