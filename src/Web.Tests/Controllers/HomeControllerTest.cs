// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core.Calculation;
using CGI.Reflex.Core.Queries.Review;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Models.Home;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class HomeControllerTest : BaseControllerTest<HomeController>
    {
        [Test]
        public void Index_should_return_default_view()
        {
            var result = Controller.Index(new HomeParams());

            result.Should().BeDefaultView();
        }

        [Test]
        public void ApplicationsChart_should_return_chart()
        {
            Factories.Application.Save();
            Factories.Application.Save();
            Factories.Application.Save();
            Factories.Application.Save();

            var result = Controller.ApplicationsChart(300, 300, "ApplicationType");

            result.Should().BeChartResult();
        }

        [Test]
        public void ApplicationsReviewChart_should_return_chart()
        {
            var app1 = Factories.Application.Save();
            var app2 = Factories.Application.Save();

            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();
            var techno3 = Factories.Technology.Save();

            app1.AddTechnologyLink(techno1);
            app1.AddTechnologyLink(techno2);
            app2.AddTechnologyLink(techno2);
            app2.AddTechnologyLink(techno3);

            var question1 = Factories.Question.Save(q => q.Type = Core.Entities.QuestionType.BusinessValue);
            question1.AddAnswer(new Core.Entities.QuestionAnswer { Value = Rand.Int(100), Name = Rand.String() });
            question1.AddAnswer(new Core.Entities.QuestionAnswer { Value = Rand.Int(100), Name = Rand.String() });
            question1.AddAnswer(new Core.Entities.QuestionAnswer { Value = Rand.Int(100), Name = Rand.String() });

            var question2 = Factories.Question.Save(q => { q.Type = Core.Entities.QuestionType.TechnologyValue; q.Calculation = new Technologies(); });

            NHSession.Flush();

            app1.AnswerQuestion(question1, question1.Answers.FirstOrDefault());

            var result = Controller.ApplicationsReviewChart(300, 300, null);

            result.Should().BeChartResult();
        }

        [Test]
        public void CriticalTechno_should_return_partial()
        {
            var app1 = Factories.Application.Save();
            var app2 = Factories.Application.Save();
            var app3 = Factories.Application.Save();

            var techno1 = Factories.Technology.Save();
            var techno2 = Factories.Technology.Save();
            var techno3 = Factories.Technology.Save();
            var techno4 = Factories.Technology.Save();
            var techno5 = Factories.Technology.Save();
            var techno6 = Factories.Technology.Save();
            var techno7 = Factories.Technology.Save();

            app1.AddTechnologyLink(techno1);
            app1.AddTechnologyLink(techno2);
            app1.AddTechnologyLink(techno3);
            app1.AddTechnologyLink(techno4);
            app1.AddTechnologyLink(techno5);

            app2.AddTechnologyLink(techno4);
            app2.AddTechnologyLink(techno5);
            app2.AddTechnologyLink(techno6);
            app2.AddTechnologyLink(techno7);

            var result = Controller.CriticalTechno();

            result.Should().BePartialView("_CriticalTechno");
        }

        [Test]
        public void Assessment_should_return_view()
        {
            var result = Controller.Assessment();

            result.Should().BePartialView("_Assessment");
            result.Model<ApplicationsReviewResult>().Should().NotBeNull();
        }

        [Test]
        public void SummaryDetails_should_return_view()
        {
            StubStandardRequest();
            var result = Controller.SummaryDetails(new SummaryDetails());

            result.Should().BeDefaultView();
            result.Model<SummaryDetails>().LineCriteriaDisplayName.Should().NotBeBlank();
            result.Model<SummaryDetails>().ColumnCriteriaDisplayName.Should().BeBlank();
        }

        [Test]
        public void SummaryDetails_should_return_ajax_view()
        {
            StubAjaxRequest();
            var result = Controller.SummaryDetails(new SummaryDetails());

            result.Should().BePartialView("_Results");
        }
    }
}
