// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PickersController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Models.Pickers;
using NHibernate.Criterion;
using ContactsList = CGI.Reflex.Web.Models.Pickers.ContactsList;

namespace CGI.Reflex.Web.Controllers
{
    public class PickersController : BaseController
    {
        [IsAllowed("/Organizations/Contacts")]
        public ActionResult ContactsModal(ContactsModalOptions modalOptions)
        {
            var model = new ContactsList
            {
                PostUrl = modalOptions.PostUrl, 
                ReturnUrl = modalOptions.ReturnUrl, 
                AddFunctionName = modalOptions.AddFunctionName, 
                SelectionMode = modalOptions.SelectionMode, 
                ApplicationId = modalOptions.ApplicationId, 
                Contacts = NHSession.QueryOver<Contact>()
                                    .OrderBy(c => c.LastName).Asc
                                    .ThenBy(c => c.FirstName).Asc
                                    .ThenBy(c => c.Company).Asc
                                    .List()
            };

            return PartialView("_ContactsModal", model);
        }

        [IsAllowed("/Organizations/Contacts")]
        public ActionResult ContactDetails(int id)
        {
            var contact = NHSession.Get<Contact>(id);
            if (contact == null)
                return HttpNotFound();

            var model = new ContactDetails
            {
                Id = contact.Id, 
                FirstName = contact.FirstName, 
                LastName = contact.LastName, 
                Company = contact.Company, 
                Email = contact.Email, 
                ContactTypeId = contact.Type.ToId(), 
                PhoneNumber = contact.PhoneNumber, 
                Sector = contact.Sector != null ? contact.Sector.Name : string.Empty, 
                Notes = contact.Notes
            };

            return PartialView("_ContactDetails", model);
        }

        [IsAllowed("/Organizations/Sectors")]
        public ActionResult SectorsModal(SectorsModalOptions modalOptions)
        {
            var model = new SectorHierarchy
            {
                RootSectors = new SectorHierarchicalQuery().List(), 
                TargetUpdateId = modalOptions.TargetUpdateId
            };
            return PartialView("_SectorsModal", model);
        }

        [IsAllowed("/Organizations/Sectors")]
        public ActionResult SectorsTreeData(int rootId)
        {
            var sector = new SectorHierarchicalQuery { RootId = rootId, NumberOfEagerLoadingLevels = 2 }.SingleOrDefault();

            if (sector == null)
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            return Json(
                sector.Children.Select(s => new
                {
                    attr = new { id = string.Format("sector-{0}", s.Id) }, 
                    metadata = new { id = s.Id }, 
                    data = new { title = s.Name }, 
                    state = s.HasChildren() ? "closed" : string.Empty
                }), 
                JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Technologies")]
        public ActionResult TechnologiesModal(TechnologiesModalOptions modalOptions)
        {
            var model = new TechnologyHierarchy
            {
                RootTechnologies = new TechnologyHierarchicalQuery { NumberOfEagerLoadingLevels = 2 }.List(), 
                PostUrl = modalOptions.PostUrl, 
                SelectionMode = modalOptions.SelectionMode, 
                AddFunctionName = modalOptions.AddFunctionName, 
                TechnologyTypes = new DomainValueQuery { Category = DomainValueCategory.TechnologyType }.OrderBy(dv => dv.Name).List()
            };
            return PartialView("_TechnologiesModal", model);
        }

        [IsAllowed("/Technologies")]
        public ActionResult TechnologiesTreeData(int rootId, SelectionMode selectionMode)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = rootId, NumberOfEagerLoadingLevels = 2 }.SingleOrDefault();

            if (techno == null)
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            return Json(
                techno.Children.Select(t => new
                {
                    attr = new { id = string.Format("techno-{0}", t.Id), @class = (selectionMode == SelectionMode.Multiple || selectionMode == SelectionMode.SingleLeafOnly) && t.HasChildren() ? "no_checkbox" : string.Empty }, 
                    metadata = new { id = t.Id, fullname = t.FullName, technoType = t.TechnologyType != null ? t.TechnologyType.Id.ToString(CultureInfo.InvariantCulture) : string.Empty }, 
                    data = new { title = t.Name }, 
                    state = t.HasChildren() ? "closed" : string.Empty
                }), 
                JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Technologies")]
        public ActionResult TechnologiesSearch(string search, int? technoType)
        {
            var technos = new TechnologyQuery { NameLike = search, TechnologyTypeId = technoType }
                            .Fetch(t => t.Parent)
                            .Fetch(t => t.Parent.Parent)
                            .Fetch(t => t.Parent.Parent.Parent)
                            .Fetch(t => t.Parent.Parent.Parent.Parent)
                            .List().OrderBy(t => t.HierarchicalLevel);
            return Json(technos.SelectMany(t => t.AllParentIds).Distinct().Select(i => string.Format("#techno-{0}", i)).ToList(), JsonRequestBehavior.AllowGet);
        }

        [IsAllowed("/Technologies")]
        public ActionResult TechnologiesDetails(int id)
        {
            var techno = new TechnologyHierarchicalQuery { RootId = id }
                         .FetchParents()
                         .SingleOrDefault();
            if (techno == null)
                return HttpNotFound();

            var model = new TechnologyDetails
            {
                Name = techno.Name, 
                ParentFullName = techno.Parent != null ? techno.Parent.FullName : string.Empty, 
                EndOfSupport = techno.EndOfSupport, 
                TechnologyTypeId = techno.TechnologyType.ToId(), 
                HasChildren = techno.HasChildren(), 
                ContactId = techno.Contact.ToId(), 
                Description = techno.Description
            };

            return PartialView("_TechnologyDetails", model);
        }

        [IsAllowed("/Servers/DbInstances")]
        public ActionResult DbInstanceModal(DbInstancesModalOptions modalOptions)
        {
            var query = NHSession.QueryOver<DbInstance>();

            if (modalOptions.HideWithServer)
                query.Where(db => db.Server == null);

            if (modalOptions.HideInstanceIds != null && modalOptions.HideInstanceIds.Count > 0)
                query.WhereRestrictionOn(db => db.Id).Not.IsIn(modalOptions.HideInstanceIds);

            query = query.Fetch(db => db.Server).Eager;
            query = query.Fetch(db => db.TechnologyLinks).Eager;
            query = query.Fetch(db => db.TechnologyLinks.First()).Eager;

            var model = new DbInstanceList
            {
                PostUrl = modalOptions.PostUrl, 
                HideWithServer = modalOptions.HideWithServer, 
                DbInstances = query.OrderBy(db => db.Name).Asc.List(), 
                ServersWithInstances = new ServerQuery { WithDbInstances = true }.OrderBy(ls => ls.Name).List()
            };
            return PartialView("_DbInstanceModal", model);
        }

        [IsAllowed("/Servers")]
        public ActionResult ServersModal(ServersModalOptions modalOptions)
        {
            var query = NHSession.QueryOver<Server>();

            if (modalOptions.HideWithLandscape)
            {
                if (modalOptions.CurLandscapeId.HasValue)
                {
                    query.Where(s => s.Landscape == null || s.Landscape.Id == modalOptions.CurLandscapeId.Value);
                }
                else
                {
                    query.Where(s => s.Landscape == null);
                }
            }

            var model = new ServersList
            {
                PostUrl = modalOptions.PostUrl, 
                AddFunctionName = modalOptions.AddFunctionName, 
                SelectionMode = modalOptions.SelectionMode, 
                HideWithLandscape = modalOptions.HideWithLandscape, 
                HideServersFromTarget = modalOptions.HideServersFromTarget, 
                Servers = query.OrderBy(s => s.Name).Asc.List()
            };

            return PartialView("_ServersModal", model);
        }

        [IsAllowed("/Servers")]
        public ActionResult ServerDetails(int id)
        {
            var server = NHSession.Get<Server>(id);
            if (server == null)
                return HttpNotFound();

            var model = new ServerDetails
            {
                Id = server.Id, 
                Name = server.Name, 
                Alias = server.Alias, 
                Environment = server.Environment != null ? server.Environment.Name : string.Empty, 
                Role = server.Role != null ? server.Role.Name : string.Empty, 
                Status = server.Status != null ? server.Status.Name : string.Empty, 
                Type = server.Type != null ? server.Type.Name : string.Empty, 
                EvergreenDate = server.EvergreenDate.HasValue ? server.EvergreenDate.Value.ToShortDateString() : string.Empty, 
                Landscape = server.Landscape != null ? server.Landscape.Name : string.Empty, 
                Comments = server.Comments
            };

            return PartialView("_ServerDetails", model);
        }
    }
}
