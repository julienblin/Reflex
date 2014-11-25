// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHierarchicalEntity.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Entities
{
    public interface IHierarchicalEntity
    {
        int Id { get; }

        IEnumerable<int> AllParentIds { get; }

        IEnumerable<int> AllIds { get; }

        string Name { get; }

        IHierarchicalEntity Parent { get; }

        int HierarchicalLevel { get; }
    }
}
