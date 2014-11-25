// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionQueryTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class QuestionQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_question()
        {
            new QuestionQuery().Count().Should().Be(0);
        }

        [Test]
        public void It_should_filter_by_type()
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

            new QuestionQuery { Type = type1 }.Count().Should().Be(3);
            new QuestionQuery { Type = type2 }.Count().Should().Be(2);
        }
    }
}
