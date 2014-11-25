// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditableCollectionReferenceAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AuditableCollectionReferenceAttribute : Attribute
    {
        private readonly string _propertyName;
        private readonly bool _logToCurrentEntity;

        public AuditableCollectionReferenceAttribute(string propertyName, bool logToCurrentEntity = false)
        {
            _propertyName = propertyName;
            _logToCurrentEntity = logToCurrentEntity;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public bool LogToCurrentEntity
        {
            get { return _logToCurrentEntity; }
        }
    }
}
