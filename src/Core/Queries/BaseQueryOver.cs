// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseQueryOver.cs" company="CGI">
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
    public abstract class BaseQueryOver<T>
    {
        public virtual QueryIntermediateResult<T> Fetch(Expression<Func<T, object>> path)
        {
            var queryOver = Over();
            queryOver = queryOver.Fetch(path).Eager;
            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual QueryIntermediateResult<T> Cacheable()
        {
            var queryOver = Over();
            if (References.ReferencesConfiguration.EnableQueryCache)
                queryOver.Cacheable();
            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual QueryIntermediateResult<T> JoinQueryOver(Expression<Func<T, object>> path)
        {
            var queryOver = Over();
            queryOver.JoinQueryOver(path);
            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual QueryIntermediateResult<T> LeftJoinQueryOver(Expression<Func<T, object>> path)
        {
            var queryOver = Over();
            queryOver.Left.JoinQueryOver(path);
            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual QueryIntermediateResult<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc)
        {
            var queryOver = Over();
            switch (orderType)
            {
                case OrderType.Asc:
                    queryOver.OrderBy(path).Asc();
                    break;
                case OrderType.Desc:
                    queryOver.OrderBy(path).Desc();
                    break;
            }

            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual QueryIntermediateResult<T> OrderBy(string propertyName, OrderType orderType = OrderType.Asc)
        {
            var queryOver = Over();

            if (!OrderByCallback(queryOver, propertyName, orderType))
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    switch (orderType)
                    {
                        case OrderType.Asc:
                            queryOver.OrderBy(Projections.Property(propertyName)).Asc();
                            break;
                        case OrderType.Desc:
                            queryOver.OrderBy(Projections.Property(propertyName)).Desc();
                            break;
                    }
                }
            }

            return new QueryIntermediateResult<T>(queryOver);
        }

        public virtual IQueryOver<T, T> Over()
        {
            return Over(References.NHSession);
        }

        public virtual IQueryOver<T, T> Over(ISession session)
        {
            return OverImpl(session);
        }

        public virtual T SingleOrDefault()
        {
            return SingleOrDefault(References.NHSession);
        }

        public virtual T SingleOrDefault(ISession session)
        {
            return Over(session).SingleOrDefault();
        }

        public virtual IEnumerable<T> List()
        {
            return List(References.NHSession);
        }

        public virtual IEnumerable<T> List(ISession session)
        {
            return Over(session).List();
        }

        public virtual IEnumerable<T> Future()
        {
            return Future(References.NHSession);
        }

        public virtual IEnumerable<T> Future(ISession session)
        {
            return Over(session).Future();
        }

        public virtual PaginationResult<T> Paginate(int page = 1, int pageSize = 20)
        {
            return Paginate(References.NHSession, page, pageSize);
        }

        public virtual PaginationResult<T> Paginate(ISession session, int page = 1, int pageSize = 20)
        {
            var countQueryOver = Over(session);
            countQueryOver.ClearOrders();
            var rowCount = countQueryOver.Select(Projections.RowCount()).FutureValue<int>();

            var itemsQueryOver = Over(session);
            var items = itemsQueryOver.Skip((page - 1) * pageSize).Take(pageSize).Future();

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
            return Count(References.NHSession);
        }

        public virtual int Count(ISession session)
        {
            var countQueryOver = Over(session);
            countQueryOver.ClearOrders();
            return countQueryOver.RowCount();
        }

        protected virtual bool OrderByCallback(IQueryOver<T, T> queryOver, string propertyName, OrderType orderType)
        {
            return false;
        }

        protected abstract IQueryOver<T, T> OverImpl(ISession session);
    }
}