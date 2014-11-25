// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServerLinkMap.cs" company="CGI">
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
    public class AppServerLinkMap : BaseEntityMap<AppServerLink>
    {
        public AppServerLinkMap()
        {
            References(x => x.Application).Cascade.None();
            References(x => x.Server).Cascade.None();
        }
    }
}
