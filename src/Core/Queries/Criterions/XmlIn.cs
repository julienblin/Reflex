// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlIn.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

namespace CGI.Reflex.Core.Queries.Criterions
{
    /// <summary>
    /// Criterion that allows the passing of more values in IN clause than SQLServer authorized (hard 2100 parameters limit)
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class XmlIn : AbstractCriterion
    {
        private readonly AbstractCriterion _expr;
        private readonly string _propertyName;
        private readonly int _maxNumberOfParametersToNotUseXml;
        private readonly object[] _values;

        /// <summary>
        /// Criterion that allows the passing of more values in IN clause than SQLServer authorized (hard 2100 parameters limit)
        /// </summary>
        /// <param name="propertyName">Name of entity property</param>
        /// <param name="values">Values for IN(...) clause</param>
        /// <param name="maxNumberOfParametersToNotUseXml">Threshold for the use of XML fallback. Below, a normal IN clause will be used. (default 200)</param>
        public XmlIn(string propertyName, IEnumerable values, int maxNumberOfParametersToNotUseXml = 200)
        {
            _propertyName = propertyName;
            _maxNumberOfParametersToNotUseXml = maxNumberOfParametersToNotUseXml;
            _values = values.Cast<object>().ToArray();
            _expr = Restrictions.In(propertyName, _values);
        }

        public override string ToString()
        {
            return _propertyName + " xmlin (" + StringHelper.ToString(_values) + ')';
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            if (References.ReferencesConfiguration.DatabaseType != DatabaseType.SqlServer2008)
                return _expr.ToSqlString(criteria, criteriaQuery, enabledFilters);

            if (_values.Length < _maxNumberOfParametersToNotUseXml)
                return _expr.ToSqlString(criteria, criteriaQuery, enabledFilters);

            var type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);
            if (type.IsCollectionType)
                throw new QueryException("Cannot use collections with InExpression");

            if (_values.Length == 0)
                return new SqlString("1=0");

            var result = new SqlStringBuilder();
            var columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, _propertyName);

            var parameters = GetTypedValues(criteria, criteriaQuery).SelectMany(criteriaQuery.NewQueryParameter).ToArray();

            for (int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
            {
                string columnName = columnNames[columnIndex];

                if (columnIndex > 0)
                    result.Add(" and ");

                var sqlType = type.SqlTypes(criteriaQuery.Factory)[columnIndex];
                result
                    .Add(columnName)
                    .Add(" in (")
                    .Add("SELECT ParamValues.Val.value('.','")
                    .Add(criteriaQuery.Factory.Dialect.GetTypeName(sqlType))
                    .Add("') FROM ")
                    .Add(parameters[columnIndex])
                    .Add(".nodes('/items/val') as ParamValues(Val)")
                    .Add(")");
            }

            return result.ToSqlString();
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            if (References.ReferencesConfiguration.DatabaseType != DatabaseType.SqlServer2008)
                return _expr.GetTypedValues(criteria, criteriaQuery);

            if (_values.Length < _maxNumberOfParametersToNotUseXml)
                return _expr.GetTypedValues(criteria, criteriaQuery);

            IEntityPersister persister = null;
            var type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);

            if (type.IsEntityType)
                persister = criteriaQuery.Factory.GetEntityPersister(type.ReturnedClass.FullName);

            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw);
            writer.WriteStartElement("items");
            foreach (object value in _values)
            {
                if (value == null)
                    continue;
                var valToWrite = persister != null ? persister.GetIdentifier(value, EntityMode.Poco) : value;
                writer.WriteElementString("val", valToWrite.ToString());
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            var xmlString = sw.GetStringBuilder().ToString();

            return new[]
            {
                new TypedValue(
                    new CustomType(
                        typeof(XmlType),
                        new Dictionary<string, string>()),
                    xmlString,
                    EntityMode.Poco)
            };
        }

        public override IProjection[] GetProjections()
        {
            if (References.ReferencesConfiguration.DatabaseType != DatabaseType.SqlServer2008)
                return _expr.GetProjections();

            if (_values.Length < _maxNumberOfParametersToNotUseXml)
                return _expr.GetProjections();

            return null;
        }

        private class XmlType : IUserType
        {
            private static readonly SqlType[] SQLTypes = new[] { new SqlType(DbType.Xml) };
            
            private readonly Type _returnedType = typeof(string);

            public SqlType[] SqlTypes
            {
                get { return SQLTypes; }
            }

            public Type ReturnedType
            {
                get { return _returnedType; }
            }

            public bool IsMutable
            {
                get { return false; }
            }

            bool IUserType.Equals(object x, object y)
            {
                return Equals(x, y);
            }

            public int GetHashCode(object x)
            {
                return x.GetHashCode();
            }

            public object NullSafeGet(IDataReader rs, string[] names, object owner)
            {
                return null;
            }

            public void NullSafeSet(IDbCommand cmd, object value, int index)
            {
                var parameter = (IDataParameter)cmd.Parameters[index];
                parameter.Value = value;
            }

            public object DeepCopy(object value)
            {
                return value;
            }

            public object Replace(object original, object target, object owner)
            {
                return original;
            }

            public object Assemble(object cached, object owner)
            {
                return cached;
            }

            public object Disassemble(object value)
            {
                return value;
            }
        }
    }
}
