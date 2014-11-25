// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseHierarchicalEntityMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.Collections;

namespace CGI.Reflex.Core.Mappings
{
    public abstract class BaseHierarchicalEntityMap<T> : BaseEntityMap<T>
        where T : BaseHierarchicalEntity<T>
    {
        protected BaseHierarchicalEntityMap()
        {
            Map(x => x.Name);

            References(x => x.Parent);
            HasMany(x => x.Children)
                .AsSet()
                .Inverse()
                .KeyColumn("Parent" + typeof(T).Name + "Id")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan();
        }
    }
}
