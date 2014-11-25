// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseQueryOverTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Queries
{
    public class BaseQueryOverTest : BaseDbTest
    {
        [Test]
        [TestCase(250, 10)]
        [TestCase(51, 25)]
        public void It_should_paginate(int numberOfElements, int pageSize)
        {
            for (var i = 0; i < numberOfElements; i++)
                Factories.DomainValue.Save();

            var expectedNumberofPages = (numberOfElements / pageSize) + (numberOfElements % pageSize == 0 ? 0 : 1);
            for (var i = 1; i <= expectedNumberofPages; i++)
            {
                var paginationResult = new DomainValueQuery().Paginate(i, pageSize);
                paginationResult.CurrentPage.Should().Be(i);
                paginationResult.HasNextPage.Should().Be(i != expectedNumberofPages);
                paginationResult.HasPreviousPage.Should().Be(i != 1);
                paginationResult.IsFirstPage.Should().Be(i == 1);
                paginationResult.IsLastPage.Should().Be(i == expectedNumberofPages);
                paginationResult.Items.Count().Should().Be(i == expectedNumberofPages ? numberOfElements - (pageSize * (i - 1)) : pageSize);
                paginationResult.PageSize.Should().Be(pageSize);
                paginationResult.PageCount.Should().Be(expectedNumberofPages);
                paginationResult.TotalItems.Should().Be(numberOfElements);
            }
        }

        [Test]
        public void It_should_List()
        {
            for (var i = 0; i < 50; i++)
                Factories.DomainValue.Save();

            new DomainValueQuery().List().Count().Should().Be(50);
        }

        [Test]
        public void It_should_Count()
        {
            for (var i = 0; i < 50; i++)
                Factories.DomainValue.Save();

            new DomainValueQuery().Count().Should().Be(50);
        }

        [Test]
        public void It_should_SingleOrDefault()
        {
            var dv = Factories.DomainValue.Save();

            new DomainValueQuery().SingleOrDefault().Should().Be(dv);
        }
    }
}
