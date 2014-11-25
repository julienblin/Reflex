// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionAnswerMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class QuestionAnswerMap : BaseEntityMap<QuestionAnswer>
    {
        public QuestionAnswerMap()
        {
            Map(x => x.Name);
            Map(x => x.Explanation);
            Map(x => x.Value);

            References(x => x.Question).Cascade.None();

            HasMany(x => x.Reviews)
                .AsSet()
                .Inverse()
                .KeyColumn("AnswerQuestionAnswerId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
