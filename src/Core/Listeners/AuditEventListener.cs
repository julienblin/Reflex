// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditEventListener.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class AuditEventListener : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener
    {
        public void OnPostInsert(PostInsertEvent @event)
        {
            if (@event.Entity.GetType().GetCustomAttributes(typeof(AuditableAttribute), false).Any())
            {
                var auditInfo = new AuditInfo
                                    {
                                        EntityType = @event.Entity.GetType().FullName,
                                        EntityId = Convert.ToInt32(@event.Id),
                                        Timestamp = DateTime.Now,
                                        Action = AuditInfoAction.Create,
                                        User = References.CurrentUser,
                                        DisplayName = @event.Entity.ToString()
                                    };

                var optimisticConcurrency = @event.Entity as IOptimisticConcurrency;
                if (optimisticConcurrency != null)
                    auditInfo.ConcurrencyVersion = optimisticConcurrency.ConcurrencyVersion;

                for (var i = 0; i < @event.State.Length; i++)
                {
                    if (@event.State[i] != null)
                    {
                        var propertyName = @event.Persister.PropertyNames[i];

                        if ((optimisticConcurrency != null) && (propertyName == "ConcurrencyVersion"))
                            continue;

                        var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);
                        if (propertyInfo.GetCustomAttributes(typeof(NotAuditableAttribute), true)
                                        .Cast<NotAuditableAttribute>()
                                        .Any())
                            continue;

                        var auditableCollectionRef = propertyInfo.GetCustomAttributes(typeof(AuditableCollectionReferenceAttribute), true)
                                        .Cast<AuditableCollectionReferenceAttribute>()
                                        .SingleOrDefault();
                        if (auditableCollectionRef != null)
                        {
                            var targetEntity = @event.State[i] as BaseEntity;
                            if (targetEntity != null)
                            {
                                var auditInfoRef = new AuditInfo
                                {
                                    EntityType = propertyInfo.PropertyType.FullName,
                                    EntityId = targetEntity.Id,
                                    Timestamp = DateTime.Now,
                                    Action = AuditInfoAction.AddAssociation,
                                    User = References.CurrentUser,
                                    DisplayName = targetEntity.ToString()
                                };
                                auditInfoRef.Add(new AuditInfoProperty
                                {
                                    AuditInfo = auditInfoRef,
                                    NewValue = @event.Entity.ToString(),
                                    PropertyName = auditableCollectionRef.PropertyName,
                                    PropertyType = @event.Entity.GetType().FullName
                                });

                                FixCollectionProcessedByFlush(@event.Session);
                                var session = @event.Session.GetSession(EntityMode.Poco);
                                session.Save(auditInfoRef);
                            }

                            if (!auditableCollectionRef.LogToCurrentEntity)
                                continue;
                        }

                        string value;

                        if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType)
                         && propertyInfo.PropertyType != typeof(string))
                        {
                            var valueBuilder = new StringBuilder();
                            foreach (var element in (IEnumerable)@event.State[i])
                            {
                                if (element != null)
                                    valueBuilder.AppendLine(string.Format("+ {0}", element));
                            }

                            value = valueBuilder.ToString();
                        }
                        else
                        {
                            value = FormatValue(@event.Entity.GetType(), propertyName, @event.State[i].ToString());
                        }

                        if (!string.IsNullOrEmpty(value))
                            auditInfo.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfo,
                                PropertyName = propertyName,
                                PropertyType = propertyInfo.PropertyType.FullName,
                                NewValue = value
                            });
                    }
                }

                if (auditInfo.Properties.Any())
                {
                    FixCollectionProcessedByFlush(@event.Session);
                    var session = @event.Session.GetSession(EntityMode.Poco);
                    session.Save(auditInfo);
                }
            }
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            if (@event.Entity.GetType().GetCustomAttributes(typeof(AuditableAttribute), false).Any())
            {
                var auditInfo = new AuditInfo
                {
                    EntityType = @event.Entity.GetType().FullName,
                    EntityId = Convert.ToInt32(@event.Id),
                    Timestamp = DateTime.Now,
                    Action = AuditInfoAction.Update,
                    User = References.CurrentUser,
                    DisplayName = @event.Entity.ToString()
                };

                var optimisticConcurrency = @event.Entity as IOptimisticConcurrency;
                if (optimisticConcurrency != null)
                    auditInfo.ConcurrencyVersion = optimisticConcurrency.ConcurrencyVersion;

                for (var i = 0; i < @event.OldState.Length; i++)
                {
                    var propertyName = @event.Persister.PropertyNames[i];

                    if ((optimisticConcurrency != null) && (propertyName == "ConcurrencyVersion"))
                        continue;

                    var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);
                    if (propertyInfo.GetCustomAttributes(typeof(NotAuditableAttribute), true)
                                    .Cast<NotAuditableAttribute>()
                                    .Any())
                        continue;

                    if (Equals(@event.OldState[i], @event.State[i]))
                        continue;

                    var auditableCollectionRef = propertyInfo.GetCustomAttributes(typeof(AuditableCollectionReferenceAttribute), true)
                                        .Cast<AuditableCollectionReferenceAttribute>()
                                        .SingleOrDefault();
                    if (auditableCollectionRef != null)
                    {
                        var addedEntity = @event.State[i] as BaseEntity;
                        if (addedEntity != null)
                        {
                            var auditInfoRef = new AuditInfo
                            {
                                EntityType = propertyInfo.PropertyType.FullName,
                                EntityId = addedEntity.Id,
                                Timestamp = DateTime.Now,
                                Action = AuditInfoAction.AddAssociation,
                                User = References.CurrentUser,
                                DisplayName = addedEntity.ToString()
                            };
                            auditInfoRef.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfoRef,
                                NewValue = @event.Entity.ToString(),
                                PropertyName = auditableCollectionRef.PropertyName,
                                PropertyType = @event.Entity.GetType().FullName
                            });

                            FixCollectionProcessedByFlush(@event.Session);
                            var session = @event.Session.GetSession(EntityMode.Poco);
                            session.Save(auditInfoRef);
                        }

                        var removedEntity = @event.OldState[i] as BaseEntity;
                        if (removedEntity != null)
                        {
                            var auditInfoRef = new AuditInfo
                            {
                                EntityType = propertyInfo.PropertyType.FullName,
                                EntityId = removedEntity.Id,
                                Timestamp = DateTime.Now,
                                Action = AuditInfoAction.RemoveAssociation,
                                User = References.CurrentUser,
                                DisplayName = removedEntity.ToString()
                            };
                            auditInfoRef.Add(new AuditInfoProperty
                            {
                                AuditInfo = auditInfoRef,
                                OldValue = @event.Entity.ToString(),
                                PropertyName = auditableCollectionRef.PropertyName,
                                PropertyType = @event.Entity.GetType().FullName
                            });

                            var session = @event.Session.GetSession(EntityMode.Poco);
                            session.Save(auditInfoRef);
                        }

                        if (!auditableCollectionRef.LogToCurrentEntity)
                            continue;
                    }

                    string oldValue = null;
                    string newValue = null;

                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType)
                         && propertyInfo.PropertyType != typeof(string))
                    {
                        var oldValueList = ((IEnumerable)@event.OldState[i]).Cast<object>().Where(o => o != null).Select(o => o.ToString()).Distinct().OrderBy(s => s).ToList();
                        var newValueList = ((IEnumerable)@event.State[i]).Cast<object>().Where(o => o != null).Select(o => o.ToString()).Distinct().OrderBy(s => s).ToList();

                        var removed = oldValueList.Except(newValueList);
                        var added = newValueList.Except(oldValueList);

                        if (removed.Any())
                            newValue = string.Join(Environment.NewLine, removed.Select(s => string.Format("- {0}", s)));
                        if (added.Any() && removed.Any())
                            newValue += Environment.NewLine;
                        if (added.Any())
                            newValue = string.Join(Environment.NewLine, added.Select(s => string.Format("+ {0}", s)));
                    }
                    else
                    {
                        oldValue = @event.OldState[i] != null ? FormatValue(@event.Entity.GetType(), propertyName, @event.OldState[i].ToString()) : null;
                        newValue = @event.State[i] != null ? FormatValue(@event.Entity.GetType(), propertyName, @event.State[i].ToString()) : null;
                    }

                    if (!string.IsNullOrEmpty(oldValue) || !string.IsNullOrEmpty(newValue))
                        auditInfo.Add(new AuditInfoProperty
                        {
                            AuditInfo = auditInfo,
                            PropertyName = propertyName,
                            PropertyType = propertyInfo.PropertyType.FullName,
                            OldValue = oldValue,
                            NewValue = newValue
                        });
                }

                if (auditInfo.Properties.Any())
                {
                    FixCollectionProcessedByFlush(@event.Session);
                    var session = @event.Session.GetSession(EntityMode.Poco);
                    session.Save(auditInfo);
                }
            }
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            if (@event.Entity.GetType().GetCustomAttributes(typeof(AuditableAttribute), false).Any())
            {
                var auditInfo = new AuditInfo
                {
                    EntityType = @event.Entity.GetType().FullName,
                    EntityId = Convert.ToInt32(@event.Id),
                    Timestamp = DateTime.Now,
                    Action = AuditInfoAction.Delete,
                    User = References.CurrentUser,
                    DisplayName = @event.Entity.ToString()
                };

                var optimisticConcurrency = @event.Entity as IOptimisticConcurrency;
                if (optimisticConcurrency != null)
                    auditInfo.ConcurrencyVersion = optimisticConcurrency.ConcurrencyVersion;

                for (var i = 0; i < @event.DeletedState.Length; i++)
                {
                    if (@event.DeletedState[i] != null)
                    {
                        var propertyName = @event.Persister.PropertyNames[i];

                        if ((optimisticConcurrency != null) && (propertyName == "ConcurrencyVersion"))
                            continue;

                        var propertyInfo = @event.Entity.GetType().GetProperty(propertyName);
                        if (propertyInfo.GetCustomAttributes(typeof(NotAuditableAttribute), true)
                                        .Cast<NotAuditableAttribute>()
                                        .Any())
                            continue;

                        var auditableCollectionRef = propertyInfo.GetCustomAttributes(typeof(AuditableCollectionReferenceAttribute), true)
                                        .Cast<AuditableCollectionReferenceAttribute>()
                                        .SingleOrDefault();
                        if (auditableCollectionRef != null)
                        {
                            var targetEntity = @event.DeletedState[i] as BaseEntity;
                            if (targetEntity != null)
                            {
                                var auditInfoRef = new AuditInfo
                                {
                                    EntityType = propertyInfo.PropertyType.FullName,
                                    EntityId = targetEntity.Id,
                                    Timestamp = DateTime.Now,
                                    Action = AuditInfoAction.RemoveAssociation,
                                    User = References.CurrentUser,
                                    DisplayName = targetEntity.ToString()
                                };
                                auditInfoRef.Add(new AuditInfoProperty
                                {
                                    AuditInfo = auditInfoRef,
                                    OldValue = @event.Entity.ToString(),
                                    PropertyName = auditableCollectionRef.PropertyName,
                                    PropertyType = @event.Entity.GetType().FullName
                                });

                                FixCollectionProcessedByFlush(@event.Session);
                                var session = @event.Session.GetSession(EntityMode.Poco);
                                session.Save(auditInfoRef);
                            }

                            if (!auditableCollectionRef.LogToCurrentEntity)
                                continue;
                        }

                        if (@event.DeletedState[i] != null && NHibernateUtil.IsInitialized(@event.DeletedState[i]))
                        {
                            string value;
                            if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType)
                             && propertyInfo.PropertyType != typeof(string))
                            {
                                var valueBuilder = new StringBuilder();
                                foreach (var element in (IEnumerable)@event.DeletedState[i])
                                {
                                    if (element != null)
                                        valueBuilder.AppendLine(element.ToString());
                                }

                                value = valueBuilder.ToString();
                            }
                            else
                            {
                                value = FormatValue(@event.Entity.GetType(), propertyName, @event.DeletedState[i] != null ? @event.DeletedState[i].ToString() : null);
                            }

                            if (!string.IsNullOrEmpty(value))
                            {
                                auditInfo.Add(new AuditInfoProperty
                                {
                                    AuditInfo = auditInfo,
                                    PropertyName = propertyName,
                                    PropertyType = propertyInfo.PropertyType.FullName,
                                    OldValue = value
                                });
                            }
                        }
                    }
                }

                if (auditInfo.Properties.Any())
                {
                    FixCollectionProcessedByFlush(@event.Session);
                    var session = @event.Session.GetSession(EntityMode.Poco);
                    session.Save(auditInfo);
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

        private static string FormatValue(Type entityType, string propertyName, string value)
        {
            // Format Date to yyyy-MM-dd
            if (entityType.GetProperty(propertyName)
                .GetCustomAttributes(typeof(DataTypeAttribute), true)
                .Cast<DataTypeAttribute>().Any(dt => dt.DataType == DataType.Date))
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                    return date.ToString("yyyy-MM-dd");
            }

            return value;
        }
    }
}