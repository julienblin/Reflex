// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Mappings.UserTypes;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class QuestionMap : BaseConcurrentEntityMap<Question>
    {
        public QuestionMap()
        {
            Map(x => x.Type);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Weight);
            Map(x => x.Calculation)
                .CustomType<CalculationUserType>();

            HasMany(x => x.Answers)
                .AsSet()
                .Inverse()
                .KeyColumn("QuestionId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
