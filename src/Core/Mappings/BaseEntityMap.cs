// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseEntityMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;
using Prefix = FluentNHibernate.Mapping.Prefix;

namespace CGI.Reflex.Core.Mappings
{
    public abstract class BaseEntityMap<T> : ClassMap<T>
        where T : BaseEntity
    {
        protected BaseEntityMap(bool enableCache = true)
        {
            Id(x => x.Id).Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.LastUpdatedAtUTC).Access.CamelCaseField(Prefix.Underscore);

            if (enableCache && (Core.References.ReferencesConfiguration.EnableQueryCache || Core.References.ReferencesConfiguration.EnableSecondLevelCache))
                Cache.ReadWrite();
        }
    }
}
