// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfigurationSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class ReflexConfigurationSeeder : BaseSeeder
    {
        public override int Priority { get { return 5; } }

        protected override void SeedImpl()
        {
            var conf = new ReflexConfiguration();
            var dv = new DomainValueQuery { Category = DomainValueCategory.ApplicationStatus, Name = "En production" }.List(Session);
            conf.SetActiveAppStatusDVIds(dv.Select(d => d.Id));
            Session.SaveOrUpdate(conf);
        }
    }
}
