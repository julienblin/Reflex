// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseAppLineCriteria.cs" company="CGI">
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
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal abstract class BaseAppLineCriteria : IAppLineCriteria
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

        protected BaseAppLineCriteria(string resName)
        {
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

        public virtual IEnumerable<Type> ExcludedColumnCriterias
        { 
            get { return Enumerable.Empty<Type>(); }
        }

        public virtual bool OnlyActiveApplications { get; set; }

        public virtual LineMultiplicities LineMultiplicities { get { return LineMultiplicities.OneToOne; } }

        public abstract void ApplyJoins(IQueryOver<Application, Application> query);

        public abstract void SelectGroup(QueryOverProjectionBuilder<Application> list);

        public abstract void SelectCountIfNoColumns(QueryOverProjectionBuilder<Application> list);

        public abstract IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines);

        public virtual void ColumnCorrelation(QueryOver<Application, Application> subQuery)
        {
        }

        public virtual void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver)
        {
        }
    }
}
