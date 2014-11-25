// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableNameConvention.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace CGI.Reflex.Core.Mappings.Conventions
{
    public class TableNameConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
        }

        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.Pluralize());
        }
    }
}