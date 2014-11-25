// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sector.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;

namespace CGI.Reflex.Core.Entities
{
    [Auditable(typeof(CoreResources), "Sector")]
    public class Sector : BaseHierarchicalEntity<Sector>
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
