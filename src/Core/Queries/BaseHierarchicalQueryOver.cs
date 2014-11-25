// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseHierarchicalQueryOver.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries
{
    public abstract class BaseHierarchicalQueryOver<T>
        where T : BaseHierarchicalEntity<T>
    {
        protected BaseHierarchicalQueryOver(int defaultNumberOfEagerLoading = 3)
        {
            NumberOfEagerLoadingLevels = defaultNumberOfEagerLoading;
        }

        public int NumberOfEagerLoadingLevels { get; set; }

        public virtual IQueryOver<T, T> Over()
        {
            return Over(References.NHSession);
        }

        public virtual IQueryOver<T, T> Over(ISession session)
        {
            if (NumberOfEagerLoadingLevels > 5)
                throw new InvalidOperationException("It is not recommended to have a number of eager loading levels > 5. Override Over(ISession) to by pass the limit if you want.");

            var queryOver = OverImpl(session);
            
            if (NumberOfEagerLoadingLevels > 0)
                queryOver = queryOver.Fetch(x => x.Children).Eager;

            if (NumberOfEagerLoadingLevels > 1)
                queryOver = queryOver.Fetch(x => x.Children.First().Children).Eager;

            if (NumberOfEagerLoadingLevels > 2)
                queryOver = queryOver.Fetch(x => x.Children.First().Children.First().Children).Eager;

            if (NumberOfEagerLoadingLevels > 3)
                queryOver = queryOver.Fetch(x => x.Children.First().Children.First().Children.First().Children).Eager;

            if (NumberOfEagerLoadingLevels > 4)
                queryOver = queryOver.Fetch(x => x.Children.First().Children.First().Children.First().Children.First().Children).Eager;

            queryOver.TransformUsing(Transformers.DistinctRootEntity);
            return queryOver;
        }

        public virtual IQueryOver<T, T> FetchParents()
        {
            return FetchParents(References.NHSession);
        }

        public virtual IQueryOver<T, T> FetchParents(ISession session)
        {
            if (NumberOfEagerLoadingLevels > 5)
                throw new InvalidOperationException("It is not recommended to have a number of eager loading levels > 5. Override Over(ISession) to by pass the limit if you want.");

            var queryOver = OverImpl(session);

            if (NumberOfEagerLoadingLevels > 0)
                queryOver = queryOver.Fetch(x => x.Parent).Eager;

            if (NumberOfEagerLoadingLevels > 1)
                queryOver = queryOver.Fetch(x => x.Parent.Parent).Eager;

            if (NumberOfEagerLoadingLevels > 2)
                queryOver = queryOver.Fetch(x => x.Parent.Parent.Parent).Eager;

            if (NumberOfEagerLoadingLevels > 3)
                queryOver = queryOver.Fetch(x => x.Parent.Parent.Parent.Parent).Eager;

            if (NumberOfEagerLoadingLevels > 4)
                queryOver = queryOver.Fetch(x => x.Parent.Parent.Parent.Parent.Parent).Eager;

            queryOver.TransformUsing(Transformers.DistinctRootEntity);
            return queryOver;
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

        protected abstract IQueryOver<T, T> OverImpl(ISession session);
    }
}
