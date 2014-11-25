// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextLogProvider.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Management;

namespace CGI.Reflex.Web.Infra.Log
{
    [ExcludeFromCodeCoverage]
    public class ContextLogProvider
    {
        public override string ToString()
        {
            if (HttpContext.Current == null)
                return string.Empty;

            try
            {
                return string.Format(
                    "{0} {1}[{2}] (<- {3})", 
                    HttpContext.Current.Request.HttpMethod, 
                    HttpContext.Current.Request.Url, 
                    HttpContext.Current.Request.Form, 
                    HttpContext.Current.Request.UrlReferrer);
            }
            catch (HttpException)
            {
                // Sometimes the HttpContext is available, but the request is not.
                // We filter out those errors here.
                return string.Empty;
            }
        }
    }
}