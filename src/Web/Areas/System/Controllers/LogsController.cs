// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.Logs;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate.Criterion;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class LogsController : BaseController
    {
        [IsAllowed("/System/Logs")]
        public ActionResult Index(LogsIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Date";
                model.OrderType = OrderType.Desc;
            }

            model.SearchResults = Map(model).OrderBy(model.OrderBy, model.OrderType).Paginate(model.Page);

            var loggers = NHSession.QueryOver<LogEntry>()
                                   .Select(Projections.Distinct(Projections.Property<LogEntry>(l => l.Logger)))
                                   .OrderBy(l => l.Logger).Asc
                                   .Future<string>();

            var levels = NHSession.QueryOver<LogEntry>()
                                  .Select(Projections.Distinct(Projections.Property<LogEntry>(l => l.Level)))
                                  .OrderBy(l => l.Level).Asc
                                  .Future<string>();

            var users = NHSession.QueryOver<LogEntry>()
                                  .Select(Projections.Distinct(Projections.Property<LogEntry>(l => l.LoggedUser)))
                                  .OrderBy(l => l.LoggedUser).Asc
                                  .Future<string>();

            model.Loggers = new[] { string.Empty }.Concat(loggers);
            model.Levels = new[] { string.Empty }.Concat(levels);
            model.Users = new[] { string.Empty }.Concat(users);

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);

            return View(model);
        }

        [IsAllowed("/System/Logs/Delete")]
        [HttpPost]
        public ActionResult DeleteSearch(LogsIndex model)
        {
            var logEntries = Map(model).List();

            foreach (var logEntry in logEntries)
            {
                NHSession.Delete(logEntry);
            }

            Flash(FlashLevel.Success, string.Format("{0} logs ont été supprimés.", logEntries.Count()));
            return RedirectToAction("Index");
        }

        [IsAllowed("/System/Logs/Delete")]
        public ActionResult DeleteModal(int id)
        {
            var logEntry = NHSession.Get<LogEntry>(id);
            if (logEntry == null)
                return HttpNotFound();

            return PartialView("_DeleteModal", logEntry);
        }

        [IsAllowed("/System/Logs/Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var logEntry = NHSession.Get<LogEntry>(id);
            if (logEntry == null)
                return HttpNotFound();

            NHSession.Delete(logEntry);

            Flash(FlashLevel.Success, "Le log a bien été supprimé.");
            return RedirectToAction("Index");
        }

        private static LogEntryQuery Map(LogsIndex model)
        {
            return new LogEntryQuery
            {
                CorrelationId = model.CorrelationId,
                Level = model.Level,
                Logger = model.Logger,
                User = model.User,
                MessageLike = model.Message,
                ContextLike = model.Context,
                ExceptionLike = model.Exception
            };
        }
    }
}