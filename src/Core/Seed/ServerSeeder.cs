// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSeeder.cs" company="CGI">
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
    public class ServerSeeder : BaseSeeder
    {
        public override int Priority { get { return 10; } }

        public Landscape GetLandscape(string name)
        {
            return new LandscapeQuery { NameLike = name }.SingleOrDefault(Session);
        }

        protected override void SeedImpl()
        {
            var mtlmdvweb001 = new Server
            {
                Name = "MTLMDVWEB001",
                Alias = "WHMM004",
                Comments = "Serveur  de développement 1",
                EvergreenDate = new DateTime(2014, 02, 22),
                Environment = Get(DomainValueCategory.Environment, "Dev"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Virtuel"),
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmdvweb001);

            var mtlmdvweb003 = new Server
            {
                Name = "MTLMDVWEB003",
                Alias = "WHMM005",
                Comments = "Serveur  de développement 2",
                EvergreenDate = new DateTime(2014, 02, 22),
                Environment = Get(DomainValueCategory.Environment, "Dev"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Virtuel"),
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmdvweb003);

            var mtlmtsweb001 = new Server
            {
                Name = "MTLMTSWEB001",
                Alias = "WHMM003",
                Comments = "Serveur  de test 3",
                EvergreenDate = DateTime.Today,
                Environment = Get(DomainValueCategory.Environment, "Test"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = null,
                Type = Get(DomainValueCategory.ServerType, "Virtuel"),
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmtsweb001);

            var mtlmtsweb003 = new Server
            {
                Name = "MTLMTSWEB003",
                Alias = "WHMM006",
                Comments = "Serveur  de test 3",
                EvergreenDate = DateTime.Today,
                Environment = Get(DomainValueCategory.Environment, "Test"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = null,
                Type = Get(DomainValueCategory.ServerType, "Virtuel"),
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmtsweb003);

            var mtlmppweb001 = new Server
            {
                Name = "MTLMPPWEB001",
                Alias = "WHMM001",
                Comments = "Serveur  de Pré-production  1",
                EvergreenDate = null,
                Environment = Get(DomainValueCategory.Environment, "Preprod"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = null,
                Type = null,
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmppweb001);

            var mtlmprweb001 = new Server
            {
                Name = "MTLMPRWEB001",
                Alias = "WHMM002",
                Comments = "Serveur  de Production 1",
                EvergreenDate = new DateTime(2016, 08, 22),
                Environment = Get(DomainValueCategory.Environment, "Prod"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur web"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Virtuel"),
                Landscape = GetLandscape("Web001")
            };

            Session.Save(mtlmprweb001);

            var mtlmdvsql005 = new Server
            {
                Name = "MTLMDVSQL005",
                Alias = "WHMM007",
                Comments = "Serveur sql de dev",
                EvergreenDate = new DateTime(2016, 08, 22),
                Environment = Get(DomainValueCategory.Environment, "Dev"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur de BD"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Physique"),
                Landscape = GetLandscape("SQL005")
            };

            Session.Save(mtlmdvsql005);

            var mtlmtssql005 = new Server
            {
                Name = "MTLMTSSQL005",
                Alias = "WHMM008",
                Comments = "Serveur sql de test",
                EvergreenDate = new DateTime(2016, 08, 22),
                Environment = Get(DomainValueCategory.Environment, "Test"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur de BD"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Physique"),
                Landscape = GetLandscape("SQL005")
            };

            Session.Save(mtlmtssql005);

            var mtlmppsql005 = new Server
            {
                Name = "MTLMPPSQL005",
                Alias = "WHMM009",
                Comments = "Serveur sql de preprod",
                EvergreenDate = new DateTime(2016, 08, 22),
                Environment = Get(DomainValueCategory.Environment, "Preprod"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur de BD"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Physique"),
                Landscape = GetLandscape("SQL005")
            };

            Session.Save(mtlmppsql005);

            var mtlmprsql005 = new Server
            {
                Name = "MTLMPRSQL005",
                Alias = "WHMM010",
                Comments = "Serveur sql de prod",
                EvergreenDate = new DateTime(2016, 08, 22),
                Environment = Get(DomainValueCategory.Environment, "Prod"),
                Role = Get(DomainValueCategory.ServerRole, "Serveur de BD"),
                Status = Get(DomainValueCategory.ServerStatus, "En service"),
                Type = Get(DomainValueCategory.ServerType, "Physique"),
                Landscape = GetLandscape("SQL005")
            };

            Session.Save(mtlmprsql005);
        }
    }
}
