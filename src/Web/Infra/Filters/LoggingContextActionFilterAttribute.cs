// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingContextActionFilterAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Web.Infra.Log;
using log4net;

namespace CGI.Reflex.Web.Infra.Filters
{
    [ExcludeFromCodeCoverage]
    public class LoggingContextActionFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public const string RequestLogger = @"Request";

        public const string ThreadContextCorrelationId = @"ThreadContextCorrelationId";

        private static readonly ILog Logger = LogManager.GetLogger(RequestLogger);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var correlationId = Path.GetRandomFileName().Replace(".", string.Empty);
            filterContext.HttpContext.Items[CorrelationIdLogProvider.RequestCorrelationIdContextKey] = correlationId;

            Logger.Debug("OnActionExecuting");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger.Debug("OnActionExecuted");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Logger.Debug("OnResultExecuting");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Logger.Debug("OnResultExecuted");
        }
        
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Logger.Error(filterContext.Exception.Message, filterContext.Exception);
                ThreadContext.Properties[ThreadContextCorrelationId] = filterContext.HttpContext.Items[CorrelationIdLogProvider.RequestCorrelationIdContextKey].ToString();
            }
        }
    }
}