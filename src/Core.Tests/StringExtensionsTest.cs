// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests
{
    public class StringExtensionsTest
    {
        [Test]
        public void It_should_truncate_strings()
        {
            "aaa".TruncateWords(50).Should().Be("aaa");
            Rand.LoremIpsum(2000).TruncateWords(50).Length.Should().BeLessOrEqualTo(50);
            Rand.LoremIpsum(2000).TruncateWords(50).EndsWith("...").Should().BeTrue();
        }
    }
}
