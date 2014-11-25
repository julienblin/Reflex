// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConsoleController.cs" company="CGI">
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
using CGI.Reflex.Core.Commands;
using CGI.Reflex.Web.Areas.System.Models.DbConsole;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

using NHibernate.Transform;

using HttpContext = System.Web.HttpContext;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class DbConsoleController : BaseController
    {
        [IsAllowed("/System/DbConsole")]
        [ExcludeFromCodeCoverage]
        public ActionResult Index(DbQuery model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Query))
                return View("Index", model);

            try
            {
                var sqlQuery = NHSession.CreateSQLQuery(model.Query);
                switch (model.DbQueryAction)
                {
                    case DbQueryAction.Execute:
                        sqlQuery.SetResultTransformer(Transformers.AliasToEntityMap);
                        model.Result = sqlQuery.List();
                        break;
                    case DbQueryAction.Update:
                        var numberofRowAffected = sqlQuery.ExecuteUpdate();
                        Flash(FlashLevel.Success, string.Format("Number of row affected: {0}", numberofRowAffected));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Flash(FlashLevel.Error, "Erreur", ex.ToString());
            }

            return View("Index", model);
        }

        [HttpPost]
        [IsAllowed("/System/DbConsole")]
        [ExcludeFromCodeCoverage]
        public ActionResult ClearAllNHCaches()
        {
            References.DatabaseOperations.ClearAllNHCaches();
            Flash(FlashLevel.Success, "NHibernate caches cleared.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [IsAllowed("/System/DbConsole")]
        [ExcludeFromCodeCoverage]
        public ActionResult ClearAspnetCache()
        {
            var enumerator = ControllerContext.HttpContext.Cache.GetEnumerator();
            while (enumerator.MoveNext())
                ControllerContext.HttpContext.Cache.Remove((string)enumerator.Key);

            Flash(FlashLevel.Success, "ASP.Net cache cleared.");
            return RedirectToAction("Index");
        }
    }
}
