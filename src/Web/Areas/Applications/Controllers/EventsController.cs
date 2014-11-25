// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Events;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class EventsController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Events")]
        public ActionResult Details(int appId, EventsDetails model)
        {
            var application = NHSession.Get<Application>(appId);
            if (application == null)
                return HttpNotFound();

            if (string.IsNullOrEmpty(model.OrderBy) || model.OrderBy == "Date")
            {
                model.OrderBy = "Date";
                model.OrderType = OrderType.Desc;
            }

            model.OrderList = new[]
            {
                new { Name = "Date", OrderBy = "Date" }, 
                new { Name = "Type", OrderBy = "EventType.Name" }, 
                new { Name = "Source", OrderBy = "Source" }
            };

            model.AppName = application.Name;
            model.AppId = application.Id;
            model.SearchResults = new EventQuery
                                      {
                                          ApplicationId = application.Id, 
                                          SourceLike = model.Source, 
                                          DateFrom = model.DateFrom, 
                                          DateTo = model.DateTo, 
                                          TypeId = model.Type, 
                                          DescriptionLike = model.Description
                                      }
                                      .OrderBy(model.OrderBy, model.OrderType)
                                      .Paginate(model.Page, 10);

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);
            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Events/Create")]
        public ActionResult Create(int appId)
        {
            var model = new EventEdit();

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Events/Create")]
        [HttpPost]
        public ActionResult Create(int appId, EventEdit model)
        {
            var application = NHSession.Get<Application>(appId);
            if (application == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(model);

            var theEvent = new Event { Application = application };

            BindFromModel(model, theEvent);

            NHSession.Save(theEvent);

            Flash(FlashLevel.Success, "L'événement a été créé avec succès.");

            return RedirectToAction("Details");
        }

        [AppHeader]
        [IsAllowed("/Applications/Events/Update")]
        public ActionResult Edit(int id)
        {
            var theEvent = NHSession.Get<Event>(id);
            if (theEvent == null)
                return HttpNotFound();

            var model = new EventEdit();

            BindToModel(theEvent, model);

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Events/Update")]
        [HttpPost]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        public ActionResult Edit(int id, EventEdit model)
        {
            var theEvent = NHSession.Get<Event>(id);
            if (theEvent == null)
                return HttpNotFound();

            if (theEvent.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                      string.Format(
                            @"
                            <p>L'événement a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{0}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs et perdre mes modifications</a>
                            </p>
                        ",
                        Url.Action("Edit", new { id = theEvent.Id })), 
                    disableHtmlEscaping: true);
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            BindFromModel(model, theEvent);

            Flash(FlashLevel.Success, "L'événement été modifié avec succès.");

            return RedirectToAction("Details");
        }

        [IsAllowed("/Applications/Events/Delete")]
        public ActionResult Delete(int id)
        {
            var theEvent = NHSession.Get<Event>(id);
            if (theEvent == null)
                return HttpNotFound();

            var model = new EventEdit();

            BindToModel(theEvent, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/Applications/Events/Delete")]
        [HttpPost]
        public ActionResult Delete(EventEdit model)
        {
            var theEvent = NHSession.Get<Event>(model.Id);
            if (theEvent == null)
                return HttpNotFound();

            NHSession.Delete(theEvent);

            Flash(FlashLevel.Success, "L'événement a été supprimé.");

            return RedirectToAction("Details");
        }

        private void BindFromModel(EventEdit model, Event theEvent)
        {
            if (model.Date != null)
                theEvent.Date = model.Date.Value;

            theEvent.Description = model.Description;
            theEvent.Type = NHSession.Load<DomainValue>(model.Type);
            theEvent.Source = model.Source;
        }

        private void BindToModel(Event theEvent, EventEdit model)
        {
            model.Id = theEvent.Id;
            model.ConcurrencyVersion = theEvent.ConcurrencyVersion;

            model.Date = theEvent.Date;
            model.Description = theEvent.Description;
            model.Type = theEvent.Type.Id;
            model.Source = theEvent.Source;
        }
    }
}