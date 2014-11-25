// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTechnoLinkMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Mappings
{
    public class IntegrationTechnoLinkMap : BaseEntityMap<IntegrationTechnoLink>
    {
        public IntegrationTechnoLinkMap()
        {
            References(x => x.Integration).Cascade.None();
            References(x => x.Technology).Cascade.None();
        }
    }
}
