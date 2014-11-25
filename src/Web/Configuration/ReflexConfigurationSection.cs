// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfigurationSection.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;
using CGI.Reflex.Core;
using log4net;
using log4net.Core;

namespace CGI.Reflex.Web.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ReflexConfigurationSection : ConfigurationSection
    {
        public const string ConfigurationSectionName = @"reflex";

        private static readonly ILog Log = LogManager.GetLogger(typeof(ReflexConfigurationSection));

        private AuthenticationMode? _authenticationMode;

        [ConfigurationProperty("appName", DefaultValue = "CGI Reflex")]
        public string AppName
        {
            get { return (string)base["appName"]; }
            set { base["appName"] = value; }
        }

        [ConfigurationProperty("encryptionKey", IsRequired = true)]
        public string EncryptionKey
        {
            get { return (string)base["encryptionKey"]; }
            set { base["encryptionKey"] = value; }
        }

        [ConfigurationProperty("environment", DefaultValue = ConfigEnvironment.Production)]
        public ConfigEnvironment Environment
        {
            get { return (ConfigEnvironment)base["environment"]; }
            set { base["environment"] = value; }
        }

        [ConfigurationProperty("recreateDatabase", DefaultValue = false)]
        public bool RecreateDatabase
        {
            get { return (bool)base["recreateDatabase"]; }
            set { base["recreateDatabase"] = value; }
        }

        [ConfigurationProperty("autoCreateUsers", DefaultValue = false)]
        public bool AutoCreateUsers
        {
            get { return (bool)base["autoCreateUsers"]; }
            set { base["autoCreateUsers"] = value; }
        }

        [ConfigurationProperty("enableASPNetCache", DefaultValue = false)]
        public bool EnableASPNetCache
        {
            get { return (bool)base["enableASPNetCache"]; }
            set { base["enableASPNetCache"] = value; }
        }

        [ConfigurationProperty("supportEmail", DefaultValue = @"supportreflex.qc@cgi.com")]
        public string SupportEmail
        {
            get { return (string)base["supportEmail"]; }
            set { base["supportEmail"] = value; }
        }

        [ConfigurationProperty("slowRequestThreshold", DefaultValue = @"2000")]
        public int SlowRequestThreshold
        {
            get { return (int)base["slowRequestThreshold"]; }
            set { base["slowRequestThreshold"] = value; }
        }

        public AuthenticationMode AuthenticationMode
        {
            get
            {
                try
                {
                    if (!_authenticationMode.HasValue)
                    {
                        var webSectionGroup = (SystemWebSectionGroup)WebConfigurationManager.OpenWebConfiguration("~").GetSectionGroup("system.web");
                        if ((webSectionGroup == null) || (webSectionGroup.Authentication == null))
                        {
                            _authenticationMode = AuthenticationMode.None;
                        }
                        else
                        {
                            _authenticationMode = webSectionGroup.Authentication.Mode;
                        }
                    }

                    return _authenticationMode.Value;
                }
                catch (ArgumentException ex)
                {
                    Log.Error("There has been an error while reading AuthenticationMode in configuration.", ex);
                    return AuthenticationMode.None;
                }
            }

            set
            {
                _authenticationMode = value;
            }
        }

        public static ReflexConfigurationSection GetConfigurationSection()
        {
            return GetConfigurationSection(ConfigurationSectionName);
        }

        public static ReflexConfigurationSection GetConfigurationSection(string section)
        {
            var config = (ReflexConfigurationSection)ConfigurationManager.GetSection(section);
            return config ?? new ReflexConfigurationSection();
        }
    }
}