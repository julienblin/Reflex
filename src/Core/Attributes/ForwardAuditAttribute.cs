// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwardAuditAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CGI.Reflex.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ForwardAuditAttribute : Attribute
    {
        public ForwardAuditAttribute(string targetProperty, string targetPropertyName)
        {
            TargetProperty = targetProperty;
            TargetPropertyName = targetPropertyName;
        }

        public string TargetProperty { get; private set; }

        public string TargetPropertyName { get; private set; }
    }
}
