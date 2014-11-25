// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasManyToManyConvention.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace CGI.Reflex.Core.Mappings.Conventions
{
    public class HasManyToManyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            if (instance.OtherSide == null)
            {
                var tableName = instance.Relationship.EntityType.Name.Pluralize() + "To" + instance.Member.Name.Pluralize();
                instance.Table(tableName);

                var keyColumnName = instance.EntityType.Name + "Id";
                instance.Key.Column(keyColumnName);
                instance.Key.ForeignKey(string.Format("FK_{0}_{1}", tableName, keyColumnName));

                var realtionShipColumnName = instance.Relationship.StringIdentifierForModel + "Id";
                instance.Relationship.Column(realtionShipColumnName);
                instance.Relationship.ForeignKey(string.Format("FK_{0}_{1}", tableName, realtionShipColumnName));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
