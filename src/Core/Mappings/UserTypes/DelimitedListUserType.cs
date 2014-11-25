// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelimitedListUserType.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace CGI.Reflex.Core.Mappings.UserTypes
{
    [Serializable]
    public class DelimitedListUserType<T> : IUserType
    {
        private const string Delimiter = @"|";

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { new SqlType(DbType.String) };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(ICollection<T>); }
        }

        public bool IsMutable
        {
            get { return true; }
        }

        bool IUserType.Equals(object x, object y)
        {
            if (x == null) return false;
            if (y == null) return false;

            return ((IEnumerable<T>)x).SequenceEqual((IEnumerable<T>)y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var r = rs[names[0]];
            if (r == DBNull.Value)
                return new List<T>();
            var strValues = ((string)r).Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            
            if (typeof(T) == typeof(string))
                return strValues.ToList();

            var descriptor = TypeDescriptor.GetConverter(typeof(T));
            return strValues.Select(descriptor.ConvertFromString).Cast<T>().ToList();
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                return;
            }

            NHibernateUtil.String.NullSafeSet(cmd, string.Join(Delimiter, (IEnumerable<T>)value), index);
        }

        public object DeepCopy(object value)
        {
            if (value == null) return null;

            return ((IEnumerable<T>)value).ToArray().ToList();
        }

        public object Replace(object original, object target, object owner)
        {
            return DeepCopy(original);
        }

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }

        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }
    }
}
