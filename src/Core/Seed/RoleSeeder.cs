// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class RoleSeeder : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        public override bool IsMinimal
        {
            get
            {
                return true;
            }
        }

        protected override void SeedImpl()
        {
            var adminRole = new Role { Name = "Admin" };
            adminRole.SetAllowedOperations(new[] { "*" });
            Session.Save(adminRole);

            var spectatorRole = new Role { Name = "Spectator" };
            spectatorRole.SetAllowedOperations(new[]
            {
                "/Applications", "/Applications/Events", "/Applications/Integrations", "/Applications/Review",
                "/Servers", "/Servers/Landscapes", "/Servers/DbInstances",
                "/Technologies",
                "/Organizations/Sectors",
                "/Organizations/Contacts"
            });
            Session.Save(spectatorRole);
        }
    }
}