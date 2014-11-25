// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwardAuditEventListener.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Event;

namespace CGI.Reflex.Core.Listeners
{
    [Serializable]
    public class ForwardAuditEventListener : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener
    {
        public void OnPostInsert(PostInsertEvent @event)
        {
            foreach (ForwardAuditAttribute forwardAttribute in @event.Entity.GetType().GetCustomAttributes(typeof(ForwardAuditAttribute), false))
            {
                for (var i = 0; i < @event.State.Length; i++)
                {
                    var propertyName = @event.Persister.PropertyNames[i];

                    if (propertyName == "ConcurrencyVersion")
                        continue;

                    if (propertyName == forwardAttribute.TargetProperty)
                    {
                        var targetEntity = @event.State[i] as BaseEntity;
                        if (targetEntity != null)
                        {
                            var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);

                            var auditInfo = new AuditInfo
                            {
                                EntityType = propertyInfo.PropertyType.FullName,
                                EntityId = targetEntity.Id,
                                Timestamp = DateTime.Now,
                                Action = AuditInfoAction.AddAssociation,
                                User = References.CurrentUser,
                                DisplayName = targetEntity.ToString()
                            };

                            auditInfo.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfo,
                                NewValue = @event.Entity.ToString(),
                                PropertyName = forwardAttribute.TargetPropertyName,
                                PropertyType = @event.Entity.GetType().FullName
                            });

                            FixCollectionProcessedByFlush(@event.Session);
                            var session = @event.Session.GetSession(EntityMode.Poco);
                            session.Save(auditInfo);
                        }
                    }
                }
            }            
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            foreach (ForwardAuditAttribute forwardAttribute in @event.Entity.GetType().GetCustomAttributes(typeof(ForwardAuditAttribute), false))
            {
                for (var i = 0; i < @event.State.Length; i++)
                {
                    var propertyName = @event.Persister.PropertyNames[i];

                    if (propertyName == "ConcurrencyVersion")
                        continue;

                    if (propertyName == forwardAttribute.TargetProperty)
                    {
                        var targetEntity = @event.State[i] as BaseEntity;
                        if (targetEntity != null)
                        {
                            var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);

                            var auditInfo = new AuditInfo
                            {
                                EntityType = propertyInfo.PropertyType.FullName,
                                EntityId = targetEntity.Id,
                                Timestamp = DateTime.Now,
                                Action = AuditInfoAction.UpdateAssociation,
                                User = References.CurrentUser,
                                DisplayName = targetEntity.ToString()
                            };

                            auditInfo.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfo,
                                NewValue = @event.Entity.ToString(),
                                PropertyName = forwardAttribute.TargetPropertyName,
                                PropertyType = @event.Entity.GetType().FullName
                            });

                            FixCollectionProcessedByFlush(@event.Session);
                            var session = @event.Session.GetSession(EntityMode.Poco);
                            session.Save(auditInfo);
                        }
                    }
                }
            }
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            foreach (ForwardAuditAttribute forwardAttribute in @event.Entity.GetType().GetCustomAttributes(typeof(ForwardAuditAttribute), false))
            {
                for (var i = 0; i < @event.DeletedState.Length; i++)
                {
                    var propertyName = @event.Persister.PropertyNames[i];

                    if (propertyName == "ConcurrencyVersion")
                        continue;

                    if (propertyName == forwardAttribute.TargetProperty)
                    {
                        var targetEntity = @event.DeletedState[i] as BaseEntity;
                        if (targetEntity != null)
                        {
                            var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);

                            var auditInfo = new AuditInfo
                            {
                                EntityType = propertyInfo.PropertyType.FullName,
                                EntityId = targetEntity.Id,
                                Timestamp = DateTime.Now,
                                Action = AuditInfoAction.RemoveAssociation,
                                User = References.CurrentUser,
                                DisplayName = targetEntity.ToString()
                            };

                            auditInfo.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfo,
                                OldValue = @event.Entity.ToString(),
                                PropertyName = forwardAttribute.TargetPropertyName,
                                PropertyType = @event.Entity.GetType().FullName
                            });

                            FixCollectionProcessedByFlush(@event.Session);
                            var session = @event.Session.GetSession(EntityMode.Poco);
                            session.Save(auditInfo);
                        }
                    }
                }
            }
        }

        private static void FixCollectionProcessedByFlush(ISessionImplementor sessionImplementor)
        {
            // see http://stackoverflow.com/questions/3090733/an-nhibernate-audit-trail-that-doesnt-cause-collection-was-not-processed-by-fl
            foreach (var collection in sessionImplementor.PersistenceContext.CollectionEntries.Values)
            {
                var collectionEntry = collection as CollectionEntry;
                if (collectionEntry != null)
                    collectionEntry.IsProcessed = true;
            }
        }
    }
}
