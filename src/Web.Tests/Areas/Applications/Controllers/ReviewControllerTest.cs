// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Calculation;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Areas.Applications.Controllers;
using CGI.Reflex.Web.Areas.Applications.Models;
using CGI.Reflex.Web.Areas.Applications.Models.Review;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Areas.Applications.Controllers
{
    public class ReviewControllerTest : BaseControllerTest<ReviewController>
    {
        private int question1Id;

        [Test]
        public void Details_should_return_not_found()
        {
            Controller.Details(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
        }

        [Test]
        public void Details_should_return_default_view_when_empty()
        {
            var application = Factories.Application.Save();

            var result = Controller.Details(application.Id);

            result.Should().BeDefaultView();

            var model = result.Model<ReviewEdit>();

            model.AppName.Should().Be(application.Name);

            model.BusinessQuestions.Should().NotBeNull();
            model.BusinessQuestions.Should().BeEmpty();

            model.TechnoQuestions.Should().NotBeNull();
            model.TechnoQuestions.Should().BeEmpty();

            model.CurBusinessValue.HasValue.Should().BeFalse();
            model.CurTechnoValue.HasValue.Should().BeFalse();
        }

        [Test]
        public void Details_should_return_default_view()
        {
            var application = InitData();

            var result = Controller.Details(application.Id);

            result.Should().BeDefaultView();

            var model = result.Model<ReviewEdit>();

            model.AppName.Should().Be(application.Name);

            model.BusinessQuestions.Should().NotBeNull();
            model.BusinessQuestions.Count().Should().Be(NHSession.QueryOver<Question>().Where(q => q.Type == QuestionType.BusinessValue).RowCount());

            model.TechnoQuestions.Should().NotBeNull();
            model.TechnoQuestions.Count().Should().Be(NHSession.QueryOver<Question>().Where(q => q.Type == QuestionType.TechnologyValue).RowCount());

            model.CurBusinessValue.HasValue.Should().BeTrue();
            model.CurTechnoValue.HasValue.Should().BeTrue();
        }

        [Test]
        public void Edit_should_return_not_found()
        {
            Controller.Edit(Rand.Int(int.MaxValue)).Should().BeHttpNotFound();
        }

        [Test]
        public void Edit_should_return_default_view_when_empty()
        {
            var application = Factories.Application.Save();

            var result = Controller.Edit(application.Id);

            result.Should().BeDefaultView();

            var model = result.Model<ReviewEdit>();

            model.AppName.Should().Be(application.Name);

            model.BusinessQuestions.Should().NotBeNull();
            model.BusinessQuestions.Should().BeEmpty();

            model.TechnoQuestions.Should().NotBeNull();
            model.TechnoQuestions.Should().BeEmpty();

            model.CurBusinessValue.HasValue.Should().BeFalse();
            model.CurTechnoValue.HasValue.Should().BeFalse();
        }

        [Test]
        public void Edit_should_return_default_view()
        {
            var application = InitData();

            var result = Controller.Edit(application.Id);

            result.Should().BeDefaultView();

            var model = result.Model<ReviewEdit>();

            model.AppName.Should().Be(application.Name);

            model.BusinessQuestions.Should().NotBeNull();
            model.BusinessQuestions.Count().Should().Be(NHSession.QueryOver<Question>().Where(q => q.Type == QuestionType.BusinessValue).RowCount());

            model.TechnoQuestions.Should().NotBeNull();
            model.TechnoQuestions.Count().Should().Be(NHSession.QueryOver<Question>().Where(q => q.Type == QuestionType.TechnologyValue).RowCount());

            model.CurBusinessValue.HasValue.Should().BeTrue();
            model.CurTechnoValue.HasValue.Should().BeTrue();
        }

        [Test]
        public void Edit_should_update_data()
        {
            var application = InitData();

            var resultGet = Controller.Edit(application.Id);

            resultGet.Should().BeDefaultView();

            var model = resultGet.Model<ReviewEdit>();

            var newAnswerId = NHSession.QueryOver<QuestionAnswer>().Where(a => a.Question.Id == question1Id).List().Last().Id;

            model.BusinessQuestions.Where(q => q.Id == question1Id).Single().SelectedAnswer = newAnswerId;

            StubStandardRequest();

            var result = Controller.Edit(application.Id, model);

            result.Should().BeRedirectToActionName("Details");

            NHSession.Flush();
            NHSession.Clear();

            var question = NHSession.Get<Question>(question1Id);

            var selectedAnswer = question.GetAnswerIdByApplication(application);

            selectedAnswer.Should().Be(newAnswerId);
        }

        private Application InitData()
        {
            var type1 = Rand.Enum<QuestionType>();
            var type2 = Rand.Enum<QuestionType>();
            while (type2 == type1)
                type2 = Rand.Enum<QuestionType>();

            var application = Factories.Application.Save();

            var question1 = Factories.Question.Save(q => q.Type = QuestionType.BusinessValue);
            var question2 = Factories.Question.Save(q => q.Type = QuestionType.BusinessValue);
            var question3 = Factories.Question.Save(q => q.Type = QuestionType.TechnologyValue);
            var question4 = Factories.Question.Save(q => q.Type = QuestionType.TechnologyValue);
            var question5 = Factories.Question.Save(q => { q.Type = QuestionType.TechnologyValue; q.Calculation = new CGI.Reflex.Core.Calculation.Technologies(); });
            var question6 = Factories.Question.Save(q => { q.Type = QuestionType.TechnologyValue; q.Calculation = new ServersTechnologies(); });
            var question7 = Factories.Question.Save(q => { q.Type = QuestionType.TechnologyValue; q.Calculation = new IntegrationsTechnologies(); });

            question1.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question1.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question1.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });

            question3.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question3.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question3.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question3.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });

            question4.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });
            question4.AddAnswer(new QuestionAnswer { Name = Rand.String(), Value = Rand.Int(int.MaxValue) });

            NHSession.Flush();

            question1Id = question1.Id;

            application.AnswerQuestion(question1, question1.Answers.FirstOrDefault());
            application.AnswerQuestion(question2, null);
            application.AnswerQuestion(question3, question3.Answers.FirstOrDefault());
            application.AnswerQuestion(question4, null);

            application.AddTechnologyLink(Factories.Technology.Save());
            application.AddTechnologyLink(Factories.Technology.Save());
            application.AddTechnologyLink(Factories.Technology.Save());
            application.AddTechnologyLink(Factories.Technology.Save());
            application.AddTechnologyLink(Factories.Technology.Save());
            application.AddTechnologyLink(Factories.Technology.Save());

            var server = Factories.Server.Save();

            server.AddTechnologyLink(Factories.Technology.Save());
            server.AddTechnologyLink(Factories.Technology.Save());

            application.AddServerLink(server);

            var integration1 = Factories.Integration.Save(i => i.AppSource = application);
            var integration2 = Factories.Integration.Save(i => i.AppDest = application);

            integration1.AddTechnologyLink(Factories.Technology.Save());
            integration1.AddTechnologyLink(Factories.Technology.Save());

            NHSession.Flush();
            NHSession.Refresh(application);

            return application;
        }
    }
}
