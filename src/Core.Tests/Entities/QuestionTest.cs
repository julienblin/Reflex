// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentAssertions;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class QuestionTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<Question>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Type, Rand.Enum<QuestionType>())
                .CheckProperty(x => x.Name, Rand.String(30))
                .CheckProperty(x => x.Description, Rand.LoremIpsum())
                .CheckProperty(x => x.Weight, Rand.Int(int.MaxValue))
                .VerifyTheMappings();
        }

        [Test]
        public void It_should_retrieve_answers()
        {
            var question = Factories.Question.Save();
            var answer1 = Factories.QuestionAnswer.Save(q => q.Question = question);
            var answer2 = Factories.QuestionAnswer.Save();

            NHSession.Refresh(question);
            question.Answers.Should().OnlyContain(s => s == answer1);
        }

        [Test]
        public void It_should_add_answers()
        {
            var question = Factories.Question.Save();

            question.AddAnswer(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            question.AddAnswer(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            NHSession.Flush();
            NHSession.Clear();

            Question q = NHSession.Get<Question>(question.Id);

            question.Answers.Count().Should().Be(2);

            NHSession.QueryOver<QuestionAnswer>().List().Count().Should().Be(2);
        }

        [Test]
        public void It_should_remove_answers()
        {
            var question = Factories.Question.Save();

            question.AddAnswer(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            question.AddAnswer(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            question.AddAnswer(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            NHSession.Flush();
            NHSession.Clear();

            var answerToDelete = NHSession.QueryOver<QuestionAnswer>().List().FirstOrDefault();

            var q = NHSession.Get<Question>(question.Id);

            q.RemoveAnswer(answerToDelete.Id);

            NHSession.Flush();
            NHSession.Clear();

            var finalQuestion = NHSession.Load<Question>(question.Id);

            finalQuestion.Answers.Count().Should().Be(2);
            finalQuestion.Answers.Should().NotContain(answerToDelete);

            NHSession.QueryOver<QuestionAnswer>().List().Count().Should().Be(2);
        }

        [Test]
        public void It_should_set_answers()
        {
            var question = Factories.Question.Save();

            var answerList = new List<QuestionAnswer>();
            for (int i = 0; i < 10; i++)
            {
                answerList.Add(new QuestionAnswer
                {
                    Name = Rand.String(255),
                    Explanation = Rand.String(255),
                    Value = Rand.Int(int.MaxValue),
                    Question = question
                });
            }

            question.SetQuestionAnswers(answerList);

            NHSession.Flush();
            NHSession.Clear();

            var q = NHSession.Load<Question>(question.Id);

            q.Answers.Count().Should().Be(10);

            NHSession.QueryOver<QuestionAnswer>().List().Count().Should().Be(10);

            answerList.Remove(answerList.First());

            answerList.Add(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            answerList.Add(new QuestionAnswer
            {
                Name = Rand.String(255),
                Explanation = Rand.String(255),
                Value = Rand.Int(int.MaxValue),
                Question = question
            });

            q.SetQuestionAnswers(answerList);

            NHSession.Flush();
            NHSession.Clear();

            var finalQuestion = NHSession.Load<Question>(question.Id);

            finalQuestion.Answers.Count().Should().Be(11);

            NHSession.QueryOver<QuestionAnswer>().List().Count().Should().Be(11);
        }
    }
}
