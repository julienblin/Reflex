// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSchema.cs" company="CGI">
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
    public class UpdateSchema : ConsoleCommand
    {
        public UpdateSchema()
        {
            IsCommand("update-schema", "Updates the database schema from a database to a file a database.");

            Options = new OptionSet
            {
                { "t|databaseType=", "Type of database", (DatabaseType db) => DatabaseType = db },
                { "c|connectionString=", "Connection string for the target database", cs => ConnectionString = cs },
                { "a|apply", "Apply the update to the database", a => Apply = a != null },
                { "f|file=", "File to export the database to", f => File = f },
            };
        }

        public DatabaseType DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public string File { get; set; }

        public bool Apply { get; set; }

        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new OptionException("The connectionString argument is mandatory.", "connectionString");

            if (string.IsNullOrEmpty(File) && !Apply)
            {
                Console.WriteLine(References.DatabaseOperations.UpdateSchema(new ReferencesConfiguration { DatabaseType = DatabaseType, ConnectionString = ConnectionString }));
            }

            if (!string.IsNullOrEmpty(File))
            {
                var fileInfo = new FileInfo(File);
                if (fileInfo.Exists)
                    fileInfo.Delete();

                Console.WriteLine(string.Format("Exporting update to {0}...", fileInfo.FullName));
                References.DatabaseOperations.UpdateSchema(new ReferencesConfiguration { DatabaseType = DatabaseType }, fileInfo.FullName);
            }

            if (Apply)
            {
                References.Configure(new ReferencesConfiguration { DatabaseType = DatabaseType, ConnectionString = ConnectionString });
                References.DatabaseOperations.UpdateSchema();
            }

            return 0;
        }
    }
}
