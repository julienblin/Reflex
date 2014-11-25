// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditableAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Resources;

namespace CGI.Reflex.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AuditableAttribute : Attribute
    {
        public AuditableAttribute(Type resourceType, string name)
        {
            ResourceType = resourceType;
            Name = name;
        }

        public Type ResourceType { get; private set; }

        public string Name { get; private set; }

        public string GetName()
        {
            if (ResourceType == null)
                throw new InvalidOperationException("Unable to retrieve display name because no ResourceType has been provided.");
            var res = new ResourceManager(ResourceType);
            return res.GetString(Name);
        }
    }
}