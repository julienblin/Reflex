// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimestamped.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public interface ITimestamped
    {
        DateTime LastUpdatedAtUTC { get; }

        void SetLastUpdatedAtUTC(DateTime value);
    }
}
