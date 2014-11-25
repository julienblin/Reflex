// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateConnectionString.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class CreateConnectionString : ConsoleCommand
    {
        public CreateConnectionString()
        {
            IsCommand("create-connection-string", "Create a connection string");

            Options = new OptionSet
            {
                { "t|databaseType=", "Type of database", (DatabaseType db) => DatabaseType = db },
                { "f|file=", "File to use", f => FileName = f },
                { "s|server=", "Server name (with named instance if needed)", s => ServerName = s },
                { "d|dbName=", "Name of the database", d => DbName = d },
                { "u|username=", "Username (in case of SQL Auth)", d => Username = d },
                { "p|password=", "Password (in case of SQL Auth)", d => Password = d }
            };
        }

        public DatabaseType DatabaseType { get; set; }

        public string FileName { get; set; }

        public string ServerName { get; set; }

        public string DbName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public override int Run(string[] remainingArguments)
        {
            string connectionString;
            switch (DatabaseType)
            {
                case DatabaseType.SqlServer2008:
                    if (string.IsNullOrEmpty(ServerName))
                        throw new OptionException("The server argument is mandatory when using SqlServer.", "server");

                    if (string.IsNullOrEmpty(DbName))
                        throw new OptionException("The dbName argument is mandatory when using SqlServer.", "dbName");

                    if (string.IsNullOrEmpty(Username))
                    {
                        connectionString = string.Format("Server={0};Database={1};Trusted_Connection=True;", ServerName, DbName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Password))
                            throw new OptionException("The password argument is mandatory when using SqlServer and providing a username.", "password");

                        connectionString = string.Format("Server={0};Database={1};User ID={2};Password={3};", ServerName, DbName, Username, Password);
                    }

                    break;
                case DatabaseType.SQLite:
                    if (string.IsNullOrEmpty(FileName))
                        throw new OptionException("The file argument is mandatory when using SQLite.", "file");

                    connectionString = string.Format("Data Source={0};Version=3;", FileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine(connectionString);
                if (Utils.SafeSetClipboard(connectionString))
                    Console.WriteLine(@"The connection string has been placed on the clipboard.");
            }

            return 0;
        }
    }
}
