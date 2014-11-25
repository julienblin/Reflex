// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppTechnoQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace CGI.Reflex.Core.Queries
{
    public class AppTechnoQuery : SingleResultQuery<AppTechnoQueryResult>
    {
        public int ApplicationId { get; set; }

        public override AppTechnoQueryResult Execute(ISession session)
        {
            var result = new AppTechnoQueryResult();

            // Direct link with application
            var subAppTechnoLinksQuery = QueryOver.Of<AppTechnoLink>()
                .Where(atl => atl.Application.Id == ApplicationId)
                .SelectList(list => list.Select(atl => atl.Technology.Id));

            result.ApplicationTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subAppTechnoLinksQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            // Link through integrations, without duplicates
            var subIntAppQuery = QueryOver.Of<Integration>()
                .Where(i => i.AppDest.Id == ApplicationId || i.AppSource.Id == ApplicationId)
                .SelectList(list => list.Select(i => i.Id));

            var subIntTechnoLinksQuery = QueryOver.Of<IntegrationTechnoLink>()
                .WithSubquery.WhereProperty(itl => itl.Integration.Id).In(subIntAppQuery)
                .SelectList(
                    list =>
                    list.Select(
                        Projections.Distinct(Projections.Property<IntegrationTechnoLink>(itl => itl.Technology.Id))));

            result.IntegrationTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subIntTechnoLinksQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            // Link through servers, without duplicates
            var subServerAppQuery = QueryOver.Of<AppServerLink>()
                .Where(asl => asl.Application.Id == ApplicationId)
                .SelectList(list => list.Select(asl => asl.Server.Id));

            var subServerTechnoLinksQuery = QueryOver.Of<ServerTechnoLink>()
                .WithSubquery.WhereProperty(stl => stl.Server.Id).In(subServerAppQuery)
                .SelectList(
                    list =>
                    list.Select(Projections.Distinct(Projections.Property<ServerTechnoLink>(stl => stl.Technology.Id))));

            result.ServerTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subServerTechnoLinksQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            // Link through dbInstances, without duplicates
            var subDbInstanceAppQuery = QueryOver.Of<AppDbInstanceLink>()
                .Where(adbil => adbil.Application.Id == ApplicationId)
                .SelectList(list => list.Select(adbil => adbil.DbInstances.Id));

            var subDbInstanceTechnoLinksQuery = QueryOver.Of<DbInstanceTechnoLink>()
                .WithSubquery.WhereProperty(dbitl => dbitl.DbInstance.Id).In(subDbInstanceAppQuery)
                .SelectList(
                    list =>
                    list.Select(
                        Projections.Distinct(Projections.Property<DbInstanceTechnoLink>(dbitl => dbitl.Technology.Id))));

            result.DbInstanceTechnologies = session.QueryOver<Technology>()
                .WithSubquery.WhereProperty(t => t.Id).In(subDbInstanceTechnoLinksQuery)
                .Fetch(t => t.TechnologyType).Eager
                .Fetch(t => t.Parent).Eager
                .Fetch(t => t.Parent.Parent).Eager
                .Fetch(t => t.Parent.Parent.Parent).Eager
                .Future();

            return result;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class AppTechnoQueryResult
    {
        public IEnumerable<Technology> ApplicationTechnologies { get; set; }

        public IEnumerable<Technology> IntegrationTechnologies { get; set; }

        public IEnumerable<Technology> ServerTechnologies { get; set; }

        public IEnumerable<Technology> DbInstanceTechnologies { get; set; }
    }
}