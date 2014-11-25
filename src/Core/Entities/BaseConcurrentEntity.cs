// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseConcurrentEntity.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public abstract class BaseConcurrentEntity : BaseEntity, IOptimisticConcurrency
    {
#pragma warning disable 649
        private int _concurrencyVersion;
#pragma warning restore 649

        public virtual int ConcurrencyVersion
        {
            get { return _concurrencyVersion; }
        }
    }
}
