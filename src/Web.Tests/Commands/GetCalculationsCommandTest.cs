// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCalculationsCommandTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Commands
{
    public class GetCalculationsCommandTest
    {
        [Test]
        public void It_should_return_return_list()
        {
            var cmd = new GetCalculationsCommand();
            var result = cmd.Execute();

            result.Count().Should().BeGreaterThan(0);
        }
    }
}
