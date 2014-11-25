// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLServerExceptionConverterTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CGI.Reflex.Core.Mappings.ExceptionConverters;

using FluentAssertions;

using NHibernate;
using NHibernate.Exceptions;

using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Mappings.ExceptionConverters
{
    [TestFixture]
    public class SQLServerExceptionConverterTest
    {
        [TestCase(547, typeof(ConstraintViolationException))]
        [TestCase(208, typeof(SQLGrammarException))]
        [TestCase(3960, typeof(StaleObjectStateException))]
        [TestCase(2, typeof(GenericADOException))]
        public void It_should_convert_exceptions(int number, Type convertedExceptionType)
        {
            var converter = new SQLServerExceptionConverter();
            
            var collection = Construct<SqlErrorCollection>();
            var error = Construct<SqlError>(number, (byte)2, (byte)3, "server name", "error message", "proc", 100, (uint)2);

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(collection, new object[] { error });

            var sqlException = typeof(SqlException)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(m => m.Name.Equals("CreateException", StringComparison.InvariantCultureIgnoreCase) && m.GetParameters().Length == 2)
                .Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;

            converter.Convert(new AdoExceptionContextInfo() { SqlException = sqlException })
                     .GetType()
                     .Should()
                     .Be(convertedExceptionType);
        }

        private static T Construct<T>(params object[] p)
        {
            return (T)typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(p);
        }
    }
}
