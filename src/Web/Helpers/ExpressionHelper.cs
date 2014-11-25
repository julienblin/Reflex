// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelper.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Helpers
{
    public static class ExpressionHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression)
                return ((MemberExpression)expression.Body).Member.Name;

            if (expression.Body is UnaryExpression)
                return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name;
            
            throw new NotSupportedException();
        }

        public static string GetPropertyName<T, TValue>(Expression<Func<T, TValue>> expression)
        {
            if (expression.Body is MemberExpression)
                return ((MemberExpression)expression.Body).Member.Name;

            if (expression.Body is UnaryExpression)
                return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member.Name;

            throw new NotSupportedException();
        }
    }
}