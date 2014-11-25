// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionFactory.cs" company="CGI">
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
    public class QuestionFactory : BaseFactory<Question>
    {
        public QuestionFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Question CreateImpl()
        {
            return new Question
            {
                Name = Rand.String(20),
                Description = Rand.LoremIpsum(),
                Type = Rand.Enum<QuestionType>(),
                Weight = Rand.Int(100)
            };
        }
    }
}
