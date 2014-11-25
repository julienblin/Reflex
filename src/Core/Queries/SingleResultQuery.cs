// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleResultQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace CGI.Reflex.Core.Queries
{
    public abstract class SingleResultQuery<T>
    {
        public virtual T Execute()
        {
            return Execute(References.NHSession);
        }

        public abstract T Execute(ISession session);
    }
}
