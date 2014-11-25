// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteSQLExceptionConverterTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CGI.Reflex.Core.Mappings.ExceptionConverters;

using FluentAssertions;

using NHibernate.Exceptions;

using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Mappings.ExceptionConverters
{
    [TestFixture]
    public class SQLiteSQLExceptionConverterTest
    {
        [TestCase(SQLiteErrorCode.Constraint, typeof(ConstraintViolationException))]
        [TestCase(SQLiteErrorCode.Internal, typeof(GenericADOException))]
        public void It_should_convert_exceptions(SQLiteErrorCode errorCode, Type convertedExceptionType)
        {
            var converter = new SQLiteSQLExceptionConverter();
            converter.Convert(new AdoExceptionContextInfo() { SqlException = new SQLiteException(errorCode, Rand.String()) })
                     .GetType()
                     .Should()
                     .Be(convertedExceptionType);
        }
    }
}
