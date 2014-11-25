// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateUser.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using ManyConsole;
using NDesk.Options;

namespace CGI.Reflex.Commands.Commands
{
    public class CreateUser : ConsoleCommand
    {
        public CreateUser()
        {
            IsCommand("create-user", "Create a user.");

            Options = new OptionSet
            {
                { "t|databaseType=", "Type of database", (DatabaseType db) => DatabaseType = db },
                { "c|connectionString=", "Connection string for the target database", cs => ConnectionString = cs },
                { "e|email=", "Email of the user. Will be used also as user name", e => Email = e },
                { "p|password=", "Password", p => Password = p },
                { "k|encryptionKey=", "Encryption key used to generate password digest", k => EncryptionKey = k },
                { "r|role=", "Role of the user. If it doesn't exist will be created as Admin role.", r => Role = r }
            };
        }

        public DatabaseType DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string EncryptionKey { get; set; }

        public string Role { get; set; }

        public override int Run(string[] remainingArguments)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new OptionException("The connectionString argument is mandatory.", "connectionString");
            if (string.IsNullOrEmpty(Email))
                throw new OptionException("The email argument is mandatory.", "email");
            if (string.IsNullOrEmpty(Password))
                throw new OptionException("The password argument is mandatory.", "password");
            if (string.IsNullOrEmpty(EncryptionKey))
                throw new OptionException("The encryptionKey argument is mandatory.", "encryptionKey");
            if (string.IsNullOrEmpty(Role))
                throw new OptionException("The role argument is mandatory.", "role");

            References.Configure(new ReferencesConfiguration { DatabaseType = DatabaseType, ConnectionString = ConnectionString, EncryptionKey = EncryptionKey });

            using (var session = References.SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var role = new RoleQuery { NameLike = Role }.SingleOrDefault(session);
                if (role == null)
                {
                    role = new Role { Name = Role };
                    role.SetAllowedOperations(new[] { "*" });
                    session.Save(role);
                }

                var user = new User { UserName = Email, Email = Email, Role = role };
                session.Save(user);
                var userAuth = new UserAuthentication { User = user };
                userAuth.SetPassword(Password);
                session.Save(userAuth);
                tx.Commit();
            }

            return 0;
        }
    }
}
