// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionResultExtensions.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FluentAssertions;

namespace CGI.Reflex.Web.Tests
{
    public static class ActionResultExtensions
    {
        public static T Model<T>(this ActionResult actionResult)
            where T : class
        {
            var viewResult = actionResult as ViewResultBase;
            viewResult.Should().NotBeNull();

            return viewResult.Model as T;
        }
    }
}
