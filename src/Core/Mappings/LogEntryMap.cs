// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryMap.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

namespace CGI.Reflex.Core.Mappings
{
    public class LogEntryMap : BaseEntityMap<LogEntry>
    {
        public LogEntryMap()
        {
            Map(x => x.Date);
            Map(x => x.Thread);
            Map(x => x.Level);
            Map(x => x.Logger);
            Map(x => x.CorrelationId);
            Map(x => x.LoggedUser);
            Map(x => x.Context);
            Map(x => x.Message);
            Map(x => x.Exception);
        }
    }
}
