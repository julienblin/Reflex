// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseConcurrentEntityMap.cs" company="CGI">
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
    public class BaseConcurrentEntityMap<T> : BaseEntityMap<T>
        where T : BaseConcurrentEntity
    {
        protected BaseConcurrentEntityMap(bool enableCache = true)
            : base(enableCache)
        {
            Version(x => x.ConcurrencyVersion).Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
