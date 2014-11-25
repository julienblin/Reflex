// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationQuery.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CGI.Reflex.Core.Entities;
using CGI.Reflex.Core.Queries.Criterions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace CGI.Reflex.Core.Queries
{
    public class ApplicationQuery : BaseQueryOver<Application>
    {
        private IQueryOver<Application, AppInfo> _appInfoQueryOver;

        public string NameOrCodeLike { get; set; }

        public string NameLike { get; set; }

        public string CodeLike { get; set; }

        public int? ApplicationTypeId { get; set; }

        public IEnumerable<int> ApplicationTypeIds { get; set; }

        public int? StatusId { get; set; }

        public IEnumerable<int> StatusIds { get; set; }

        public int? CriticityId { get; set; }

        public IEnumerable<int> CriticityIds { get; set; }

        public int? LinkedTechnologyId { get; set; }

        public int? LinkedDbInstanceId { get; set; }

        public int? LinkedContactId { get; set; }

        public int? LinkedSectorId { get; set; }

        public int? LinkedServerId { get; set; }

        public int? RoleId { get; set; }

        public bool? Active { get; set; }

        protected override IQueryOver<Application, Application> OverImpl(ISession session)
        {
            var query = session.QueryOver<Application>();
            _appInfoQueryOver = null;

            if (!string.IsNullOrEmpty(NameOrCodeLike))
                query.Where(
                    Restrictions.Or(
                        Restrictions.InsensitiveLike(Projections.Property<Application>(r => r.Name), NameOrCodeLike, MatchMode.Anywhere),
                        Restrictions.InsensitiveLike(Projections.Property<Application>(r => r.Code), NameOrCodeLike, MatchMode.Anywhere)));

            if (!string.IsNullOrEmpty(NameLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Application>(r => r.Name), NameLike, MatchMode.Anywhere));

            if (!string.IsNullOrEmpty(CodeLike))
                query.Where(Restrictions.InsensitiveLike(Projections.Property<Application>(r => r.Code), CodeLike, MatchMode.Anywhere));

            if (ApplicationTypeId.HasValue)
                query.Where(a => a.ApplicationType.Id == ApplicationTypeId.Value);

            if ((ApplicationTypeIds != null) && ApplicationTypeIds.Any())
                query.WhereRestrictionOn(a => a.ApplicationType.Id).IsIn(ApplicationTypeIds.ToArray());

            if (StatusId.HasValue)
                GetAppInfoQueryOver(query).Where(a => a.Status.Id == StatusId.Value);

            if ((StatusIds != null) && StatusIds.Any())
                GetAppInfoQueryOver(query).WhereRestrictionOn(a => a.Status.Id).IsIn(StatusIds.ToArray());

            if (CriticityId.HasValue)
                GetAppInfoQueryOver(query).Where(a => a.Criticity.Id == CriticityId.Value);

            if ((CriticityIds != null) && CriticityIds.Any())
                GetAppInfoQueryOver(query).WhereRestrictionOn(a => a.Criticity.Id).IsIn(CriticityIds.ToArray());

            if (LinkedTechnologyId.HasValue)
            {
                var allTechnoIds = new[] { LinkedTechnologyId.Value };
                var techno = new TechnologyHierarchicalQuery { RootId = LinkedTechnologyId }.SingleOrDefault(session);
                if (techno != null)
                    allTechnoIds = techno.AllIds.ToArray();

                // Direct Link
                var subAppTechnoLinksIds = session.QueryOver<AppTechnoLink>()
                                                  .WhereRestrictionOn(atl => atl.Technology.Id).IsIn(allTechnoIds)
                                                  .SelectList(list => list.Select(atl => atl.Application.Id))
                                                  .Future<int>();

                // Link through db instance, without duplicates
                var subDbInstanceTechnoLinksQuery = QueryOver.Of<DbInstanceTechnoLink>()
                                                           .WhereRestrictionOn(dtl => dtl.Technology.Id).IsIn(allTechnoIds)
                                                           .SelectList(list => list.Select(dtl => dtl.DbInstance.Id));

                var subAppDbInstanceLinksIds = session.QueryOver<AppDbInstanceLink>()
                                                    .WithSubquery.WhereProperty(adl => adl.DbInstances.Id).In(subDbInstanceTechnoLinksQuery)
                                                    .SelectList(list => list.Select(adl => adl.Application.Id))
                                                    .Future<int>();

                // Link through integrations, without duplicates
                var subIntTechnoLinksQuery = QueryOver.Of<IntegrationTechnoLink>()
                                                      .WhereRestrictionOn(itl => itl.Technology.Id).IsIn(allTechnoIds)
                                                      .SelectList(list => list.Select(adl => adl.Integration.Id));

                IntAppSourceDestResult intAlias = null;
                var subIntAppSourceDestResult = session.QueryOver<Integration>()
                                                       .WithSubquery.WhereProperty(i => i.Id).In(subIntTechnoLinksQuery)
                                                       .SelectList(list => list
                                                           .Select(i => i.AppDest.Id).WithAlias(() => intAlias.AppDestId)
                                                           .Select(i => i.AppSource.Id).WithAlias(() => intAlias.AppSourceId))
                                                       .TransformUsing(Transformers.AliasToBean<IntAppSourceDestResult>())
                                                       .Future<IntAppSourceDestResult>();

                // Link through servers, without duplicates
                var subServerAppQuery = QueryOver.Of<ServerTechnoLink>()
                                                 .WhereRestrictionOn(stl => stl.Technology.Id).IsIn(allTechnoIds)
                                                 .SelectList(list => list.Select(stl => stl.Server.Id));

                var subAppServerLinksIds = session.QueryOver<AppServerLink>()
                                                    .WithSubquery.WhereProperty(asl => asl.Server.Id).In(subServerAppQuery)
                                                    .SelectList(list => list.Select(asl => asl.Application.Id))
                                                    .Future<int>();

                var appIds = subAppTechnoLinksIds.Concat(subIntAppSourceDestResult.Select(x => x.AppSourceId))
                                                 .Concat(subIntAppSourceDestResult.Select(x => x.AppDestId))
                                                 .Concat(subAppServerLinksIds)
                                                 .Concat(subAppDbInstanceLinksIds)
                                                 .Distinct()
                                                 .ToList();

                query.Where(new XmlIn("Id", appIds));
            }

            if (LinkedDbInstanceId.HasValue)
            {
                var subAppDbInstanceLinksQuery = QueryOver.Of<AppDbInstanceLink>()
                                                        .Where(adl => adl.DbInstances.Id == LinkedDbInstanceId.Value)
                                                        .SelectList(list => list.Select(adl => adl.Application.Id));
                query.WithSubquery.WhereProperty(a => a.Id).In(subAppDbInstanceLinksQuery);
            }

            if (LinkedContactId.HasValue && RoleId.HasValue)
            {
                var subAppContactLinkQuery = QueryOver.Of<AppContactLink>()
                                                      .Where(acl => acl.Contact.Id == LinkedContactId.Value)
                                                      .JoinQueryOver(acl => acl.RolesInApp)
                                                      .Where(dv => dv.Id == RoleId.Value)
                                                      .SelectList(list => list.Select(acl => acl.Application.Id));

                query.WithSubquery.WhereProperty(a => a.Id).In(subAppContactLinkQuery);
            }
            else if (LinkedContactId.HasValue)
            {
                var subAppContactLinkQuery = QueryOver.Of<AppContactLink>()
                                                      .Where(acl => acl.Contact.Id == LinkedContactId.Value)
                                                      .SelectList(list => list.Select(acl => acl.Application.Id));

                query.WithSubquery.WhereProperty(a => a.Id).In(subAppContactLinkQuery);
            }

            if (LinkedSectorId.HasValue)
            {
                var allSectorIds = new[] { LinkedSectorId.Value };
                var sector = new SectorHierarchicalQuery { RootId = LinkedSectorId }.SingleOrDefault(session);
                if (sector != null)
                    allSectorIds = sector.AllIds.ToArray();

                GetAppInfoQueryOver(query).WhereRestrictionOn(a => a.Sector.Id).IsIn(allSectorIds.ToArray());
            }

            if (LinkedServerId.HasValue)
            {
                var subAppServerLinkQuery = QueryOver.Of<AppServerLink>()
                                                      .Where(asl => asl.Server.Id == LinkedServerId.Value)
                                                      .SelectList(list => list.Select(asl => asl.Application.Id));
                query.WithSubquery.WhereProperty(a => a.Id).In(subAppServerLinkQuery);
            }

            if (Active.HasValue)
            {
                var activeAppStatuses = ReflexConfiguration.GetCurrent().ActiveAppStatusDVIds;
                if (Active.Value)
                    GetAppInfoQueryOver(query).WhereRestrictionOn(ai => ai.Status.Id).IsIn(activeAppStatuses.ToArray());
                else
                    GetAppInfoQueryOver(query).WhereRestrictionOn(ai => ai.Status.Id).Not.IsIn(activeAppStatuses.ToArray());
            }

            return query;
        }

        protected override bool OrderByCallback(IQueryOver<Application, Application> queryOver, string propertyName, OrderType orderType)
        {
            switch (propertyName)
            {
                case "ApplicationType.Name":
                    queryOver.JoinQueryOver(a => a.ApplicationType).OrderByDomainValue(orderType);
                    return true;
                case "Status.Name":
                    GetAppInfoQueryOver(queryOver).Left.JoinQueryOver(a => a.Status).OrderByDomainValue(orderType);
                    return true;
                case "Criticity.Name":
                    GetAppInfoQueryOver(queryOver).Left.JoinQueryOver(a => a.Criticity).OrderByDomainValue(orderType);
                    return true;
                default:
                    return false;
            }
        }

        private IQueryOver<Application, AppInfo> GetAppInfoQueryOver(IQueryOver<Application, Application> queryOver)
        {
            return _appInfoQueryOver ?? (_appInfoQueryOver = queryOver.JoinQueryOver(a => a.AppInfo));
        }

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class IntAppSourceDestResult
        {
            public int AppSourceId { get; set; }

            public int AppDestId { get; set; }
        }
        //// ReSharper restore UnusedAutoPropertyAccessor.Local
        //// ReSharper restore ClassNeverInstantiated.Local
    }
}