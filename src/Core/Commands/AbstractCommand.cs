// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractCommand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Commands
{
    public abstract class AbstractCommand<TResult>
    {
        protected ISession NHSession { get { return References.NHSession; } }

        protected User CurrentUser { get { return References.CurrentUser; } }

        public virtual TResult Execute()
        {
            return ExecuteImpl();
        }

        protected abstract TResult ExecuteImpl();
    }
}