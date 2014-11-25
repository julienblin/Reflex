// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryControllerTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Tests;
using CGI.Reflex.Web.Controllers;
using CGI.Reflex.Web.Models.History;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Controllers
{
    public class HistoryControllerTest : BaseControllerTest<HistoryController>
    {
        [Test]
        public void View_should_return_not_found_if_entity_does_not_exist()
        {
            Controller.View(Rand.String(10), 1, string.Empty, string.Empty, new AuditView()).Should().BeHttpNotFound();
        }
    }
}
