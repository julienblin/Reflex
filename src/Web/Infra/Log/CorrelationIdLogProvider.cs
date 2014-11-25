// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationIdLogProvider.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web;
using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Infra.Log
{
    /// <summary>
    /// Provides correlation ids per request for log4net ThreadContext
    /// </summary>
    /// <remarks>
    /// <see href="http://piers7.blogspot.com/2005/12/log4net-context-problems-with-aspnet.html" />
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class CorrelationIdLogProvider
    {
        public const string RequestCorrelationIdContextKey = @"CorrelationId";

        public override string ToString()
        {
            if (HttpContext.Current == null)
                return string.Empty;
            return (string)HttpContext.Current.Items[RequestCorrelationIdContextKey];
        }
    }
}