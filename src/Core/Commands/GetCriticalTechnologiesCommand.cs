// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCriticalTechnologiesCommand.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;

using NHibernate.Criterion;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Commands
{
    public class GetCriticalTechnologiesCommand : AbstractCommand<IEnumerable<CriticalTechnologiesResultLine>>
    {
        protected override IEnumerable<CriticalTechnologiesResultLine> ExecuteImpl()
        {
            CriticalTechnologiesResultLine lineAlias = null;
            Technology technoAlias = null;
            var appLinksCountQuery = QueryOver.Of<AppTechnoLink>()
                .Where(atl => atl.Technology.Id == technoAlias.Id)
                .SelectList(list => list.SelectCountDistinct(atl => atl.Application.Id));

            var serverLinksCountQuery = QueryOver.Of<ServerTechnoLink>()
                .Where(stl => stl.Technology.Id == technoAlias.Id)
                .SelectList(list => list.SelectCountDistinct(stl => stl.Server.Id));

            var intLinksCountQuery = QueryOver.Of<IntegrationTechnoLink>()
                .Where(itl => itl.Technology.Id == technoAlias.Id)
                .SelectList(list => list.SelectCountDistinct(itl => itl.Integration.Id));

            var dbInstanceLinksCountQuery = QueryOver.Of<DbInstanceTechnoLink>()
                .Where(dbtl => dbtl.Technology.Id == technoAlias.Id)
                .SelectList(list => list.SelectCountDistinct(dbtl => dbtl.DbInstance.Id));

            var query = NHSession.QueryOver(() => technoAlias)
                .Where(t => t.EndOfSupport != null)
                .SelectList(list => list
                                        .SelectGroup(t => t.Id).WithAlias(() => lineAlias.TechnologyId)
                                        .SelectGroup(t => t.EndOfSupport).WithAlias(() => lineAlias.EndOfSupport)
                                        .SelectSubQuery(appLinksCountQuery).WithAlias(() => lineAlias.AppLinkCount)
                                        .SelectSubQuery(serverLinksCountQuery).WithAlias(() => lineAlias.ServerLinkCount)
                                        .SelectSubQuery(intLinksCountQuery).WithAlias(() => lineAlias.IntLinkCount)
                                        .SelectSubQuery(dbInstanceLinksCountQuery).WithAlias(
                                            () => lineAlias.DbInstanceLinkCount));
            query.TransformUsing(Transformers.AliasToBean<CriticalTechnologiesResultLine>());

            var lines = query.List<CriticalTechnologiesResultLine>();
            return lines.OrderByDescending(l => l.Score);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class CriticalTechnologiesResultLine
    {
        public int TechnologyId { get; set; }

        public DateTime? EndOfSupport { get; set; }

        public int AppLinkCount { get; set; }

        public int ServerLinkCount { get; set; }

        public int IntLinkCount { get; set; }

        public int DbInstanceLinkCount { get; set; }

        public int TotalCount
        {
            get { return AppLinkCount + ServerLinkCount + IntLinkCount + DbInstanceLinkCount; }
        }

        public int Score
        {
            get
            {
                var baseMultiplier = 1;
                if (EndOfSupport <= DateTime.Today.AddYears(1) && EndOfSupport > DateTime.Today)
                    baseMultiplier = 50;
                if (EndOfSupport < DateTime.Today)
                    baseMultiplier = 100;

                return baseMultiplier * TotalCount;
            }
        }

        public Technology GetTechno()
        {
            return References.NHSession.Load<Technology>(TechnologyId);
        }
    }
}