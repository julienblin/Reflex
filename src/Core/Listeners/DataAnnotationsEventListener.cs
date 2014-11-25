// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsEventListener.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NHibernate.Event;

namespace CGI.Reflex.Core.Listeners
{
    [Serializable]
    public class DataAnnotationsEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            Validator.ValidateObject(@event.Entity, new ValidationContext(@event.Entity, null, new Dictionary<object, object> { { "NHEvent", true } }), true);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validator.ValidateObject(@event.Entity, new ValidationContext(@event.Entity, null, new Dictionary<object, object> { { "NHEvent", true } }), true);
            return false;
        }
    }
}
