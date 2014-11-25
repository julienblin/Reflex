// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsAllowedAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CGI.Reflex.Core;
using NHibernate.Context;

namespace CGI.Reflex.Web.Infra.Filters
{
    public enum IsAllowedOperator
    {
        And,
        Or
    }

    [ExcludeFromCodeCoverage]
    public class IsAllowedAttribute : ActionFilterAttribute
    {
        private readonly string _operation;
        private readonly string[] _operations;

        public IsAllowedAttribute(string operation, params string[] operations)
        {
            _operation = operation;
            _operations = operations;
        }

        public IEnumerable<string> Operations
        {
            get
            {
                yield return _operation;
                if (_operations != null && _operations.Length != 0)
                {
                    foreach (var op in _operations)
                        yield return op;
                }
            }
        }

        public IsAllowedOperator Operator { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (References.CurrentUser == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            var authorized = Operator == IsAllowedOperator.And;
            if (References.CurrentUser != null)
            {
                foreach (var op in Operations)
                {
                    switch (Operator)
                    {
                        case IsAllowedOperator.And:
                            authorized = authorized && References.CurrentUser.IsAllowed(op);
                            break;
                        case IsAllowedOperator.Or:
                            authorized = authorized || References.CurrentUser.IsAllowed(op);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (!authorized)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}