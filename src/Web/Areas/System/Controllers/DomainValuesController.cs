// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValuesController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.DomainValues;
using CGI.Reflex.Web.Helpers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using NHibernate;
using NHibernate.Exceptions;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class DomainValuesController : BaseController
    {
        [IsAllowed("/System/DomainValues")]
        public ActionResult Index(DomainValuesIndex model)
        {
            model.Items = new DomainValueQuery { Category = model.Category }.OrderBy(d => d.DisplayOrder).List().ToList();
            model.CategoryList = Enum.GetNames(typeof(DomainValueCategory))
                                     .Select(dvc => new { Category = dvc, Name = DisplayNameExtensions.EnumDisplayName((DomainValueCategory)Enum.Parse(typeof(DomainValueCategory), dvc)) })
                                     .OrderBy(dvc => dvc.Name)
                                     .ToList();

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);
            return View(model);
        }

        [IsAllowed("/System/DomainValues/Create")]
        public ActionResult Create(DomainValueCategory category)
        {
            var model = new DomainValueEdit { Category = category };
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/DomainValues/Create")]
        public ActionResult Create(DomainValueEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dv = new DomainValue();
            BindFromModel(model, dv);
            dv.DisplayOrder = new DomainValueQuery { Category = model.Category }.Over()
                                  .SelectList(sl => sl.SelectMax(d => d.DisplayOrder))
                                  .SingleOrDefault<int>() + 1;
            NHSession.Save(dv);

            Flash(FlashLevel.Success, string.Format("La valeur {0} a bien été créé.", dv.Name));
            return RedirectToAction("Index", new { dv.Category });
        }

        [IsAllowed("/System/DomainValues/Update")]
        public ActionResult Edit(int id)
        {
            var dv = NHSession.Get<DomainValue>(id);
            if (dv == null)
                return HttpNotFound();

            var model = new DomainValueEdit();
            BindToModel(dv, model);
            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/DomainValues/Update")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(DomainValueEdit model)
        {
            var dv = NHSession.Get<DomainValue>(model.Id);
            if (dv == null)
                return HttpNotFound();

            if (dv.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>La valeur {0} a été modifié par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(dv.Name),
                        Url.Action("Edit", new { id = dv.Id })), 
                    disableHtmlEscaping: true);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            BindFromModel(model, dv);

            Flash(FlashLevel.Success, string.Format("La valeur {0} a bien été mis à jour.", dv.Name));
            return RedirectToAction("Index", new { dv.Category });
        }

        [IsAllowed("/System/DomainValues/Delete")]
        public ActionResult Delete(int id)
        {
            var dv = NHSession.Get<DomainValue>(id);
            if (dv == null)
                return HttpNotFound();

            var model = new DomainValueEdit();
            BindToModel(dv, model);

            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [IsAllowed("/System/DomainValues/Delete")]
        public ActionResult Delete(DomainValueEdit model)
        {
            var dv = NHSession.Get<DomainValue>(model.Id);
            if (dv == null)
                return HttpNotFound();

            // Using sub-session / sub-transaction to catch ConstraintViolationException
            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                try
                {
                    NHSession.Delete(dv);
                    subTx.Commit();
                }
                catch (ConstraintViolationException)
                {
                    Flash(FlashLevel.Error, string.Format("Impossible de supprimer la valeur {0} parce que certaines entités sont encore reliées.", dv.Name));
                    return RedirectToAction("Index", new { dv.Category });
                }
            }

            Flash(FlashLevel.Success, string.Format("La valeur {0} a bien été supprimé.", dv.Name));
            return RedirectToAction("Index", new { dv.Category });
        }

        [HttpPost]
        [IsAllowed("/System/DomainValues/Update")]
        public ActionResult Reorder(int id, string direction)
        {
            var dv = NHSession.Get<DomainValue>(id);
            if (dv == null)
                return new HttpNotFoundResult();

            var allValuesInDomain = new DomainValueQuery { Category = dv.Category }.OrderBy(d => d.DisplayOrder).List().ToList();
            var currentIndex = allValuesInDomain.IndexOf(dv);
            if (direction == "up" && currentIndex > 0)
            {
                allValuesInDomain.RemoveAt(currentIndex);
                allValuesInDomain.Insert(currentIndex - 1, dv);
            }

            if (direction == "down" && currentIndex < (allValuesInDomain.Count - 1))
            {
                allValuesInDomain.RemoveAt(currentIndex);
                allValuesInDomain.Insert(currentIndex + 1, dv);
            }

            // reconstruct DisplayOrder
            for (int i = 0; i < allValuesInDomain.Count; i++)
                allValuesInDomain[i].DisplayOrder = i;

            if (Request.IsAjaxRequest())
            {
                var indexModel = new DomainValuesIndex
                {
                    Category = dv.Category, 
                    Items = allValuesInDomain
                };
                indexModel.CategoryList = Enum.GetNames(typeof(DomainValueCategory))
                          .Select(dvc => new { Category = dvc, Name = DisplayNameExtensions.EnumDisplayName((DomainValueCategory)Enum.Parse(typeof(DomainValueCategory), dvc)) })
                          .OrderBy(dvc => dvc.Name)
                          .ToList();

                ViewData["Category"] = dv.Category;
                return PartialView("_List", indexModel);
            }

            return RedirectToAction("Index", new { dv.Category });
        }

        private void BindToModel(DomainValue domain, DomainValueEdit model)
        {
            model.Id = domain.Id;
            model.ConcurrencyVersion = domain.ConcurrencyVersion;
            model.Category = domain.Category;
            model.Name = domain.Name;
            model.Description = domain.Description;
            model.Color = domain.Color.IsEmpty ? null : ColorTranslator.ToHtml(domain.Color);
        }

        private void BindFromModel(DomainValueEdit model, DomainValue domain)
        {
            domain.Category = model.Category;
            domain.Name = model.Name;
            domain.Description = model.Description;
            domain.Color = string.IsNullOrEmpty(model.Color) ? Color.Empty : ColorTranslator.FromHtml(model.Color);
        }
    }
}
