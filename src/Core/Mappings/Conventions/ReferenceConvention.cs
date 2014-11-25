// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceConvention.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace CGI.Reflex.Core.Mappings.Conventions
{
    public class ReferenceConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            var columnName = instance.Property.Name + instance.Property.PropertyType.Name + "Id";
            if (instance.Property.Name == instance.Property.PropertyType.Name)
                columnName = instance.Property.Name + "Id";

            instance.Column(columnName);
            instance.ForeignKey(string.Format("FK_{0}_{1}", instance.EntityType.Name.Pluralize(), columnName));

            if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), true))
                instance.Not.Nullable();
        }
    }
}