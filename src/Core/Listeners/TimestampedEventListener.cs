// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampedEventListener.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Entities;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace CGI.Reflex.Core.Listeners
{
    [Serializable]
    public class TimestampedEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var timestamped = @event.Entity as ITimestamped;
            if (timestamped == null)
                return false;

            var time = DateTime.UtcNow;
            Set(@event.Persister, @event.State, "LastUpdatedAtUTC", time);
            timestamped.SetLastUpdatedAtUTC(time);

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var timestamped = @event.Entity as ITimestamped;
            if (timestamped == null)
                return false;

            var time = DateTime.UtcNow;
            Set(@event.Persister, @event.State, "LastUpdatedAtUTC", time);
            timestamped.SetLastUpdatedAtUTC(time);

            return false;
        }

        private static void Set(IEntityPersister persister, IList<object> state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}
