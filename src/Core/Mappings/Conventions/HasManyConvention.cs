// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasManyConvention.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace CGI.Reflex.Core.Mappings.Conventions
{
    public class HasManyConvention : IHasManyConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            var columnName = instance.EntityType.Name + "Id";
            instance.Key.Column(columnName);
            instance.Key.ForeignKey(string.Format("FK_{0}_{1}", instance.Relationship.StringIdentifierForModel.Pluralize(), columnName));
        }
    }
}
