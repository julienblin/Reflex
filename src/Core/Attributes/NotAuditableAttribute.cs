// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotAuditableAttribute.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CGI.Reflex.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotAuditableAttribute : Attribute
    {
    }
}