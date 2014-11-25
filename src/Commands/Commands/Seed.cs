// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Seed.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class Seed : ConsoleCommand
    {
        public Seed()
        {
            IsCommand("seed", "Seed a database with default values.");

            Options = new OptionSet
            {
                { "t|databaseType=", "Type of database", (DatabaseType db) => DatabaseType = db },
                { "c|connectionString=", "Connection string for the target database", cs => ConnectionString = cs }
            };
        }

        public DatabaseType DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new OptionException("The connectionString argument is mandatory.", "connectionString");

            References.Configure(new ReferencesConfiguration { DatabaseType = DatabaseType, ConnectionString = ConnectionString });
            References.DatabaseOperations.Seed();

            return 0;
        }
    }
}
