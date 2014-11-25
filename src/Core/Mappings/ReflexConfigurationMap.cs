// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfigurationMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Mappings.UserTypes;
using FluentNHibernate.Mapping;

namespace CGI.Reflex.Core.Mappings
{
    public class ReflexConfigurationMap : ClassMap<ReflexConfiguration>
    {
        public ReflexConfigurationMap()
        {
            Id(x => x.Id)
                .GeneratedBy.Assigned()
                .Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.ActiveAppStatusDVIds).CustomType<DelimitedListUserType<int>>();
        }
    }
}
