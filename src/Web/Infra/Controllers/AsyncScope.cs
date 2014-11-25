// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncScope.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Web.Infra.Controllers
{
    /// <summary>
    /// Allow providing a current user through global callback for asynchronous operations (using Thread context).
    /// + References.NHSession working for the duration of the scope.
    /// </summary>
    /// <see cref="MvcApplication.CurrentUserCallback()"/>
    /// <see cref="References.NHSession"/>
    public class AsyncScope : IDisposable
    {
        private const string CurrentUserDataSlot = @"CurrentUserDataSlot";

        private const string CurrentNHSessionDataSlot = @"CurrentNHSessionDataSlot";

        public AsyncScope(ISession session, User asyncUser)
        {
            Thread.SetData(Thread.GetNamedDataSlot(CurrentNHSessionDataSlot), session);
            Thread.SetData(Thread.GetNamedDataSlot(CurrentUserDataSlot), asyncUser);
        }

        ~AsyncScope()
        {
            Dispose(false);
        }

        public static User GetCurrentAsyncUser()
        {
            return (User)Thread.GetData(Thread.GetNamedDataSlot(CurrentUserDataSlot));
        }

        public static ISession GetCurrentAsyncNHSession()
        {
            return (ISession)Thread.GetData(Thread.GetNamedDataSlot(CurrentNHSessionDataSlot));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Thread.FreeNamedDataSlot(CurrentUserDataSlot);
                Thread.FreeNamedDataSlot(CurrentNHSessionDataSlot);
            }
        }
    }
}