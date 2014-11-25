// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DomainValueAttribute : Attribute
    {
        private readonly DomainValueCategory _category;

        public DomainValueAttribute(DomainValueCategory category)
        {
            _category = category;
        }

        public DomainValueCategory Category
        {
            get { return _category; }
        }
    }
}