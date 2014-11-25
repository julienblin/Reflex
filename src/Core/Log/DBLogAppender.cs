// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DBLogAppender.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Layout;

namespace CGI.Reflex.Core.Log
{
    [ExcludeFromCodeCoverage]
    public class DBLogAppender : AdoNetAppender
    {
        public DBLogAppender()
        {
            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@date",
                DbType = DbType.DateTime,
                Layout = new RawTimeStampLayout()
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@thread",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@level",
                DbType = DbType.String,
                Size = 50,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@logger",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@correlationId",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{CorrelationId}"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@loggedUser",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{User}"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@context",
                DbType = DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{Context}"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@message",
                DbType = DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"))
            });

            AddParameter(new AdoNetAppenderParameter
            {
                ParameterName = "@exception",
                DbType = DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new ExceptionLayout())
            });

            BufferSize = 1;
        }

        public static void ApplyConfigurationFromReferences()
        {
            var repo = log4net.LogManager.GetRepository();

            if (repo != null)
            {
                var dbLogAppender = (DBLogAppender)repo.GetAppenders().SingleOrDefault(a => a.GetType() == typeof(DBLogAppender));
                if (dbLogAppender != null)
                {
                    dbLogAppender.ConnectionString = References.ReferencesConfiguration.ConnectionString;

                    switch (References.ReferencesConfiguration.DatabaseType)
                    {
                        case DatabaseType.SQLite:
                            // No direct reference with SQLite
                            dbLogAppender.ConnectionType = @"System.Data.SQLite.SQLiteConnection, System.Data.SQLite, Version=1.0.82.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139";
                            dbLogAppender.CommandText = @"INSERT INTO LogEntries (Date, Thread, Level, Logger, CorrelationId, LoggedUser, Context, Message, Exception) VALUES (@date, @thread, @level, @logger, @correlationId, @loggedUser, @context, @message, @exception)";
                            break;
                        case DatabaseType.SqlServer2008:
                            dbLogAppender.ConnectionType = typeof(System.Data.SqlClient.SqlConnection).AssemblyQualifiedName;
                            dbLogAppender.CommandText = @"INSERT INTO LogEntries ([Date],[Thread],[Level],[Logger],[CorrelationId],[LoggedUser],[Context],[Message],[Exception]) VALUES (@date, @thread, @level, @logger, @correlationId, @loggedUser, @context, @message, @exception)";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    dbLogAppender.ActivateOptions();
                }
            }
        }
    }
}
