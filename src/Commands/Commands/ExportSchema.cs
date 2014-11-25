// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportSchema.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using CGI.Reflex.Core;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class ExportSchema : ConsoleCommand
    {
        public ExportSchema()
        {
            IsCommand("export-schema", "Exports the database schema to a file or a target database.");

            Options = new OptionSet
            {
                { "t|databaseType=", "Type of database", (DatabaseType db) => DatabaseType = db },
                { "c|connectionString=", "Connection string for the target database", cs => ConnectionString = cs },
                { "f|file=", "File to export the database to", f => File = f },
            };
        }

        public DatabaseType DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public string File { get; set; }

        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(File) && string.IsNullOrEmpty(ConnectionString))
            {
                Console.WriteLine(References.DatabaseOperations.ExportSchema(new ReferencesConfiguration { DatabaseType = DatabaseType }));
            }

            if (!string.IsNullOrEmpty(File))
            {
                var fileInfo = new FileInfo(File);
                if (fileInfo.Exists)
                    fileInfo.Delete();

                Console.WriteLine(string.Format("Exporting database to {0}...", fileInfo.FullName));
                References.DatabaseOperations.ExportSchema(new ReferencesConfiguration { DatabaseType = DatabaseType }, fileInfo.FullName);
            }

            if (!string.IsNullOrEmpty(ConnectionString))
            {
                References.Configure(new ReferencesConfiguration { DatabaseType = DatabaseType, ConnectionString = ConnectionString });
                References.DatabaseOperations.ExportSchema();
            }

            return 0;
        }
    }
}
