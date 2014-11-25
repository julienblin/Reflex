// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLServerExceptionConverter.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

using NHibernate;
using NHibernate.Exceptions;

namespace CGI.Reflex.Core.Mappings.ExceptionConverters
{
    public class SQLServerExceptionConverter : ISQLExceptionConverter
    {
        public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
        {
            var sqlException = ADOExceptionHelper.ExtractDbException(adoExceptionContextInfo.SqlException) as SqlException;

            if (sqlException != null)
            {
                switch (sqlException.Number)
                {
                    case 547:
                        return new ConstraintViolationException(adoExceptionContextInfo.Message, sqlException.InnerException, adoExceptionContextInfo.Sql);
                    case 208:
                        return new SQLGrammarException(adoExceptionContextInfo.Message, sqlException.InnerException, adoExceptionContextInfo.Sql);
                    case 3960:
                        return new StaleObjectStateException(adoExceptionContextInfo.EntityName, adoExceptionContextInfo.EntityId);
                }
            }

            return SQLStateConverter.HandledNonSpecificException(
                adoExceptionContextInfo.SqlException,
                adoExceptionContextInfo.Message,
                adoExceptionContextInfo.Sql);
        }
    }
}