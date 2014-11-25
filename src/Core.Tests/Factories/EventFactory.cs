// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventFactory.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class EventFactory : BaseFactory<Event>
    {
        public EventFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override Event CreateImpl()
        {
            return new Event
            {
                Application = Factories.Application.Save(),
                Description = Rand.LoremIpsum(),
                Source = Rand.String(100),
                Date = Rand.DateTime(),
                Type = Factories.DomainValue.Save(dv => dv.Category = DomainValueCategory.EventType)
            };
        }
    }
}
