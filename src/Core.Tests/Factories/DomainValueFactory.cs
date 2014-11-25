// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class DomainValueFactory : BaseFactory<DomainValue>
    {
        public DomainValueFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override DomainValue CreateImpl()
        {
            return new DomainValue
            {
                Category = Rand.Enum<DomainValueCategory>(),
                DisplayOrder = Rand.Int(10),
                Name = Rand.String(20),
                Description = Rand.LoremIpsum(2000)
            };
        }
    }
}
