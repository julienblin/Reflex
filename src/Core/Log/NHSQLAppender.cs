// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NHSQLAppender.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using log4net.Appender;
using log4net.Core;

namespace CGI.Reflex.Core.Log
{
    [ExcludeFromCodeCoverage]
    public class NHSQLAppender : ForwardingAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggingEventData = loggingEvent.GetLoggingEventData();

            if (loggingEventData.Message.Contains("@p"))
            {
                var messageBuilder = new StringBuilder();

                string message = loggingEventData.Message;
                var queries = Regex.Split(message, @"command\s\d+:");

                foreach (var query in queries)
                    messageBuilder.Append(ReplaceQueryParametersWithValues(query));

                loggingEventData.Message = messageBuilder.ToString();
            }

            base.Append(new LoggingEvent(loggingEventData));
        }

        private static string ReplaceQueryParametersWithValues(string query)
        {
            return Regex.Replace(
                query,
                @"@p\d(?=[,);\s])(?!\s*=)",
                match =>
                {
                    var parameterValueRegex = new Regex(string.Format(@".*{0}\s*=\s*(.*?)\s*[\[].*", match));
                    return parameterValueRegex.Match(query).Groups[1].ToString();
                });
        }
    }
}
