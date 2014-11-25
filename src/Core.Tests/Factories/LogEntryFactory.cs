// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryFactory.cs" company="CGI">
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
    public class LogEntryFactory : BaseFactory<LogEntry>
    {
        public LogEntryFactory(AllFactories factories)
            : base(factories)
        {
        }

        protected override LogEntry CreateImpl()
        {
            return new LogEntry
            {
                Date = Rand.DateTime(),
                Thread = Rand.String(),
                Level = log4net.Core.Level.Info.Name,
                Logger = Rand.String(),
                CorrelationId = Rand.String(),
                Message = Rand.LoremIpsum(),
                Exception = Rand.LoremIpsum()
            };
        }
    }
}
