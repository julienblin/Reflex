// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfigurationEnvironmentLogProvider.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

using CGI.Reflex.Web.Configuration;

namespace CGI.Reflex.Web.Infra.Log
{
    [ExcludeFromCodeCoverage]
    public class ReflexConfigurationEnvironmentLogProvider
    {
        private Lazy<ReflexConfigurationSection> _configurationSection = new Lazy<ReflexConfigurationSection>(ReflexConfigurationSection.GetConfigurationSection);
        
        public override string ToString()
        {
            return _configurationSection.Value.Environment.ToString();
        }
    }
}