// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsPropertyConvention.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Linq;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Mappings.UserTypes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace CGI.Reflex.Core.Mappings.Conventions
{
    public class DataAnnotationsPropertyConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance == null) return;

            if (instance.Property.PropertyType.IsEnum)
                instance.Length(40);

            if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), true))
                instance.Not.Nullable();

            if (instance.Property.MemberInfo.IsDefined(typeof(UniqueAttribute), true) && instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), true))
                instance.UniqueKey(string.Format("UNQ_{0}_{1}", instance.EntityType.Name.Pluralize(), instance.Property.MemberInfo.Name));

            if (instance.Property.MemberInfo.IsDefined(typeof(IndexedAttribute), true))
                instance.Index(string.Format("IDX_{0}_{1}", instance.EntityType.Name.Pluralize(), instance.Property.MemberInfo.Name));

            if (instance.Property.MemberInfo.IsDefined(typeof(StringLengthAttribute), true))
            {
                var stringLenghtAttr =
                    instance.Property.MemberInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).Cast<StringLengthAttribute>().First();
                instance.Length(stringLenghtAttr.MaximumLength);
            }

            if (instance.Property.MemberInfo.IsDefined(typeof(DataTypeAttribute), true))
            {
                var dataTypeAttr =
                   instance.Property.MemberInfo.GetCustomAttributes(typeof(DataTypeAttribute), true).Cast<DataTypeAttribute>().First();

                if (dataTypeAttr.DataType == DataType.MultilineText)
                {
                    if (References.ReferencesConfiguration.DatabaseType == DatabaseType.SqlServer2008)
                    {
                        instance.CustomSqlType("nvarchar(max)");
                        instance.CustomType("StringClob");
                    }
                }
            }
        }
    }
}