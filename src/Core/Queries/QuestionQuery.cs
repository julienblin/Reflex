// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Queries
{
    public class QuestionQuery : BaseQueryOver<Question>
    {
        public QuestionType? Type { get; set; }

        protected override IQueryOver<Question, Question> OverImpl(ISession session)
        {
            var query = session.QueryOver<Question>();

            if (Type.HasValue)
                query.Where(q => q.Type == Type.Value);

            return query;
        }
    }
}
