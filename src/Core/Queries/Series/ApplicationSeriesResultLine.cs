// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeriesResultLine.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace CGI.Reflex.Core.Queries.Series
{
    public class ApplicationSeriesResultLine
    {
        private readonly IList<int> _counts = new List<int>();

        public object LineCriteria { get; set; }

        public int Total
        {
            get { return _counts.Sum(); }
        }

        public IList<int> Counts
        {
            get { return _counts; }
        }

        internal int CumulativeCount
        {
            get { throw new NotImplementedException(); }
            set { _counts.Add(value); }
        }

        public T GetCriteria<T>()
        {
            return (T)LineCriteria;
        }

        public int GetCount(int columnIndex)
        {
            return _counts[columnIndex];
        }
    }
}