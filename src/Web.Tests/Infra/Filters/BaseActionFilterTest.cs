// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseActionFilterTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CGI.Reflex.Core.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace CGI.Reflex.Web.Tests.Infra.Filters
{
    [TestFixture]
    public abstract class BaseActionFilterTest<T> : BaseDbTest
        where T : ActionFilterAttribute, new()
    {
        protected BaseActionFilterTest()
        {
            Filter = new T();
            Mocks = new MockRepository();
        }

        protected T Filter { get; set; }

        protected MockRepository Mocks { get; set; }
    }
}
