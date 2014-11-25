using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Tests.Factories
{
    public class DatabaseFactory : BaseFactory<Database>
    {
        public DatabaseFactory(AllFactories factories)
            : base(factories)
        {
            
        }

        protected override Database CreateImpl()
        {
            return new Database
            {
                Name = Rand.String(15),
                Description = Rand.LoremIpsum()
            };
        }
    }
}
