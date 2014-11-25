// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflexConfiguration.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NHibernate;

namespace CGI.Reflex.Core.Entities
{
    public class ReflexConfiguration
    {
        public const int OnlyId = 1;

#pragma warning disable 649
// ReSharper disable ConvertToConstant.Local
        private int _id = OnlyId;
// ReSharper restore ConvertToConstant.Local
#pragma warning restore 649

        private ICollection<int> _activeAppStatusDVIds;

        public ReflexConfiguration()
        {
            _activeAppStatusDVIds = new List<int>();
        }

        public virtual int Id
        {
            get { return _id; }
        }

        [Display(ResourceType = typeof(CoreResources), Name = "ActiveAppStatusDVIds")]
        public virtual IEnumerable<int> ActiveAppStatusDVIds { get { return _activeAppStatusDVIds; } }

        public static ReflexConfiguration GetCurrent(ISession session = null)
        {
            if (session == null)
                session = References.NHSession;
            var conf = session.Load<ReflexConfiguration>(OnlyId);
            session.SetReadOnly(conf, true);
            return conf;
        }

        public virtual void SetActiveAppStatusDVIds(IEnumerable<int> values)
        {
            _activeAppStatusDVIds.Clear();
            foreach (var value in values)
                _activeAppStatusDVIds.Add(value);
        }

        public override string ToString()
        {
            return "Configuration";
        }
    }
}
