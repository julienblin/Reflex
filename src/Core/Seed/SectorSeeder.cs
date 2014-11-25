// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorSeeder.cs" company="CGI">
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
    public class SectorSeeder : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            var hr = new Sector { Name = "HR" };
            hr.AddChild(new Sector { Name = "Recruitment" });
            hr.AddChild(new Sector { Name = "Pay" });
            Session.Save(hr);

            var marketing = new Sector { Name = "Marketing" };
            marketing.AddChild(new Sector { Name = "Merchandising" });
            marketing.AddChild(new Sector { Name = "Publicity" });
            Session.Save(marketing);

            var financial = new Sector { Name = "Financial" };
            Session.Save(financial);
        }
    }
}
