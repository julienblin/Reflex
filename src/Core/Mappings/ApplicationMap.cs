// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class ApplicationMap : BaseConcurrentEntityMap<Application>
    {
        public ApplicationMap()
        {
            Map(x => x.Name);
            Map(x => x.Code);

            References(x => x.ApplicationType).Cascade.None();
            References(x => x.AppInfo)
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.All();

            HasMany(x => x.ContactLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ApplicationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.TechnologyLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ApplicationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.ServerLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ApplicationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.DbInstanceLinks)
                .AsSet()
                .Inverse()
                .KeyColumn("ApplicationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.IntegrationsAsSource)
                .AsSet()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore)
                .KeyColumns.Add("AppSourceApplicationId")
                .ForeignKeyConstraintName("FK_Integrations_AppSourceApplicationId")
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.IntegrationsAsDest)
                .AsSet()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore)
                .KeyColumns.Add("AppDestApplicationId")
                .ForeignKeyConstraintName("FK_Integrations_AppDestApplicationId")
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.Events)
               .AsSet()
               .Inverse()
               .Access.CamelCaseField(Prefix.Underscore)
               .KeyColumns.Add("ApplicationId")
               .Cascade.AllDeleteOrphan();

            HasMany(x => x.ReviewAnswers)
                .AsSet()
                .Inverse()
                .KeyColumn("ApplicationId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
