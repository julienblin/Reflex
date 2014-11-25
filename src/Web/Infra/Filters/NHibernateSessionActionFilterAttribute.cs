// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NHibernateSessionActionFilterAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGI.Reflex.Core;
using NHibernate.Context;

namespace CGI.Reflex.Web.Infra.Filters
{
    [ExcludeFromCodeCoverage]
    public class NHibernateSessionActionFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CurrentSessionContext.HasBind(References.SessionFactory))
            {
                var session = References.SessionFactory.OpenSession();
                session.BeginTransaction();
                CurrentSessionContext.Bind(session);
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var session = CurrentSessionContext.Unbind(References.SessionFactory);

            if (session != null)
            {
                try
                {
                    if (filterContext.Exception == null)
                    {
                        if (session.Transaction != null && session.Transaction.IsActive)
                            session.Transaction.Commit();
                    }
                    else
                    {
                        if (session.Transaction != null && session.Transaction.IsActive)
                            session.Transaction.Rollback();
                    }
                }
                finally
                {
                    session.Dispose();
                }
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            var session = CurrentSessionContext.Unbind(References.SessionFactory);

            if (session != null)
            {
                try
                {
                    if (session.Transaction != null && session.Transaction.IsActive)
                        session.Transaction.Rollback();
                }
                finally
                {
                    session.Dispose();
                }
            }
        }
    }
}