// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutocompleteController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

using NHibernate.Criterion;

namespace CGI.Reflex.Web.Controllers
{
    public class AutocompleteController : BaseController
    {
        public const int DefaultMax = 8;

        [IsAllowed("/Applications")]
        public ActionResult ApplicationNames(string q, int maxResults = DefaultMax)
        {
            return Json(new ApplicationQuery { NameLike  = q }.Over().Select(x => x.Name).OrderBy(x => x.Name).Asc.Take(maxResults).List<string>());
        }

        [IsAllowed("/Applications")]
        public ActionResult ApplicationCodes(string q, int maxResults = DefaultMax)
        {
            return Json(new ApplicationQuery { CodeLike = q }.Over().Select(x => x.Code).OrderBy(x => x.Code).Asc.Take(maxResults).List<string>());
        }

        [IsAllowed("/Applications")]
        public ActionResult ApplicationNamesAndCodes(string q, int maxResults = DefaultMax)
        {
            var applicationNames = new ApplicationQuery { NameLike = q }.Over().Select(x => x.Name).OrderBy(x => x.Name).Asc.Take(maxResults).Future<string>();
            var applicationCodes = new ApplicationQuery { CodeLike = q }.Over().Select(x => x.Code).OrderBy(x => x.Code).Asc.Take(maxResults).Future<string>();
            return Json(applicationNames.Concat(applicationCodes).OrderBy(x => x).Take(maxResults));
        }

        [IsAllowed("/Applications/Events")]
        public ActionResult EventSources(string q, int maxResults = DefaultMax)
        {
            return Json(
                new EventQuery { SourceLike = q }
                .Over()
                .SelectList(x => x.Select(Projections.Distinct(Projections.Property<Event>(e => e.Source))))
                .OrderBy(x => x.Source).Asc
                .Take(maxResults)
                .List<string>());
        }

        [IsAllowed("/Servers", "/Applications/Servers", Operator = IsAllowedOperator.Or)]
        public ActionResult ServerNames(string q, int maxResults = DefaultMax)
        {
            return Json(new ServerQuery { NameLike = q }.Over().Select(x => x.Name).OrderBy(x => x.Name).Asc.Take(maxResults).List<string>());
        }

        [IsAllowed("/Servers", "/Applications/Servers", Operator = IsAllowedOperator.Or)]
        public ActionResult ServerAliases(string q, int maxResults = DefaultMax)
        {
            return Json(new ServerQuery { AliasLike = q }.Over().Select(x => x.Alias).OrderBy(x => x.Alias).Asc.Take(maxResults).List<string>());
        }

        [IsAllowed("/Servers", "/Applications/Servers", Operator = IsAllowedOperator.Or)]
        public ActionResult ServerNamesAndAliases(string q, int maxResults = DefaultMax)
        {
            var serverNames = new ServerQuery { NameLike = q }.Over().Select(x => x.Name).OrderBy(x => x.Name).Asc.Take(maxResults).Future<string>();
            var serverAliases = new ServerQuery { AliasLike = q }.Over().Select(x => x.Alias).OrderBy(x => x.Alias).Asc.Take(maxResults).Future<string>();
            return Json(serverNames.Concat(serverAliases).OrderBy(x => x).Take(maxResults));
        }

        [IsAllowed("/Technologies")]
        public ActionResult TechnologyNames(string q, int maxResults = DefaultMax)
        {
            return Json(
                new TechnologyQuery { NameLike = q }
                .Over()
                .SelectList(x => x.Select(Projections.Distinct(Projections.Property<Technology>(t => t.Name))))
                .OrderBy(x => x.Name).Asc
                .Take(maxResults)
                .List<string>());
        }
    }
}
