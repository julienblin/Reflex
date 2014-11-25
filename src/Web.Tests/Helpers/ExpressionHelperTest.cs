// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelperTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CGI.Reflex.Web.Helpers;

using FluentAssertions;

using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Helpers
{
    [TestFixture]
    public class ExpressionHelperTest
    {
        [Test]
        public void It_should_return_property_name()
        {
            ExpressionHelper.GetPropertyName<Test>(x => x.Foo).Should().Be("Foo");
            ExpressionHelper.GetPropertyName<Test>(x => x.Bar).Should().Be("Bar");
            ExpressionHelper.GetPropertyName<Test, string>(x => x.Foo).Should().Be("Foo");
            ExpressionHelper.GetPropertyName<Test, int>(x => x.Bar).Should().Be("Bar");
        }

        private class Test
        {
            public string Foo { get; set; }

            public int Bar { get; set; }
        }
    }
}
