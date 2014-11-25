// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllSeedersTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Seed
{
    public class AllSeedersTest : BaseDbTest
    {
        [Test]
        public void Seeders_should_run_successfully()
        {
            References.DatabaseOperations.Seed(NHSession);
        }
    }
}
