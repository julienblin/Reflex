// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculationUserType.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Calculation;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace CGI.Reflex.Core.Mappings.UserTypes
{
    [Serializable]
    public class CalculationUserType : IUserType
    {
        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { new SqlType(DbType.String) };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(ICalculation); }
        }

        public bool IsMutable
        {
            get { return false; }
        }

        bool IUserType.Equals(object x, object y)
        {
            if (x == null && y == null) return true;
            if (x == null) return false;
            if (y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            throw new NotImplementedException();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var r = rs[names[0]];
            if (r == DBNull.Value)
                return null;

            var type = Type.GetType((string)r);
            return type == null ? null : Activator.CreateInstance(type);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                return;
            }

            NHibernateUtil.String.NullSafeSet(cmd, value.GetType().FullName, index);
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
