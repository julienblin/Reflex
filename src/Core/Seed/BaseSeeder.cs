// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public abstract class BaseSeeder : ISeeder
    {
        public abstract int Priority { get; }

        public virtual bool IsMinimal
        {
            get
            {
                return false;
            }
        }

        protected ISession Session { get; private set; }

        public virtual void Seed(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            Session = session;
            SeedImpl();
        }

        public DomainValue Get(DomainValueCategory category, string name)
        {
            return new DomainValueQuery { Category = category, Name = name }.SingleOrDefault(Session);
        }

        public IEnumerable<Technology> GetTechnos(params string[] fullnames)
        {
            return Session.QueryOver<Technology>().List().Where(t => fullnames.Contains(t.FullName, StringComparer.InvariantCultureIgnoreCase));
        }

        protected abstract void SeedImpl();
    }
}
