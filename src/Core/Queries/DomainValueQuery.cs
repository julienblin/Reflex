// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Queries
{
    public class DomainValueQuery : BaseQueryOver<DomainValue>
    {
        public DomainValueCategory? Category { get; set; }

        public string Name { get; set; }

        protected override IQueryOver<DomainValue, DomainValue> OverImpl(ISession session)
        {
            var query = session.QueryOver<DomainValue>();

            if (Category.HasValue)
                query.Where(d => d.Category == Category.Value);

            if (!string.IsNullOrEmpty(Name))
                query.Where(d => d.Name == Name);

            return query;
        }
    }
}