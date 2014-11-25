// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace CGI.Reflex.Commands
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Allow dynamic loading of embedded dlls.
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arArgs) =>
            {
                var resourceName = "CGI.Reflex.Commands.EmbeddedAssemblies." + new AssemblyName(arArgs.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        return null;

                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            Utils.SetUp(args);
        }
    }
}
