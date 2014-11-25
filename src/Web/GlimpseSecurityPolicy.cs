// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlimpseSecurityPolicy.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using CGI.Reflex.Web.Configuration;

using Glimpse.AspNet.Extensions;
using Glimpse.Core.Extensibility;

namespace CGI.Reflex.Web
{
    /// <summary>
    /// Custom policy for Glimpse.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GlimpseSecurityPolicy : IRuntimePolicy
    {
        private ReflexConfigurationSection _configSection;

        public GlimpseSecurityPolicy()
        {
            _configSection = ReflexConfigurationSection.GetConfigurationSection();
        }

        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; }
        }

        public RuntimePolicy Execute(IRuntimePolicyContext policyContext)
        {
            if (_configSection.Environment != ConfigEnvironment.Production)
                return RuntimePolicy.On;

            return RuntimePolicy.Off;
        }
    }
}