// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

namespace CGI.Reflex.Core.Tests
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void It_should_FindLeastCommonMultiple()
        {
            new[] { 0 }.Invoking(x => x.FindLeastCommonMultiple()).ShouldThrow<InvalidOperationException>();
            new[] { 0, 1 }.FindLeastCommonMultiple().Should().Be(1);
            new[] { 4, 6 }.FindLeastCommonMultiple().Should().Be(12);
            new[] { 25, 30 }.FindLeastCommonMultiple().Should().Be(150);
            new[] { 1, 2, 3, 4, 5 }.FindLeastCommonMultiple().Should().Be(60);
        }

        [Test]
        public void It_should_paginate()
        {
            var result = new int[0].Paginate();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalItems.Should().Be(0);
            result.Items.Should().HaveCount(0);

            result = new int[5].Paginate();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalItems.Should().Be(5);
            result.Items.Should().HaveCount(5);

            result = new int[100].Paginate();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.TotalItems.Should().Be(100);
            result.Items.Should().HaveCount(20);

            result = new int[198].Paginate(2, 100);
            result.CurrentPage.Should().Be(2);
            result.PageSize.Should().Be(100);
            result.TotalItems.Should().Be(198);
            result.Items.Should().HaveCount(98);
        }
    }
}
