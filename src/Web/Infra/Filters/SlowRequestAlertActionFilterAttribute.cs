// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlowRequestAlertActionFilterAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CGI.Reflex.Core;
using CGI.Reflex.Web.Configuration;

using log4net;

namespace CGI.Reflex.Web.Infra.Filters
{
    [ExcludeFromCodeCoverage]
    public class SlowRequestAlertActionFilterAttribute : ActionFilterAttribute
    {
        private const string SlowRequestLogger = @"SlowRequest";
        private const string StopwatchActionHttpContextKey = @"StopwatchAction";
        private const string StopwatchResultHttpContextKey = @"StopwatchResult";

        private static readonly ILog Logger = LogManager.GetLogger(SlowRequestLogger);

        private ReflexConfigurationSection _configuration;

        private ReflexConfigurationSection Configuration
        {
            get { return _configuration ?? (_configuration = ReflexConfigurationSection.GetConfigurationSection()); }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Configuration.SlowRequestThreshold <= 0 || !Logger.IsWarnEnabled)
                return;

            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(CanBeSlowAttribute), false).Any())
                return;

            var stopwatchAction = Stopwatch.StartNew();
            filterContext.HttpContext.Items[StopwatchActionHttpContextKey] = stopwatchAction;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var stopwatchAction = filterContext.HttpContext.Items[StopwatchActionHttpContextKey] as Stopwatch;
            if (stopwatchAction != null)
            {
                stopwatchAction.Stop();
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Configuration.SlowRequestThreshold > 0 && Logger.IsWarnEnabled)
            {
                var stopwatchResult = Stopwatch.StartNew();
                filterContext.HttpContext.Items[StopwatchResultHttpContextKey] = stopwatchResult;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var stopwatchAction = filterContext.HttpContext.Items[StopwatchActionHttpContextKey] as Stopwatch;
            var stopwatchResult = filterContext.HttpContext.Items[StopwatchResultHttpContextKey] as Stopwatch;
            if ((stopwatchAction != null) && (stopwatchResult != null))
            {
                stopwatchResult.Stop();
                if ((stopwatchAction.ElapsedMilliseconds + stopwatchResult.ElapsedMilliseconds) > Configuration.SlowRequestThreshold)
                {
                    Logger.WarnFormat(
                        "The request appears to execute slowly: Action: {0} ms., Result: {1} ms., Total: {2} ms. (threshold: {3} ms).",
                        stopwatchAction.ElapsedMilliseconds,
                        stopwatchResult.ElapsedMilliseconds,
                        stopwatchAction.ElapsedMilliseconds + stopwatchResult.ElapsedMilliseconds,
                        Configuration.SlowRequestThreshold);
                }
            }
        }
    }
}