// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationReviewTest.cs" company="CGI">
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
    public class ApplicationReviewTest : BaseDbTest
    {
        [Test]
        public void It_should_persist()
        {
            new PersistenceSpecification<ApplicationReview>(NHSession, new PersistenceEqualityComparer())
                .CheckReference(x => x.Application, Factories.Application.Save())
                .CheckReference(x => x.Answer, Factories.QuestionAnswer.Save())
                .VerifyTheMappings();
        }
    }
}
