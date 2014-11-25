// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechnologyByEscapedFullNameQueryTest.cs" company="CGI">
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
    public class TechnologyByEscapedFullNameQueryTest : BaseDbTest
    {
        [Test]
        public void It_should_work_with_no_result()
        {
            new TechnologyByEscapedFullNameQuery { EscapedFullName = Rand.String() }.Execute().Should().BeNull();
        }

        [Test]
        public void It_should_execute()
        {
            var rootTechno = Factories.Technology.Save();

            var parentTechno1 = Factories.Technology.Save(t => { t.Parent = rootTechno; t.Name = Rand.String() + " " + Rand.String(); });
            var parentTechno2 = Factories.Technology.Save(t => { t.Parent = rootTechno; t.Name = Rand.String() + " " + Rand.String(); });

            var appTechno1 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno2 = Factories.Technology.Save(t => t.Parent = parentTechno1);
            var appTechno3 = Factories.Technology.Save(t => t.Parent = parentTechno2);

            new TechnologyByEscapedFullNameQuery { EscapedFullName = rootTechno.GetEscapedFullName() }.Execute().Should().Be(rootTechno);
            new TechnologyByEscapedFullNameQuery { EscapedFullName = appTechno1.GetEscapedFullName() }.Execute().Should().Be(appTechno1);
            new TechnologyByEscapedFullNameQuery { EscapedFullName = parentTechno1.GetEscapedFullName() }.Execute().Should().Be(parentTechno1);

            new TechnologyByEscapedFullNameQuery { EscapedFullName = appTechno1.GetEscapedFullName() + " " + Rand.String() }.Execute().Should().BeNull();
        }
    }
}
