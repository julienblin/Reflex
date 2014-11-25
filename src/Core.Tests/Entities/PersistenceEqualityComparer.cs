// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceEqualityComparer.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Entities
{
    public class PersistenceEqualityComparer : IEqualityComparer
    {
        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x is BaseEntity && y is BaseEntity)
            {
                return ((BaseEntity)x).Id == ((BaseEntity)y).Id;
            }

            if (x is DateTime && y is DateTime)
            {
                var xDt = (DateTime)x;
                var yDt = (DateTime)y;
                return xDt.Year == yDt.Year
                    && xDt.Month == yDt.Month
                    && xDt.Day == yDt.Day
                    && xDt.Hour == yDt.Hour
                    && xDt.Minute == yDt.Minute
                    && xDt.Second == yDt.Second;
            }

            return x.Equals(y);
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Equality comparer only used in test code. Exception here to be thrown if improper usage.")]
        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
