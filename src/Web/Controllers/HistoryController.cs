// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models.History;
using NHibernate.Criterion;
using NHibernate.Event;

namespace CGI.Reflex.Web.Controllers
{
    public class HistoryController : BaseController
    {
        [IsAllowed("/History")]
        public ActionResult View(string entity, int id, string returnUrl, string propertyName, AuditView model)
        {
            string className = ((NHibernate.Impl.SessionFactoryImpl)References.SessionFactory).GetImportedClassName(entity);

            if (string.IsNullOrEmpty(className) || className == entity)
                return HttpNotFound();

            model.EntityType = Type.GetType(className);

            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Timestamp";
                model.OrderType = OrderType.Desc;
            }

            model.SearchResults = new AuditInfoQuery
            {
                EntityType = model.EntityType == null ? string.Empty : model.EntityType.FullName, 
                EntityId = id, 
                PropertyName = propertyName, 
                DistinctRoot = true
            }
            .OrderBy(model.OrderBy, model.OrderType)
            .Fetch(a => a.User)
            .Fetch(a => a.Properties)
            .Paginate(model.Page);

            if (model.SearchResults.Items.Any())
                ViewBag.DisplayName = model.SearchResults.Items.OrderByDescending(m => m.Timestamp).First().DisplayName;

            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnUrl = returnUrl;
            else if (Request.UrlReferrer != null)
                ViewBag.ReturnUrl = Request.UrlReferrer.AbsoluteUri;
            else
                ViewBag.ReturnUrl = string.Empty;

            return View(model);
        }
    }
}