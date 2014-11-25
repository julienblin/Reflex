// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using log4net;

namespace CGI.Reflex.Web.Controllers
{
    public class ErrorController : BaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(LoggingContextActionFilterAttribute.RequestLogger);

        public ActionResult NotFound()
        {
            ViewBag.UrlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : null;
            
            var wrongUrl = Request.QueryString.Count > 0 ? Request.QueryString[0] : string.Empty;
            if (!string.IsNullOrEmpty(wrongUrl))
                Logger.WarnFormat("404 Not found for {0}", wrongUrl.StartsWith("404;") ? wrongUrl.Substring(4) : wrongUrl);

            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            ViewBag.ErrorCorrelationId = string.Empty;
            if (!string.IsNullOrEmpty((string)ThreadContext.Properties[LoggingContextActionFilterAttribute.ThreadContextCorrelationId]))
            {
                ViewBag.ErrorCorrelationId = ThreadContext.Properties[LoggingContextActionFilterAttribute.ThreadContextCorrelationId];
                ThreadContext.Properties.Remove(LoggingContextActionFilterAttribute.ThreadContextCorrelationId);
            }

            return View();
        }

        public ActionResult Test()
        {
            throw new Exception("This is an error generated for test purposes. You can safely ignore it.");
        }
    }
}
