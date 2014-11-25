// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionsControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.System.Controllers;
using CGI.Reflex.Web.Areas.System.Models.Questions;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.System.Controllers
{
    public class QuestionsControllerTest : BaseControllerTest<QuestionsController>
    {
        [Test]
        public void Index_should_work_with_no_value_and_return_view()
        {
            StubStandardRequest();
            var result = Controller.Index(new QuestionsIndex());
            result.Should().BeDefaultView();
            result.Model<QuestionsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Index_should_return_partial_view_when_ajax()
        {
            StubAjaxRequest();
            var result = Controller.Index(new QuestionsIndex());
            result.Should().BePartialView("_List");
            result.Model<QuestionsIndex>().Should().NotBeNull();
        }

        [Test]
        public void Index_should_filter_by_type()
        {
            var type1 = Rand.Enum<QuestionType>();
            var type2 = Rand.Enum<QuestionType>();
            while (type2 == type1)
                type2 = Rand.Enum<QuestionType>();

            Factories.Question.Save(q => q.Type = type1);
            Factories.Question.Save(q => q.Type = type1);
            Factories.Question.Save(q => q.Type = type1);

            Factories.Question.Save(q => q.Type = type2);
            Factories.Question.Save(q => q.Type = type2);

            StubStandardRequest();
            var model = Controller.Index(new QuestionsIndex { Type = type1 }).Model<QuestionsIndex>();

            model.Should().NotBeNull();
            model.Items.Should().HaveCount(3);
        }

        [Test]
        public void Create_should_return_model_based_on_type()
        {
            var type = Rand.Enum<QuestionType>();

            var result = Controller.Create(type);

            result.Should().BeDefaultView();
            result.Model<QuestionEdit>().Type.Should().Be(type);
            result.Model<QuestionEdit>().FormAction.Should().Be("Create");
        }

        [Test]
        public void Create_should_create_Question()
        {
            var model = new QuestionEdit
            {
                Name = Rand.String(),
                Description = Rand.LoremIpsum(),
                Type = Rand.Enum<QuestionType>(),
                Answers = new[]
                {
                    new AnswerEdit { Value = Rand.Int(int.MaxValue), Answer = Rand.String(20), Explanation = Rand.String(200) },
                    new AnswerEdit { Value = Rand.Int(int.MaxValue), Answer = Rand.String(10), Explanation = Rand.String(20) },
                    new AnswerEdit { Value = Rand.Int(int.MaxValue), Answer = Rand.String(30), Explanation = string.Empty }
                }.ToList()
            };

            var result = Controller.Create(model);
            result.Should().BeRedirectToActionName("Index");
            ((RedirectToRouteResult)result).RouteValues.Should().ContainKey("type");
            ((RedirectToRouteResult)result).RouteValues["type"].Should().Be(model.Type);

            var question = NHSession.QueryOver<Question>().SingleOrDefault();

            question.Name.Should().Be(model.Name);
            question.Description.Should().Be(model.Description);
            question.Type.Should().Be(model.Type);
            question.Answers.Should().HaveCount(3);

            for (int i = 0; i < question.Answers.Count(); i++)
            {
                var answer = question.Answers.Where(a => a.Name == model.Answers[i].Answer).SingleOrDefault();

                answer.Explanation.Should().Be(model.Answers[i].Explanation);
                answer.Value.Should().Be(model.Answers[i].Value);
            }
        }

        [Test]
        public void Edit_should_return_not_found_if_not_exist()
        {
            var result = Controller.Edit(Rand.Int(int.MaxValue));

            result.Should().BeHttpNotFound();
        }

        [Test]
        public void Edit_should_return_view()
        {
            var question = Factories.Question.Save();

            Factories.QuestionAnswer.Save(a => a.Question = question);
            Factories.QuestionAnswer.Save(a => a.Question = question);

            NHSession.Refresh(question);

            var result = Controller.Edit(question.Id);

            result.Should().BeDefaultView();

            var model = result.Model<QuestionEdit>();
            model.FormAction.Should().Be("Edit");

            model.Name.Should().Be(question.Name);
            model.Description.Should().Be(question.Description);
            model.Answers.Count().Should().Be(2);

            for (int i = 0; i < question.Answers.Count(); i++)
            {
                var answer = model.Answers.Where(a => a.Id == question.Answers.ToList()[i].Id).SingleOrDefault();

                answer.Value = question.Answers.ToList()[i].Value;
                answer.Answer = question.Answers.ToList()[i].Name;
                answer.Explanation = question.Answers.ToList()[i].Explanation;
            }
        }

        [Test]
        public void Edit_post_should_return_not_found()
        {
            var result = Controller.Edit(new QuestionEdit { Id = Rand.Int(int.MaxValue) });

            result.Should().BeHttpNotFound();
        }

        [Test]
        public void Edit_should_update_question()
        {
            var question = Factories.Question.Save();

            var answer1 = Factories.QuestionAnswer.Save(a => a.Question = question);
            var answer2 = Factories.QuestionAnswer.Save(a => a.Question = question);
            var answer3 = Factories.QuestionAnswer.Save(a => a.Question = question);

            NHSession.Refresh(question);

            var model = new QuestionEdit
            {
                Id = question.Id,
                Name = Rand.String(10),
                Type = question.Type,
                Description = Rand.String(555),
                ConcurrencyVersion = question.ConcurrencyVersion,
                Answers = new[]
                {
                    new AnswerEdit { Id = answer1.Id, Value = answer1.Value, Answer = answer1.Name, Explanation = answer1.Explanation },
                    new AnswerEdit { Id = 0, Value = Rand.Int(int.MaxValue), Answer = Rand.String(50), Explanation = Rand.String(155) },
                }.ToList()
            };

            var result = Controller.Edit(model);
            result.Should().BeRedirectToActionName("Index");
            ((RedirectToRouteResult)result).RouteValues.Should().ContainKey("type");
            ((RedirectToRouteResult)result).RouteValues["type"].Should().Be(model.Type);

            NHSession.Flush();

            NHSession.Refresh(question);

            question.Name.Should().Be(model.Name);
            question.Description.Should().Be(model.Description);
            question.Answers.Count().Should().Be(2);

            question.Answers.Should().Contain(answer1);
            question.Answers.Should().NotContain(answer2);
            question.Answers.Should().NotContain(answer3);

            var ans = question.Answers.Where(a => a != answer1).SingleOrDefault();

            ans.Value.Should().Be(model.Answers[1].Value);
            ans.Name.Should().Be(model.Answers[1].Answer);
            ans.Explanation.Should().Be(model.Answers[1].Explanation);
        }

        [Test]
        public void Edit_should_check_concurrency_version()
        {
            var question = Factories.Question.Save();

            var answer1 = Factories.QuestionAnswer.Save(a => a.Question = question);
            var answer2 = Factories.QuestionAnswer.Save(a => a.Question = question);
            var answer3 = Factories.QuestionAnswer.Save(a => a.Question = question);

            NHSession.Refresh(question);

            var model = new QuestionEdit
            {
                Id = question.Id,
                Name = Rand.String(10),
                Type = question.Type,
                Description = Rand.String(555),
                ConcurrencyVersion = question.ConcurrencyVersion - 1,
                Answers = new AnswerEdit[] { }.ToList()
            };

            StubStandardRequest();

            var result = Controller.Edit(model);

            NHSession.Flush();
            NHSession.Refresh(question);

            result.Should().BeDefaultView();

            question.Name.Should().NotBe(model.Name);
            question.Description.Should().NotBe(model.Description);
            question.Answers.Count().Should().Be(3);
        }

        [Test]
        public void Delete_should_return_not_found()
        {
            var result = Controller.Delete(Rand.Int(int.MaxValue));

            result.Should().BeHttpNotFound();
        }

        [Test]
        public void AddAnswer_should_add_answer()
        {
            var model = new QuestionEdit
            {
                Name = Rand.String(10),
                Description = Rand.LoremIpsum(),
                Answers = new[]
                {
                    new AnswerEdit { },
                    new AnswerEdit { }
                }.ToList()
            };

            var result = Controller.AddAnswer(model);

            result.Model<QuestionEdit>().Answers.Count().Should().Be(3);
        }

        [Test]
        public void RemoveAnswer_should_remove_answer()
        {
            var model = new QuestionEdit
            {
                Name = Rand.String(10),
                Description = Rand.LoremIpsum(),
                Answers = new[]
                {
                    new AnswerEdit { },
                    new AnswerEdit { }
                }.ToList()
            };

            var result = Controller.RemoveAnswer(model, Rand.Int(1));

            result.Model<QuestionEdit>().Answers.Count().Should().Be(1);
        }

        [Test]
        public void UpdateWeight_should_not_update_or_delete_when_total_is_not_100()
        {
            var type = Rand.Enum<QuestionType>();

            var question1 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question2 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question3 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question4 = Factories.Question.Save(q => { q.Type = type; });

            var oldQuestion1Weight = question1.Weight;

            question4.Weight = 100 - (question1.Weight + question2.Weight + question3.Weight);

            var model = new QuestionsIndex()
            {
                Type = type,
                Items = new[]
                {
                    new QuestionWeightEdit { Id = question1.Id, Weight = question1.Weight + 1 },
                    new QuestionWeightEdit { Id = question2.Id, Weight = question2.Weight },
                    new QuestionWeightEdit { Id = question3.Id, Weight = question3.Weight },
                    new QuestionWeightEdit { Id = question4.Id, Weight = question4.Weight, ToDelete = true }
                }.ToList()
            };

            var result = Controller.UpdateWeight(model);

            NHSession.Flush();

            question1.Weight.Should().Be(oldQuestion1Weight);

            NHSession.QueryOver<Question>().List().Count().Should().Be(4);
        }

        [Test]
        public void UpdateWeight_should_update_and_delete_when_total_is_100()
        {
            var type = Rand.Enum<QuestionType>();

            var question1 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question2 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question3 = Factories.Question.Save(q => { q.Type = type; q.Weight = Rand.Int(33); });
            var question4 = Factories.Question.Save(q => { q.Type = type; });

            question4.Weight = 100 - (question1.Weight + question2.Weight + question3.Weight);

            var oldQuestion1Weight = question1.Weight;
            var question4Weight = question4.Weight;

            var model = new QuestionsIndex()
            {
                Type = type,
                Items = new[]
                {
                    new QuestionWeightEdit { Id = question1.Id, Weight = question1.Weight + question4.Weight },
                    new QuestionWeightEdit { Id = question2.Id, Weight = question2.Weight },
                    new QuestionWeightEdit { Id = question3.Id, Weight = question3.Weight },
                    new QuestionWeightEdit { Id = question4.Id, Weight = question4.Weight, ToDelete = true }
                }.ToList()
            };

            var result = Controller.UpdateWeight(model);

            NHSession.Flush();

            question1.Weight.Should().Be(oldQuestion1Weight + question4Weight);

            NHSession.QueryOver<Question>().List().Count().Should().Be(3);
        }
    }
}
