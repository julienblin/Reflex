// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionAnswerFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class QuestionAnswerFactory : BaseFactory<QuestionAnswer>
    {
        public QuestionAnswerFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override QuestionAnswer CreateImpl()
        {
            return new QuestionAnswer
            {
                Name = Rand.String(25),
                Explanation = Rand.String(255),
                Question = Factories.Question.Save(),
                Value = Rand.Int(10)
            };
        }
    }
}
