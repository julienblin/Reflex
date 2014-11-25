// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeeder.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries;
using NHibernate;

namespace CGI.Reflex.Core.Seed
{
    public class ApplicationSeeder : BaseSeeder
    {
        public override int Priority { get { return 20; } }

        protected override void SeedImpl()
        {
            var aurora = new Application
            {
                Name = "Aurora",
                Code = "TST1234",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Application maison"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "1"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application Aurora",
                    MaintenanceWindow = "Lundi de 12h à 13h",
                    Notes = "Notes d'aurora"
                }
            };
            aurora.AddTechnologyLinks(GetTechnos("Microsoft Internet Explorer 6.0", "Microsoft .Net 2.0", "Microsoft Visual Basic 6.0"));

            Session.Save(aurora);

            var gerico = new Application
            {
                Name = "Gerico",
                Code = "TEST2487",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "COTS"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "2"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application Gerico",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes gerico"
                }
            };

            gerico.AddTechnologyLinks(GetTechnos("Microsoft Internet Explorer 6.0", "Microsoft Visual Basic 6.0"));
            Session.Save(gerico);

            var adn = new Application
            {
                Name = "@DN",
                Code = "TEST2455",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Application maison"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "2"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application @DN",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes @DN"
                }
            };

            adn.AddTechnologyLinks(GetTechnos("Oracle 8i"));
            Session.Save(adn);

            var cosmos = new Application
            {
                Name = "cosmos",
                Code = "TEST2486",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "COTS"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "3"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application cosmos",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes cosmos"
                }
            };

            Session.Save(cosmos);

            var makeup = new Application
            {
                Name = "Makeup",
                Code = "WHKDG521",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Saas"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "1"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "Potentiel"),
                    Description = "Description de l'application makeup",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes makeup"
                }
            };

            makeup.AddTechnologyLinks(GetTechnos("Open Source Firefox"));
            Session.Save(makeup);

            var cpo = new Application
            {
                Name = "CPO",
                Code = "45456674",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Saas"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "1"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "Retiré"),
                    Description = "Description de l'application cpo",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes cpo"
                }
            };

            Session.Save(cpo);

            var costumeinventory = new Application
            {
                Name = "Costume Inventory",
                Code = "4543",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Application maison"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "2"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application costumeinventory",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes costumeinventory"
                }
            };

            Session.Save(costumeinventory);

            var lucioles = new Application
            {
                Name = "Lucioles",
                Code = "454354",
                ApplicationType = Get(DomainValueCategory.ApplicationType, "Application maison"),
                AppInfo =
                {
                    Criticity = Get(DomainValueCategory.ApplicationCriticity, "3"),
                    Status = Get(DomainValueCategory.ApplicationStatus, "En production"),
                    Description = "Description de l'application lucioles",
                    MaintenanceWindow = "Lundi de 18h à 23h",
                    Notes = "Notes lucioles"
                }
            };

            Session.Save(lucioles);
        }
    }
}