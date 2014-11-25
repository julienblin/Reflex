// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptConfig.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class EncryptConfig : ConsoleCommand
    {
        internal const string ProtectionProvider = @"RSAProtectedConfigurationProvider";

        public EncryptConfig()
        {
            IsCommand("encrypt-config", "Encrypt the configuration file");

            Options = new OptionSet
            {
                { "f|file=", "Web Configuration file", f => FileName = f }
            };
        }

        public string FileName { get; set; }

        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(FileName))
                throw new OptionException("The file argument is mandatory.", "file");

            if (!File.Exists(FileName))
                throw new OptionException(string.Format("The file {0} doesn't exists.", FileName), "file");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, arArgs) =>
            {
                var directoryName = Path.GetDirectoryName(FileName);
                if (directoryName == null)
                    return null;
                var targetAssembly = Path.Combine(directoryName, "bin", new AssemblyName(arArgs.Name).Name + ".dll");
                return Assembly.LoadFrom(targetAssembly);
            };

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = FileName };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var connectionStringsSection = config.Sections["connectionStrings"];
            if (connectionStringsSection != null && !connectionStringsSection.SectionInformation.IsProtected)
                connectionStringsSection.SectionInformation.ProtectSection(ProtectionProvider);

            var reflexSection = config.Sections["reflex"];
            if (reflexSection != null && !reflexSection.SectionInformation.IsProtected)
                reflexSection.SectionInformation.ProtectSection(ProtectionProvider);

            var smtpSection = config.GetSection("system.net/mailSettings/smtp");
            if (smtpSection != null && !smtpSection.SectionInformation.IsProtected)
                smtpSection.SectionInformation.ProtectSection(ProtectionProvider);

            config.Save();
            return 0;
        }
    }
}
