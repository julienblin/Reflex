// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncWebSessionContext.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;

namespace CGI.Reflex.Web.Infra.Controllers
{
    /// <summary>
    /// NHibernate session context for the web that also works with asynchronous controllers.
    /// </summary>
    /// <see cref="AsyncScope"/>
    public class AsyncWebSessionContext : WebSessionContext
    {
        public AsyncWebSessionContext(ISessionFactoryImplementor factory)
            : base(factory)
        {
        }

        public override ISession CurrentSession()
        {
            if (HttpContext.Current != null)
                return base.CurrentSession();

            return AsyncScope.GetCurrentAsyncNHSession();
        }
    }
}