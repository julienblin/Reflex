// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewController.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Review;
using CGI.Reflex.Web.Areas.Applications.Filters;
using CGI.Reflex.Web.Areas.Applications.Models.Review;
using CGI.Reflex.Web.Charts;
using CGI.Reflex.Web.Infra.Controllers;
using CGI.Reflex.Web.Infra.Filters;
using CGI.Reflex.Web.Infra.Results;
using NHibernate;
using NHibernate.Transform;

namespace CGI.Reflex.Web.Areas.Applications.Controllers
{
    public class ReviewController : BaseController
    {
        [AppHeader]
        [IsAllowed("/Applications/Review")]
        public ActionResult Details(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                .Where(a => a.Id == appId)
                                .Fetch(a => a.TechnologyLinks).Eager
                                .Fetch(a => a.TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.ServerLinks).Eager
                                .Fetch(a => a.ServerLinks.First().Server).Eager
                                .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks).Eager
                                .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.IntegrationsAsDest).Eager
                                .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks).Eager
                                .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.IntegrationsAsSource).Eager
                                .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks).Eager
                                .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.ReviewAnswers).Eager
                                .Fetch(a => a.ReviewAnswers.First().Answer).Eager
                                .Fetch(a => a.ReviewAnswers.First().Answer.Question).Eager
                                .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var questions = NHSession.QueryOver<Question>()
                                .Fetch(q => q.Answers).Eager
                                .OrderBy(q => q.Type).Asc
                                .ThenBy(q => q.Name).Asc
                                .TransformUsing(Transformers.DistinctRootEntity)
                                .List();

            var questionAnswers = NHSession.QueryOver<Question>()
                                .OrderBy(q => q.Type).Asc
                                .ThenBy(q => q.Name).Asc
                                .List()
                                .Select(q => new ReviewItem
                                {
                                    Id = q.Id, 
                                    Type = q.Type, 
                                    Weight = q.Weight, 
                                    Question = q.Name, 
                                    Description = q.Description, 
                                    CurAnswer = q.GetAnswerDisplayByApplication(application), 
                                    CurAnswerExplanation = q.GetAnswerExplanationByApplication(application), 
                                    IsCalculation = q.Calculation != null
                                });

            var model = new ReviewEdit
            {
                AppName = application.Name,
                AppId = application.Id,
                BusinessQuestions = questionAnswers.Where(q => q.Type == QuestionType.BusinessValue).ToList(),
                TechnoQuestions = questionAnswers.Where(q => q.Type == QuestionType.TechnologyValue).ToList(),
                CurBusinessValue = application.GetBusinessValue(questions),
                CurTechnoValue = application.GetTechnologyValue(questions)
            };

            return View(model);
        }

        [AppHeader]
        [IsAllowed("/Applications/Review/Update")]
        public ActionResult Edit(int appId)
        {
            var application = NHSession.QueryOver<Application>()
                                .Where(a => a.Id == appId)
                                .Fetch(a => a.TechnologyLinks).Eager
                                .Fetch(a => a.TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.ServerLinks).Eager
                                .Fetch(a => a.ServerLinks.First().Server).Eager
                                .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks).Eager
                                .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.IntegrationsAsDest).Eager
                                .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks).Eager
                                .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.IntegrationsAsSource).Eager
                                .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks).Eager
                                .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks.First().Technology).Eager
                                .Fetch(a => a.ReviewAnswers).Eager
                                .Fetch(a => a.ReviewAnswers.First().Answer).Eager
                                .Fetch(a => a.ReviewAnswers.First().Answer.Question).Eager
                                .SingleOrDefault();

            if (application == null)
                return HttpNotFound();

            var questions = NHSession.QueryOver<Question>()
                                .OrderBy(q => q.Type).Asc
                                .ThenBy(q => q.Name).Asc
                                .Fetch(q => q.Answers).Eager
                                .TransformUsing(Transformers.DistinctRootEntity)
                                .List();

            var questionAnswers = NHSession.QueryOver<Question>()
                                .OrderBy(q => q.Type).Asc
                                .ThenBy(q => q.Name).Asc
                                .List()
                                .Select(q => new ReviewItem
                                {
                                    Id = q.Id, 
                                    Type = q.Type, 
                                    Weight = q.Weight, 
                                    Question = q.Name, 
                                    Description = q.Description, 
                                    CurAnswer = q.GetAnswerDisplayByApplication(application), 
                                    CurAnswerExplanation = q.GetAnswerExplanationByApplication(application), 
                                    Answers = q.Answers.OrderBy(a => a.Value).ToDictionary(a => a.Id, a => a.Name), 
                                    IsCalculation = q.Calculation != null, 
                                    SelectedAnswer = q.GetAnswerIdByApplication(application)
                                });

            var model = new ReviewEdit
            {
                AppName = application.Name,
                AppId = application.Id,
                BusinessQuestions = questionAnswers.Where(q => q.Type == QuestionType.BusinessValue).ToList(),
                TechnoQuestions = questionAnswers.Where(q => q.Type == QuestionType.TechnologyValue).ToList(),
                CurBusinessValue = application.GetBusinessValue(questions),
                CurTechnoValue = application.GetTechnologyValue(questions)
            };

            return View(model);
        }

        [HttpPost]
        [AppHeader]
        [IsAllowed("/Applications/Review/Update")]
        public ActionResult Edit(int appId, ReviewEdit model)
        {
            var application = NHSession.Get<Application>(appId);

            var questionsList = NHSession.QueryOver<Question>()
                                .Fetch(q => q.Answers).Eager
                                .TransformUsing(Transformers.DistinctRootEntity)
                                .List();

            UpdateAnswer(application, model.BusinessQuestions, questionsList);
            UpdateAnswer(application, model.TechnoQuestions, questionsList);

            Flash(FlashLevel.Success, string.Format("Le bilan de {0} a été mis à jour", application.Name));

            return RedirectToAction("Details");
        }

        [IsAllowed("/Applications/Review/Update")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult GetGraph(int appId, ReviewEdit model)
        {
            var reviewResult = new ApplicationsReviewResult();

            using (var subSession = NHSession.GetSession(EntityMode.Poco))
            using (var subTx = subSession.BeginTransaction())
            {
                var application = NHSession.QueryOver<Application>()
                                            .Where(a => a.Id == appId)
                                            .Fetch(a => a.TechnologyLinks).Eager
                                            .Fetch(a => a.TechnologyLinks.First().Technology).Eager
                                            .Fetch(a => a.ServerLinks).Eager
                                            .Fetch(a => a.ServerLinks.First().Server).Eager
                                            .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks).Eager
                                            .Fetch(a => a.ServerLinks.First().Server.TechnologyLinks.First().Technology).Eager
                                            .Fetch(a => a.IntegrationsAsDest).Eager
                                            .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks).Eager
                                            .Fetch(a => a.IntegrationsAsDest.First().TechnologyLinks.First().Technology).Eager
                                            .Fetch(a => a.IntegrationsAsSource).Eager
                                            .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks).Eager
                                            .Fetch(a => a.IntegrationsAsSource.First().TechnologyLinks.First().Technology).Eager
                                            .Fetch(a => a.ReviewAnswers).Eager
                                            .Fetch(a => a.ReviewAnswers.First().Answer).Eager
                                            .Fetch(a => a.ReviewAnswers.First().Answer.Question).Eager
                                            .SingleOrDefault();

                var questionsList = NHSession.QueryOver<Question>()
                    .Fetch(q => q.Answers).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();

                UpdateAnswer(application, model.BusinessQuestions, questionsList);
                UpdateAnswer(application, model.TechnoQuestions, questionsList);

                reviewResult.Resulsts = new[] { new ApplicationsReviewResultLine { BusinessValue = application.GetBusinessValue(questionsList), TechnologyValue = application.GetTechnologyValue(questionsList) } };

                subTx.Rollback();
            }

            var chart = new ApplicationsReviewsChart(200, 200) { Result = reviewResult };
            return new ChartResult(chart.Produce());
        }

        public string GetAnswerExplanation(int answerId)
        {
            var answer = NHSession.Get<QuestionAnswer>(answerId);

            return answer.Explanation;
        }

        private void UpdateAnswer(Application application, IEnumerable<ReviewItem> questionAnswers, IEnumerable<Question> allQuestions)
        {
            foreach (var questionAnswer in questionAnswers)
            {
                var question = allQuestions.Where(q => q.Id == questionAnswer.Id).Single();
                
                if (question.Calculation == null)
                {
                    QuestionAnswer answer = null;
                    if (questionAnswer.SelectedAnswer.HasValue)
                        answer = question.Answers.Where(a => a.Id == questionAnswer.SelectedAnswer.Value).SingleOrDefault();

                    application.AnswerQuestion(question, answer);
                }
            }
        }
    }
}