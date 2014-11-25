// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseAppColumnCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Resources;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal abstract class BaseAppColumnCriteria : IAppColumnCriteria
    {
        // Global aliases used everywhere
        // ReSharper disable InconsistentNaming
#pragma warning disable 649
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        protected Application appAlias;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        protected ApplicationSeriesResultLine lineAlias;
        //// ReSharper restore InconsistentNaming
#pragma warning restore 649

        private readonly string _resName;

        protected BaseAppColumnCriteria(string resName)
        {
            lineAlias = null;
            _resName = resName;
        }

        public virtual string DisplayName
        {
            get
            {
                var resManager = new ResourceManager(typeof(CoreResources));
                return resManager.GetString(_resName);
            }
        }

        public virtual string TechnicalName { get { return _resName; } }

        public abstract IList<object> GetColumns();

        public abstract void SelectCounts(QueryOverProjectionBuilder<Application> list, IList<object> columns, IAppLineCriteria lineCriteria);
    }
}
