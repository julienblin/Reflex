// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContactsController.cs" company="CGI">
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
using CGI.Reflex.Web.Areas.Organizations.Models.Contacts;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.Organizations.Controllers
{
    public class ContactsController : BaseController
    {
        [IsAllowed("/Organizations/Contacts")]
        public ActionResult Index(ContactsIndex model)
        {
            if (string.IsNullOrEmpty(model.OrderBy))
            {
                model.OrderBy = "FullName";
                model.OrderType = OrderType.Asc;
            }

            model.SearchResults = new ContactQuery
                                    {
                                        LastNameLike = model.LastName, 
                                        FirstNameLike = model.FirstName, 
                                        TypeId = model.TypeId, 
                                        SectorId = model.SectorId
                                    }
                                    .OrderBy(model.OrderBy, model.OrderType)
                                    .Paginate(model.Page);

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);
            return View(model);
        }

        [IsAllowed("/Organizations/Contacts/Create")]
        public ActionResult Create(string returnUrl, int? associateWithApplicationId)
        {
            var model = new ContactEdit
            {
                ReturnUrl = returnUrl,
                AssociateWithApplicationId = associateWithApplicationId,
                Compagnies = GetDistinctCompagnyNames()
            };

            return View(model);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Contacts/Create")]
        public ActionResult Create(ContactEdit model)
        {
            if (!ModelState.IsValid)
            {
                model.Compagnies = GetDistinctCompagnyNames();
                return View(model);
            }

            var contact = new Contact();
            BindFromModel(model, contact);

            NHSession.Save(contact);

            Flash(FlashLevel.Success, string.Format("Le contact {0} a été ajoutée avec succès", contact.FullName));
            if (model.AssociateWithApplicationId.HasValue)
            {
                var app = NHSession.Get<Application>(model.AssociateWithApplicationId);

                app.AddContactLink(contact);
                
                return RedirectToAction("Details", "Contacts", new { area = "Applications", appId = app.Id });
            }

            return RedirectToAction("Index");
        }

        [IsAllowed("/Organizations/Contacts/Update")]
        public ActionResult Edit(int id, string returnUrl, int? associateWithApplicationId)
        {
            var contact = NHSession.Get<Contact>(id);

            if (contact == null)
                return HttpNotFound();

            var model = new ContactEdit
            {
                ReturnUrl = returnUrl, 
                AssociateWithApplicationId = associateWithApplicationId
            };

            BindToModel(contact, model);
            model.Compagnies = GetDistinctCompagnyNames();

            return View(model);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Contacts/Update")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(ContactEdit model)
        {
            var contact = NHSession.Get<Contact>(model.Id);

            if (contact == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.Compagnies = GetDistinctCompagnyNames();
                return View(model);
            }

            if (contact.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>Le contact {0} a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs du contact et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(contact.FullName),
                        Url.Action("Edit", new { id = contact.Id })), 
                    disableHtmlEscaping: true);

                return View(model);
            }

            BindFromModel(model, contact);

            Flash(FlashLevel.Success, string.Format("Le contact {0} a été mis à jour avec succès.", contact.FullName));

            if (model.AssociateWithApplicationId.HasValue)
            {
                var app = NHSession.Get<Application>(model.AssociateWithApplicationId);

                app.AddContactLink(contact);

                return RedirectToAction("Details", "Contacts", new { area = "Applications", appId = app.Id });
            }

            return RedirectToAction("Index");
        }

        [IsAllowed("/Organizations/Contacts/Delete")]
        public ActionResult Delete(int id)
        {
            var contact = NHSession.Get<Contact>(id);

            if (contact == null)
                return HttpNotFound();

            var model = new ContactEdit();
            BindToModel(contact, model);

            ViewBag.ContactFullName = contact.FullName;

            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [IsAllowed("/Organizations/Contacts/Delete")]
        public ActionResult Delete(ContactEdit model)
        {
            var contact = NHSession.Get<Contact>(model.Id);
            if (contact == null)
                return HttpNotFound();

            // Using sub-session / sub-transaction to catch ConstraintViolationException
            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(contact);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    Flash(FlashLevel.Error, string.Format("Impossible de supprimer le contact {0} parce que certaines entités sont encore reliées.", contact.FullName));
                    return RedirectToAction("Index");
                }
            }

            Flash(FlashLevel.Success, string.Format("Le contact {0} a bien été supprimé.", contact.FullName));

            return RedirectToAction("Index");
        }

        private void BindToModel(Contact contact, ContactEdit model)
        {
            model.Id = contact.Id;
            model.ConcurrencyVersion = contact.ConcurrencyVersion;
            model.LastName = contact.LastName;
            model.FirstName = contact.FirstName;
            model.Company = contact.Company;
            model.TypeId = contact.Type.ToId();
            model.Email = contact.Email;
            model.PhoneNumber = contact.PhoneNumber;
            model.Notes = contact.Notes;
            model.SectorId = contact.Sector.ToId();
        }

        private void BindFromModel(ContactEdit model, Contact contact)
        {
            contact.LastName = model.LastName;
            contact.FirstName = model.FirstName;
            contact.Company = model.Company;
            contact.Type = model.TypeId.ToDomainValue();
            contact.Email = model.Email;
            contact.PhoneNumber = model.PhoneNumber;
            contact.Notes = model.Notes;
            contact.Sector = model.SectorId.HasValue ? NHSession.Load<Sector>(model.SectorId.Value) : null;
        }

        private IEnumerable<string> GetDistinctCompagnyNames()
        {
            return NHSession.QueryOver<Contact>()
                            .SelectList(list => list.Select(Projections.Distinct(Projections.Property<Contact>(c => c.Company))))
                            .OrderBy(c => c.Company).Asc
                            .List<string>();
        }
    }
}
