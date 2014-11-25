// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorUserType.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace CGI.Reflex.Core.Mappings.UserTypes
{
    [Serializable]
    public class ColorUserType : IUserType
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
            get { return typeof(Color); }
        }

        public bool IsMutable
        {
            get { return false; }
        }

        bool IUserType.Equals(object x, object y)
        {
            if (x == null) return false;
            if (y == null) return false;

            return ((Color)x).Equals((Color)y);
        }

        public int GetHashCode(object x)
        {
            throw new NotImplementedException();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var r = rs[names[0]];
            return r == DBNull.Value
                    ? Color.Empty
                    : ColorTranslator.FromHtml((string)r);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null || ((Color)value).IsEmpty)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                return;
            }

            NHibernateUtil.String.NullSafeSet(cmd, ColorTranslator.ToHtml((Color)value), index);
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
