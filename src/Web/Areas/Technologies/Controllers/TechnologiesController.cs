// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologiesController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.Technologies.Models;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Technologies.Controllers
{
    public class TechnologiesController : BaseController
    {
        [IsAllowed("/Technologies")]
        public ActionResult Index()
        {
            var model = new TechnologyHierarchy
            {
                RootTechnologies = new TechnologyHierarchicalQuery { NumberOfEagerLoadingLevels = 2 }.List(), 
            };
            return View(model);
        }

        [IsAllowed("/Technologies")]
        public ActionResult TreeData(int rootId)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = rootId, NumberOfEagerLoadingLevels = 2 }.SingleOrDefault();

            if (techno == null)
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            return Json(
                techno.Children.Select(t => new
                {
                    attr = new { id = string.Format("techno-{0}", t.Id) }, 
                    metadata = new
                                   {
                                       id = t.Id, 
                                       fullname = t.FullName, 
                                       technoType = t.TechnologyType != null ? t.TechnologyType.Id.ToString(CultureInfo.InvariantCulture) : string.Empty, 
                                       endOfSupport = t.EndOfSupport.HasValue ? t.EndOfSupport.Value.ToString("yyyy-MM-dd") : string.Empty, 
                                       contactId = t.Contact.ToId()
                                   }, 
                    data = new { title = t.Name }, 
                    state = t.HasChildren() ? "closed" : string.Empty
                }).ToList(), 
                JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Technologies")]
        public ActionResult Search(string name, int? technoType, DateTime? dateFrom, DateTime? dateTo, int? contactId)
        {
            var technos = new TechnologyQuery
                              {
                                  NameLike = name, 
                                  TechnologyTypeId = technoType, 
                                  DateFrom = dateFrom, 
                                  DateTo = dateTo, 
                                  ContactId = contactId
                              }
                            .Fetch(t => t.Parent)
                            .Fetch(t => t.Parent.Parent)
                            .Fetch(t => t.Parent.Parent.Parent)
                            .Fetch(t => t.Parent.Parent.Parent.Parent)
                            .List().OrderBy(t => t.HierarchicalLevel);

            return Json(technos.SelectMany(t => t.AllParentIds).Distinct().Select(i => string.Format("#techno-{0}", i)).ToList(), JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Technologies")]
        public ActionResult Details(int id)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = id }
                .FetchParents()
                .Fetch(t => t.ApplicationLinks).Eager
                .Fetch(t => t.ServerLinks).Eager
                .SingleOrDefault();

            if (techno == null)
                return HttpNotFound();

            var model = new TechnologyEdit();
            BindToModel(techno, model);

// model.ApplicationCount = techno.ApplicationLinks.Count();
            // model.ServerCount = techno.ServerLinks.Count();
            var appCount = new ApplicationQuery { LinkedTechnologyId = techno.Id }.Over()
                .Select(Projections.RowCount()).FutureValue<int>();

            var serverCount = new ServerQuery { LinkedTechnologyId = techno.Id }.Over()
                .Select(Projections.RowCount()).FutureValue<int>();

            model.ApplicationCount = appCount.Value;
            model.ServerCount = serverCount.Value;

            return PartialView("_Details", model);
        }

        [IsAllowed("/Technologies/Create")]
        public ActionResult Create(int parentId)
        {
            Technology technoParent = null;
            if (parentId != 0)
            {
                technoParent = new TechnologyHierarchicalQuery { RootId = parentId }.FetchParents().SingleOrDefault();
                if (technoParent == null)
                    return new HttpNotFoundResult();
            }

            var model = new TechnologyEdit { ParentId = parentId };
            if (technoParent != null)
                model.ParentFullName = technoParent.FullName;

            return PartialView("_Form", model);
        }

        [HttpPost]
        [IsAllowed("/Technologies/Create")]
        public ActionResult Create(TechnologyEdit model)
        {
            Technology technoParent = null;
            if (model.ParentId != 0)
            {
                technoParent = new TechnologyHierarchicalQuery { RootId = model.ParentId }.FetchParents().SingleOrDefault();
                if (technoParent == null)
                    return new HttpNotFoundResult();
            }

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            var techno = new Technology();
            BindFromModel(model, techno);

            if (technoParent != null)
            {
                technoParent.EndOfSupport = null;
                technoParent.AddChild(techno);
            }

            NHSession.Save(techno);

            BindToModel(techno, model);
            model.JustCreated = true;
            return PartialView("_Details", model);
        }

        [IsAllowed("/Technologies/Update")]
        public ActionResult Edit(int id)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = id }.FetchParents().SingleOrDefault();
            if (techno == null)
                return new HttpNotFoundResult();

            var model = new TechnologyEdit();
            BindToModel(techno, model);

            return PartialView("_Form", model);
        }

        [HttpPost]
        [IsAllowed("/Technologies/Update")]
        public ActionResult Edit(TechnologyEdit model)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = model.Id }.FetchParents().SingleOrDefault();
            if (techno == null)
                return new HttpNotFoundResult();

            if (!ModelState.IsValid)
                return PartialView("_Form", model);

            BindFromModel(model, techno);

            Flash(FlashLevel.Success, string.Format("La technologie {0} a bien été mise à jour.", techno.FullName));
            return PartialView("_Details", model);
        }

        [HttpPost]
        [IsAllowed("/Technologies/Delete")]
        public ActionResult Delete(int id)
        {
            var techno = NHSession.Get<Technology>(id);
            if (techno == null)
                return HttpNotFound();

            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(techno);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    Flash(FlashLevel.Error, string.Format("Impossible de supprimer la technologie {0} parce que certaines entités sont encore reliées.", techno.FullName));
                    return new HttpStatusCodeResult((int)HttpStatusCode.NotAcceptable);
                }
            }

            Flash(FlashLevel.Success, string.Format("La technologie {0} a bien été supprimée.", techno.FullName));
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        private void BindToModel(Technology techno, TechnologyEdit model)
        {
            model.Id = techno.Id;
            model.Name = techno.Name;
            model.ParentFullName = techno.Parent != null ? techno.Parent.FullName : string.Empty;
            model.EndOfSupport = techno.EndOfSupport;
            model.TechnologyTypeId = techno.TechnologyType.ToId();
            model.ContactId = techno.Contact.ToId();
            model.Description = techno.Description;
            model.HasChildren = techno.HasChildren();
        }

        private void BindFromModel(TechnologyEdit model, Technology techno)
        {
            techno.Name = model.Name;
            techno.EndOfSupport = model.EndOfSupport;
            techno.TechnologyType = model.TechnologyTypeId.ToDomainValue();
            techno.Contact = model.ContactId.HasValue ? NHSession.Load<Contact>(model.ContactId.Value) : null;
            techno.Description = model.Description;
            model.ParentFullName = techno.Parent != null ? techno.Parent.FullName : string.Empty;
            model.HasChildren = techno.HasChildren();
        }
    }
}
