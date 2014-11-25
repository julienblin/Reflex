// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryOverExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Queries
{
    public static class QueryOverExtensions
    {
        public static IQueryOver<T1, T2> OrderBy<T1, T2>(this IQueryOver<T1, T2> queryOver, Expression<Func<T2, object>> expression, OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Asc:
                    return queryOver.OrderBy(expression).Asc;
                case OrderType.Desc:
                    return queryOver.OrderBy(expression).Desc;
                default:
                    throw new ArgumentOutOfRangeException("orderType");
            }
        }

        public static IQueryOver<T1, T2> OrderByDomainValue<T1, T2>(this IQueryOver<T1, T2> queryOver, OrderType orderType)
            where T2 : DomainValue
        {
            return queryOver.OrderBy(r => r.Name, orderType);
        }
    }
}
