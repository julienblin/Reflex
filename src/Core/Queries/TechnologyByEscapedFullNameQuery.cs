// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyByEscapedFullNameQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class TechnologyByEscapedFullNameQuery : SingleResultQuery<Technology>
    {
        public TechnologyByEscapedFullNameQuery()
        {
            EscapeToken = @"_";
        }

        public string EscapedFullName { get; set; }

        public string EscapeToken { get; set; }

        public override Technology Execute(ISession session)
        {
            if (string.IsNullOrEmpty(EscapedFullName)) throw new NotSupportedException();

            var names = EscapedFullName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList();

            var query = session.QueryOver<Technology>()
                               .Where(t => t.Name == names[0].Replace(EscapeToken, " "));

            IQueryOver<Technology, Technology> previousJoin = query;
            for (int i = 1; i < names.Count; i++)
            {
                previousJoin = previousJoin.JoinQueryOver(t => t.Parent).Where(t => t.Name == names[i].Replace(EscapeToken, " "));
            }

            return query.SingleOrDefault();
        }
    }
}
