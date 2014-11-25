// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSeries.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Series.Criteria;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries.Series
{
    public class ApplicationSeries : SingleResultQuery<ApplicationSeriesResult>
    {
        private static readonly object SyncRoot = new object();

        private static volatile IEnumerable<IAppLineCriteria> _linesCriteria;

        private static volatile IEnumerable<IAppColumnCriteria> _columnsCriteria;

        public string LineCriteria { get; set; }

        public string ColumnCriteria { get; set; }

        public bool OnlyActiveApplications { get; set; }

        private static IEnumerable<IAppLineCriteria> LinesCriteria
        {
            get
            {
                if (_linesCriteria == null)
                {
                    lock (SyncRoot)
                    {
                        if (_linesCriteria == null)
                        {
                            _linesCriteria = typeof(ApplicationSeries).Assembly.GetTypes()
                                .Where(t => typeof(IAppLineCriteria).IsAssignableFrom(t)
                                            && !t.IsInterface
                                            && !t.IsAbstract)
                                .Select(t => (IAppLineCriteria)Activator.CreateInstance(t));
                        }
                    }
                }

                return _linesCriteria;
            }
        }

        private static IEnumerable<IAppColumnCriteria> ColumnsCriteria
        {
            get
            {
                if (_columnsCriteria == null)
                {
                    lock (SyncRoot)
                    {
                        if (_columnsCriteria == null)
                        {
                            _columnsCriteria = typeof(ApplicationSeries).Assembly.GetTypes()
                                .Where(t => typeof(IAppColumnCriteria).IsAssignableFrom(t)
                                            && !t.IsInterface
                                            && !t.IsAbstract)
                                .Select(t => (IAppColumnCriteria)Activator.CreateInstance(t));
                        }
                    }
                }

                return _columnsCriteria;
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> GetLinesCriteriaNames(bool includeManyToOne = true)
        {
            var result = LinesCriteria;
            if (!includeManyToOne)
                result = result.Where(crit => crit.LineMultiplicities != LineMultiplicities.ManyToOne);
            return result.Select(crit => new KeyValuePair<string, string>(crit.TechnicalName, crit.DisplayName));
        }

        public static IEnumerable<KeyValuePair<string, string>> GetColumnsCriteriaNames(string lineCriteriaName)
        {
            var lineCriteria = LinesCriteria.FirstOrDefault(c => c.TechnicalName == lineCriteriaName);
            if (lineCriteria == null) throw new NotSupportedException();

            return ColumnsCriteria.Where(c => !lineCriteria.ExcludedColumnCriterias.Contains(c.GetType()))
                                  .Select(crit => new KeyValuePair<string, string>(crit.TechnicalName, crit.DisplayName));
        }

        public static string GetLineDisplayName(string technicalName)
        {
            var lineCriteria = LinesCriteria.FirstOrDefault(l => l.TechnicalName == technicalName);
            if (lineCriteria == null) throw new NotSupportedException();
            return lineCriteria.DisplayName;
        }

        public static string GetColumnDisplayName(string technicalName)
        {
            var columnCriteria = ColumnsCriteria.FirstOrDefault(l => l.TechnicalName == technicalName);
            if (columnCriteria == null) throw new NotSupportedException();
            return columnCriteria.DisplayName;
        }

        public override ApplicationSeriesResult Execute(ISession session)
        {
            var result = new ApplicationSeriesResult
            {
                LineCriteria = LineCriteria,
                ColumnCriteria = ColumnCriteria
            };

            Application appAlias = null;

            var lineCriteria = LinesCriteria.FirstOrDefault(c => c.TechnicalName == LineCriteria);
            if (lineCriteria == null) throw new NotSupportedException();
            lineCriteria.OnlyActiveApplications = OnlyActiveApplications;
            result.LineMultiplicities = lineCriteria.LineMultiplicities;

            IAppColumnCriteria columnCriteria = null;

            if (!string.IsNullOrEmpty(ColumnCriteria))
            {
                columnCriteria = ColumnsCriteria.FirstOrDefault(c => c.TechnicalName == ColumnCriteria);
                if (columnCriteria == null) throw new NotSupportedException();
                if (lineCriteria.ExcludedColumnCriterias.Contains(columnCriteria.GetType())) throw new NotSupportedException();

                result.Columns = columnCriteria.GetColumns();   
            }

            var query = session.QueryOver(() => appAlias);
            lineCriteria.ApplyJoins(query);

            query.SelectList(list =>
            {
                lineCriteria.SelectGroup(list);

                if (columnCriteria == null)
                    lineCriteria.SelectCountIfNoColumns(list);
                else
                    columnCriteria.SelectCounts(list, result.Columns, lineCriteria);

                return list;
            });

            query.TransformUsing(Transformers.AliasToBean<ApplicationSeriesResultLine>());
            result.Lines = lineCriteria.Order(query.List<ApplicationSeriesResultLine>());
            return result;
        }
    }
}
