// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Contacts;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class ContactsController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Contacts")]
        public ActionResult Details(int appId, ContactsDetails model)
        {
            var application = NHSession.Get<Application>(appId);

            if (application == null)
                return HttpNotFound();

            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "Type.Order";
                model.OrderType = OrderType.Asc;
            }

            model.AppName = application.Name;
            model.AppId = application.Id;
            model.SearchResults = new AppContactLinkQuery
                                    {
                                        ApplicationId = application.Id
                                    }
                                    .OrderBy(model.OrderBy, model.OrderType)
                                    .Fetch(cl => cl.Application)
                                    .Fetch(cl => cl.Contact)
                                    .Fetch(cl => cl.Contact.Type)
                                    .Fetch(cl => cl.Contact.Sector)
                                    .Paginate(model.Page);

            return View(model);
        }

        [IsAllowed("/Applications/Contacts/Add")]
        public ActionResult AddContacts(int appId, IEnumerable<int> contacts)
        {
            var application = NHSession.QueryOver<Application>()
                                       .Where(a => a.Id == appId)
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .Fetch(a => a.ContactLinks.First().Contact).Eager
                                       .SingleOrDefault();
            if (application == null)
                return HttpNotFound();

            var numberOfContactsAdded = 0;
            if (contacts != null)
            {
                foreach (var contactId in contacts)
                {
                    var contact = NHSession.Load<Contact>(contactId);
                    if (!application.ContactLinks.Any(cl => cl.Contact == contact))
                    {
                        application.AddContactLink(contact);
                        ++numberOfContactsAdded;
                    }
                }
            }

            if (numberOfContactsAdded == 0)
                Flash(FlashLevel.Warning, string.Format("Aucun contact ajouté à l'application {0}.", application.Name));

            if (numberOfContactsAdded == 1)
                Flash(FlashLevel.Success, string.Format("Un contact a été ajouté à l'application {0}.", application.Name));

            if (numberOfContactsAdded > 1)
                Flash(FlashLevel.Success, string.Format("{0} contacts ont été ajoutés à l'application {1}.", numberOfContactsAdded, application.Name));

            return RedirectToAction("Details", new { appId = application.Id });
        }

        [IsAllowed("/Applications/Contacts/Update")]
        public ActionResult Edit(int id, string returnUrl, int? associateWithApplicationId)
        {
            AppContactLink link = NHSession.QueryOver<AppContactLink>()
                        .Where(cl => cl.Id == id)
                        .Fetch(cl => cl.Contact).Eager
                        .Fetch(cl => cl.Application).Eager
                        .SingleOrDefault();

            if (link == null)
                return HttpNotFound();

            var model = new ContactLinkEdit
            {
                ReturnUrl = returnUrl, 
                AssociateWithApplicationId = associateWithApplicationId
            };

            BindToModel(link, model);
            model.AvailableRoles = new DomainValueQuery { Category = DomainValueCategory.ContactRoleInApp }.List();
           
            return PartialView("_EditModal", model);
        }

        [HttpPost]
        [IsAllowed("/Applications/Contacts/Update")]
        public ActionResult Edit(ContactLinkEdit model)
        {
            var link = NHSession.Get<AppContactLink>(model.Id);
            if (link == null)
                return HttpNotFound();

            BindFromModel(model, link);

            Flash(FlashLevel.Success, string.Format("Le contact {0} a bien été mise à jour.", link.Contact.FullName));

            return JSRedirect(Url.Action("Details", "Contacts", new { appId = link.Application.Id }));
        }

        [IsAllowed("/Applications/Contacts/Remove")]
        public ActionResult RemoveLink(int linkId)
        {
            AppContactLink link = NHSession.QueryOver<AppContactLink>()
                                    .Where(cl => cl.Id == linkId)
                                    .Fetch(cl => cl.Contact).Eager
                                    .Fetch(cl => cl.Application).Eager
                                    .SingleOrDefault();

            if (link == null)
                return HttpNotFound();

            var model = new ContactLinkEdit();

            BindToModel(link, model);

            return PartialView("_RemoveLinkModal", model);
        }

        [HttpPost]
        [IsAllowed("/Applications/Contacts/Remove")]
        public ActionResult RemoveLink(ContactLinkEdit model)
        {
            AppContactLink link = NHSession.QueryOver<AppContactLink>()
                                    .Where(cl => cl.Id == model.Id)
                                    .Fetch(cl => cl.Contact).Eager
                                    .SingleOrDefault();

            if (link == null)
                return HttpNotFound();

            NHSession.Delete(link);

            Flash(FlashLevel.Success, string.Format("Le contact {0} a été retiré de l'application avec succès.", link.Contact.FullName));

            return RedirectToAction("Details");
        }

        private void BindToModel(AppContactLink link, ContactLinkEdit model)
        {
            model.Id = link.Id;
            model.ApplicationName = link.Application.Name;
            model.FullContactName = link.Contact.FullName;
            model.ContactId = link.Contact.Id;
            foreach (var dv in link.RolesInApp)
            {
                model.RoleInAppIds.Add(dv.Id);
            }
        }

        private void BindFromModel(ContactLinkEdit model, AppContactLink link)
        {
            var tempDv = model.RoleInAppIds.Select(roleId => NHSession.Load<DomainValue>(roleId)).ToList();
            link.SetRoleInApp(tempDv);
        }
    }
}