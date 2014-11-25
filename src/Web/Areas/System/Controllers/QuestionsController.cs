// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionsController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Calculation;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using CGI.Reflex.Web.Areas.System.Models.Questions;
using CGI.Reflex.Web.Commands;
using CGI.Reflex.Web.Helpers;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;

namespace CGI.Reflex.Web.Areas.System.Controllers
{
    public class QuestionsController : BaseController
    {
        [IsAllowed("/System/Questions")]
        public ActionResult Index(QuestionsIndex model)
        {
            model.Items = new QuestionQuery
                            {
                                Type = model.Type
                            }
                            .OrderBy(q => q.Name)
                            .List()
                            .Select(m => new QuestionWeightEdit
                            {
                                Id = m.Id, 
                                Name = m.Name, 
                                Description = m.Description, 
                                Weight = m.Weight
                            })
                            .ToList();

            model.QuestionTypeList = GetQuestionTypeList();

            if (Request.IsAjaxRequest())
                return PartialView("_List", model);
            return View(model);
        }

        [IsAllowed("/System/Questions/Update")]
        public ActionResult UpdateWeight(QuestionsIndex model)
        {
            if (model.Items == null)
                return RedirectToAction("Index", new { type = model.Type });

            if (!ModelState.IsValid)
            {
                model.QuestionTypeList = GetQuestionTypeList();

                return View("Index", model);
            }

            int totalWeight = 0;
            bool deleteMode = false;
            foreach (var questionWeight in model.Items)
            {
                if (!questionWeight.ToDelete)
                {
                    totalWeight += questionWeight.Weight;
                }
                else
                {
                    questionWeight.Weight = 0;
                    deleteMode = true;
                }
            }

            if (totalWeight != 100 && model.Items.Where(q => q.ToDelete == false).Count() != 0)
            {
                model.QuestionTypeList = GetQuestionTypeList();
                if (!deleteMode)
                    Flash(FlashLevel.Error, "Le total doit être égale à 100%");
                else
                    Flash(FlashLevel.Error, "Le total doit être égale à 100. Veuillez ajuster les pourcentages de chaque question et cliquer sur sauvegarder pour compléter la suppression.");

                ModelState.Clear();
                return View("Index", model);
            }

            foreach (var questionWeight in model.Items)
            {
                var question = NHSession.Load<Question>(questionWeight.Id);

                if (!questionWeight.ToDelete)
                    question.Weight = questionWeight.Weight;
                else
                    NHSession.Delete(question);
            }

            Flash(FlashLevel.Success, "Sauvegarde effectué avec succès.");

            return RedirectToAction("Index", new { type = model.Type });
        }

        [IsAllowed("/System/Questions/Create")]
        public ActionResult Create(QuestionType type)
        {
            var model = new QuestionEdit { Type = type };

            ViewBag.AutoCalculations = GetCalculations(model.Type);

            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/Questions/Create")]
        public ActionResult Create(QuestionEdit model)
        {
            ViewBag.AutoCalculations = GetCalculations(model.Type);

            if (!ModelState.IsValid)
                return View(model);

            var questionCount = NHSession.QueryOver<Question>().Where(q => q.Type == model.Type).List().Count();

            var question = new Question { Weight = questionCount == 0 ? 100 : 0 };
            BindFromModel(model, question);

            NHSession.Save(question);

            Flash(FlashLevel.Success, string.Format("La question {0} a bien été créé.", question.Name));

            return RedirectToAction("Index", new { type = model.Type });
        }

        [IsAllowed("/System/Questions/Update")]
        public ActionResult Edit(int id)
        {
            var question = NHSession.Get<Question>(id);
            if (question == null)
                return HttpNotFound();

            ViewBag.AutoCalculations = GetCalculations(question.Type);

            var model = new QuestionEdit();
            BindToModel(question, model);

            return View(model);
        }

        [HttpPost]
        [IsAllowed("/System/Questions/Update")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed.")]
        public ActionResult Edit(QuestionEdit model)
        {
            ViewBag.AutoCalculations = GetCalculations(model.Type);

            if (!ModelState.IsValid)
                return View(model);

            var question = NHSession.Get<Question>(model.Id);
            if (question == null)
                return HttpNotFound();

            if (question.ConcurrencyVersion > model.ConcurrencyVersion)
            {
                Flash(
                    FlashLevel.Warning, 
                    string.Format(
                        @"
                            <p>La question {0} a été modifiée par un autre utilisateur entre-temps. Il est impossible de soumettre la modification.<p>
                            <p>
                                <b>Veuillez noter tous vos changements</b> et cliquer sur
                                <a href='{1}' class='btn btn-danger'><i class='icon-warning-sign icon-white'></i> Afficher les nouvelles valeurs et perdre mes modifications</a>
                            </p>
                        ",
                        HttpContext.Server.HtmlEncode(question.Name),
                        Url.Action("Edit", new { id = question.Id })), 
                    disableHtmlEscaping: true);
                return View(model);
            }

            BindFromModel(model, question);

            Flash(FlashLevel.Success, string.Format("La question {0} a bien été mise à jour.", question.Name));

            return RedirectToAction("Index", new { type = model.Type });
        }

        [IsAllowed("/System/Questions/Delete")]
        public ActionResult Delete(int id)
        {
            var question = NHSession.Get<Question>(id);

            if (question == null)
                return HttpNotFound();

            var model = new QuestionEdit();
            BindToModel(question, model);

            return PartialView("_DeleteModal", model);
        }

        [IsAllowed("/System/Questions/Update")]
        public ActionResult AddAnswer(QuestionEdit model)
        {
            ViewBag.AutoCalculations = GetCalculations(model.Type);

            int maxValue = 0;

            if (model.Answers.Count() > 0)
                maxValue = model.Answers.Max(a => a.Value) + 1;

            model.Answers.Add(new AnswerEdit { Value = maxValue });

            ModelState.Clear();

            return PartialView("_AnswerList", model);
        }

        [IsAllowed("/System/Questions/Update")]
        public ActionResult RemoveAnswer(QuestionEdit model, int answerToRemove)
        {
            ViewBag.AutoCalculations = GetCalculations(model.Type);

            model.Answers.RemoveAt(answerToRemove);

            ModelState.Clear();

            return PartialView("_AnswerList", model);
        }

        private IEnumerable<object> GetQuestionTypeList()
        {
            return Enum.GetNames(typeof(QuestionType))
                         .Select(dvc => new { QuestionType = dvc, Name = DisplayNameExtensions.EnumDisplayName((QuestionType)Enum.Parse(typeof(QuestionType), dvc)) })
                         .OrderBy(dvc => dvc.Name)
                         .ToList();
        }

        private IDictionary<string, string> GetCalculations(QuestionType type)
        {
            return Execute<GetCalculationsCommand, IDictionary<string, string>>(c => c.Type = type);
        }
        
        private void BindFromModel(QuestionEdit model, Question question)
        {
            question.Type = model.Type;
            question.Name = model.Name;
            question.Description = model.Description;

            var answerList = new List<QuestionAnswer>();
            foreach (var a in model.Answers)
            {
                var qa = a.Id == 0 ? new QuestionAnswer { Question = question } : NHSession.Load<QuestionAnswer>(a.Id);
                qa.Name = a.Answer;
                qa.Explanation = a.Explanation;
                qa.Value = a.Value;
                answerList.Add(qa);
            }

            question.SetQuestionAnswers(answerList);

            if (!string.IsNullOrEmpty(model.Calculation))
            {
                question.Calculation = (ICalculation)Activator.CreateInstance(typeof(ICalculation).Assembly.GetType(model.Calculation));
            }
            else
            {
                question.Calculation = null;
            }
        }

        private void BindToModel(Question question, QuestionEdit model)
        {
            model.Id = question.Id;
            model.ConcurrencyVersion = question.ConcurrencyVersion;
            model.Type = question.Type;
            model.Name = question.Name;
            model.Description = question.Description;
            model.Answers = question.Answers
                            .OrderBy(a => a.Value)
                            .Select(a => new AnswerEdit
                            {
                                Id = a.Id, 
                                Answer = a.Name, 
                                Explanation = a.Explanation, 
                                Value = a.Value
                            })
                            .ToList();
            
            model.Calculation = question.Calculation != null ? question.Calculation.GetType().FullName : string.Empty;
        }
    }
}