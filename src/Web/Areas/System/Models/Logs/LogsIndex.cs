// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsIndex.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Web.Models;

namespace CGI.Reflex.Web.Areas.System.Models.Logs
{
    public class LogsIndex : AbstractSearchResultModel<LogEntry>
    {
        public string CorrelationId { get; set; }

        public string Level { get; set; }

        public IEnumerable<string> Levels { get; set; }

        public string Logger { get; set; }

        public IEnumerable<string> Loggers { get; set; }

        public string User { get; set; }

        public IEnumerable<string> Users { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public string Exception { get; set; }

        public bool SearchDefined
        {
            get
            { 
                return !string.IsNullOrEmpty(CorrelationId)
                     || !string.IsNullOrEmpty(Level)
                     || !string.IsNullOrEmpty(Logger)
                     || !string.IsNullOrEmpty(User)
                     || !string.IsNullOrEmpty(Message)
                     || !string.IsNullOrEmpty(Context)
                     || !string.IsNullOrEmpty(Exception);
            }
        }
    }
}