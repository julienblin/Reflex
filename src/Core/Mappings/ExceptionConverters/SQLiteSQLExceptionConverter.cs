// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteSQLExceptionConverter.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using NHibernate.Exceptions;

namespace CGI.Reflex.Core.Mappings.ExceptionConverters
{
    public class SQLiteSQLExceptionConverter : ISQLExceptionConverter
    {
        public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
        {
            var sqliteException = ADOExceptionHelper.ExtractDbException(adoExceptionContextInfo.SqlException);

            // No direct reference with SQLite - need to go with reflexion.
            if ((sqliteException != null) && (sqliteException.GetType().Name == "SQLiteException"))
            {
                var returnCodeProperty = sqliteException.GetType()
                    .GetProperties().FirstOrDefault(p => p.Name == "ResultCode" && p.PropertyType.Name == "SQLiteErrorCode");
                if (returnCodeProperty != null)
                {
                    var returnCode = returnCodeProperty.GetGetMethod().Invoke(sqliteException, new object[0]).ToString();
                    switch (returnCode)
                    {
                        case "Constraint":
                            return new ConstraintViolationException(
                                adoExceptionContextInfo.Message,
                                sqliteException,
                                adoExceptionContextInfo.Sql);
                    }
                }
            }

            return SQLStateConverter.HandledNonSpecificException(
                adoExceptionContextInfo.SqlException,
                adoExceptionContextInfo.Message,
                adoExceptionContextInfo.Sql);
        }
    }
}