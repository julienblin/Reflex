// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryIntermediateResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class QueryIntermediateResult<T>
    {
        private IQueryOver<T, T> _queryOver;

        public QueryIntermediateResult(IQueryOver<T, T> queryOver)
        {
            _queryOver = queryOver;
        }

        public QueryIntermediateResult<T> Fetch(Expression<Func<T, object>> path)
        {
            _queryOver = _queryOver.Fetch(path).Eager;
            return new QueryIntermediateResult<T>(_queryOver);
        }

        public virtual QueryIntermediateResult<T> Cacheable()
        {
            if (References.ReferencesConfiguration.EnableQueryCache)
                _queryOver.Cacheable();
            return new QueryIntermediateResult<T>(_queryOver);
        }

        public QueryIntermediateResult<T> JoinQueryOver(Expression<Func<T, object>> path)
        {
            _queryOver.JoinQueryOver(path);
            return new QueryIntermediateResult<T>(_queryOver);
        }

        public virtual QueryIntermediateResult<T> LeftJoinQueryOver(Expression<Func<T, object>> path)
        {
            _queryOver.Left.JoinQueryOver(path);
            return new QueryIntermediateResult<T>(_queryOver);
        }

        public virtual T SingleOrDefault()
        {
            return _queryOver.SingleOrDefault();
        }

        public virtual IEnumerable<T> List()
        {
            return _queryOver.List();
        }

        public virtual IEnumerable<T> Future()
        {
            return _queryOver.Future();
        }

        public virtual PaginationResult<T> Paginate(int page = 1, int pageSize = 20)
        {
            var countCriteria = CriteriaTransformer.Clone(_queryOver.UnderlyingCriteria);
            countCriteria.ClearOrders();
            var rowCount = countCriteria.SetProjection(Projections.RowCount()).FutureValue<int>();

            var itemsCriteria = CriteriaTransformer.Clone(_queryOver.UnderlyingCriteria);
            var items = itemsCriteria.SetFirstResult((page - 1) * pageSize).SetMaxResults(pageSize).Future<T>();

            return new PaginationResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = rowCount.Value,
                Items = items
            };
        }

        public virtual int Count()
        {
            _queryOver.ClearOrders();
            return _queryOver.RowCount();
        }
    }
}