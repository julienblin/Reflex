// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppLineCriteria.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;

namespace CGI.Reflex.Core.Queries.Series.Criteria
{
    internal interface IAppLineCriteria
    {
        string DisplayName { get; }

        string TechnicalName { get; }

        IEnumerable<Type> ExcludedColumnCriterias { get; }

        bool OnlyActiveApplications { get; set; }

        LineMultiplicities LineMultiplicities { get; }

        void ApplyJoins(IQueryOver<Application, Application> query);

        void SelectGroup(QueryOverProjectionBuilder<Application> list);

        void SelectCountIfNoColumns(QueryOverProjectionBuilder<Application> list);

        IList<ApplicationSeriesResultLine> Order(IList<ApplicationSeriesResultLine> lines);

        void ColumnCorrelation(QueryOver<Application, Application> subQuery);

        void ColumnCorrelation(QueryOver<Application, AppInfo> appInfoQueryOver);
    }
}
