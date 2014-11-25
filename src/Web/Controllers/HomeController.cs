// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

using CGI.Reflex.Core.Commands;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Core.Queries.Review;
using CGI.Reflex.Core.Queries.Series;
using CGI.Reflex.Web.Charts;
using CGI.Reflex.Web.Helpers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Infra.Results;
using CGI.Reflex.Web.Models.Home;

namespace CGI.Reflex.Web.Controllers
{
    public class HomeController : BaseController
    {
        [IsAllowed("/")]
        public ActionResult Index(HomeParams model)
        {
            ModelState.Clear();
            model.LineCriteriaList = ApplicationSeries.GetLinesCriteriaNames(includeManyToOne: false).OrderBy(kv => kv.Value);

            if (string.IsNullOrEmpty(model.LineCriteria))
                model.LineCriteria = "ApplicationType";

            model.NumberOfActiveApplications = new ApplicationQuery { Active = true }.Count();

            return View(model);
        }

        [IsAllowed("/")]
        [OutputCache(Duration = 14400, Location = OutputCacheLocation.Server)]
        public ActionResult ApplicationsChart(int width, int height, string lineCriteria)
        {
            var result = new ApplicationSeries { LineCriteria = lineCriteria, OnlyActiveApplications = true }.Execute();
            var chart = new ApplicationSeriesChart(width, height) { Result = result };
            return new ChartResult(chart.Produce());
        }

        [IsAllowed("/SummaryDetails")]
        public ActionResult SummaryDetails(SummaryDetails model)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(model.LineCriteria))
                model.LineCriteria = "ApplicationType";

            model.LineCriteriaList = ApplicationSeries.GetLinesCriteriaNames().OrderBy(kv => kv.Value);
            model.ColumnCriteriaList = new[] { new KeyValuePair<string, string>(string.Empty, string.Empty) }.Concat(ApplicationSeries.GetColumnsCriteriaNames(model.LineCriteria).OrderBy(kv => kv.Value));
            try
            {
                model.Result = new ApplicationSeries { LineCriteria = model.LineCriteria, ColumnCriteria = model.ColumnCriteria, OnlyActiveApplications = model.OnlyActiveApplications }.Execute();
            }
            catch (NotSupportedException)
            {
                model.ColumnCriteria = string.Empty;
                model.Result = new ApplicationSeries { LineCriteria = model.LineCriteria, ColumnCriteria = model.ColumnCriteria, OnlyActiveApplications = model.OnlyActiveApplications }.Execute();
            }

            if (model.Result.LineMultiplicities == LineMultiplicities.ManyToOne)
                model.DisplayType = DisplayType.Values;

            if (Request.IsAjaxRequest())
                return PartialView("_Results", model);

            return View(model);
        }

        [IsAllowed("/")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ApplicationsReviewChart(int width, int height, int? appId)
        {
            var result = new ApplicationsReview { AppId = appId }.Execute();
            var chart = new ApplicationsReviewsChart(width, height) { Result = result };
            return new ChartResult(chart.Produce());
        }

        [IsAllowed("/Technologies")]
        [OutputCache(Duration = 14400, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        public ActionResult CriticalTechno()
        {
            var result = Execute<GetCriticalTechnologiesCommand, IEnumerable<CriticalTechnologiesResultLine>>();
            ViewData["editable"] = false;
            ViewData["linkToApplicationSearch"] = true;
            return PartialView("_CriticalTechno", result.Take(15).Select(l => l.GetTechno()));
        }

        [IsAllowed("/Applications/Review")]
        [OutputCache(Duration = 14400, Location = OutputCacheLocation.Server, VaryByParam = "none")]
        [CanBeSlow]
        public ActionResult Assessment()
        {
            var result = new ApplicationsReview { OnlyActiveApplications = true }.Execute();
            return PartialView("_Assessment", result);
        }
    }
}
