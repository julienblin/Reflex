// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class ApplicationFactory : BaseFactory<Application>
    {
        public ApplicationFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Application CreateImpl()
        {
            return new Application
            {
                Name = Rand.String(40),
                Code = Rand.String(10),
                ApplicationType = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationType),
                AppInfo =
                {
                    Status = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationStatus),
                    Criticity = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCriticity),
                    UserRange = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationUserRange),
                    Description = Rand.LoremIpsum(),
                    Certification = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.ApplicationCertification),
                    MaintenanceWindow = Rand.LoremIpsum(),
                    Notes = Rand.LoremIpsum()
                }
            };
        }
    }
}
