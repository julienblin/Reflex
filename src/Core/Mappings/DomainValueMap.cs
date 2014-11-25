// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueMap.cs" company="CGI">
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
    public class DomainValueMap : BaseConcurrentEntityMap<DomainValue>
    {
        public DomainValueMap()
        {
            Map(x => x.Category);
            Map(x => x.DisplayOrder);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Color)
                .Length(7)
                .CustomType<ColorUserType>();
        }
    }
}
