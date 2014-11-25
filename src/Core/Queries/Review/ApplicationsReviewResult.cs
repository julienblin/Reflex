// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsReviewResult.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Queries.Review
{
    public class ApplicationsReviewResult
    {
        public ApplicationsReviewResult()
        {
            Resulsts = new List<ApplicationsReviewResultLine>();
        }

        public IEnumerable<ApplicationsReviewResultLine> Resulsts { get; set; }
    }
}
