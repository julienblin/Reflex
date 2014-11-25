// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptimisticConcurrency.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CGI.Reflex.Core.Entities
{
    public interface IOptimisticConcurrency
    {
        int ConcurrencyVersion { get; }
    }
}
