// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CGI.Reflex.Core.Tests.Factories
{
    public abstract class BaseFactory<T>
    {
        protected BaseFactory(AllFactories factories)
        {
            Factories = factories;
        }

        protected AllFactories Factories { get; private set; }

        public T Create()
        {
            return Create(null);
        }

        public T Create(Action<T> overrides)
        {
            var created = CreateImpl();
            if (overrides != null)
                overrides(created);
            return created;
        }

        public T Save()
        {
            return Save(null);
        }

        public T Save(Action<T> overrides)
        {
            var created = Create(overrides);
            References.NHSession.Save(created);
            return created;
        }

        protected abstract T CreateImpl();
    }
}
