// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfoMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class AppInfoMap : BaseConcurrentEntityMap<AppInfo>
    {
        public AppInfoMap()
        {
            Map(x => x.Description);
            Map(x => x.MaintenanceWindow);
            Map(x => x.Notes);

            References(x => x.Status).Cascade.None();
            References(x => x.Criticity).Cascade.None();
            References(x => x.UserRange).Cascade.None();
            References(x => x.Category).Cascade.None();
            References(x => x.Certification).Cascade.None();
            References(x => x.Sector).Cascade.None();
        }
    }
}
