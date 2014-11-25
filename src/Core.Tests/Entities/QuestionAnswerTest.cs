// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionAnswerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class QuestionAnswerTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<QuestionAnswer>(NHSession, new PersistenceEqualityComparer())
                .CheckProperty(x => x.Name, Rand.String(25))
                .CheckProperty(x => x.Explanation, Rand.String(100))
                .CheckProperty(x => x.Value, Rand.Int(int.MaxValue))
                .CheckReference(x => x.Question, Factories.Question.Save())
                .VerifyTheMappings();
        }
    }
}
