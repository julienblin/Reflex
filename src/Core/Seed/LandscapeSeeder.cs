// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LandscapeSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class LandscapeSeeder : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            Session.Save(new Landscape
            {
                Name = "WEB001",
                Description = "Le landscape 1 Web"
            });

            Session.Save(new Landscape
            {
                Name = "SQL005",
                Description = "Le landscape 1 SQL"
            });
        }
    }
}
