// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserLogProvider.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CGI.Reflex.Core;

namespace CGI.Reflex.Web.Infra.Log
{
    [ExcludeFromCodeCoverage]
    public class UserLogProvider
    {
        public override string ToString()
        {
            return References.CurrentUser == null ? "Anonymous" : References.CurrentUser.UserName;
        }
    }
}