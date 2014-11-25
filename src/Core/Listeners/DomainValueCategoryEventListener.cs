// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainValueCategoryEventListener.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using NHibernate.Event;

namespace CGI.Reflex.Core.Listeners
{
    [Serializable]
    public class DomainValueCategoryEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            CheckDomainValueCategories(@event.Entity);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            CheckDomainValueCategories(@event.Entity);
            return false;
        }

        private static void CheckDomainValueCategories(object entity)
        {
            var domainValueProperties = entity.GetType().GetProperties()
                                              .Where(p => p.PropertyType == typeof(DomainValue));
            foreach (var property in domainValueProperties)
            {
                var domainValueAttr = property.GetCustomAttributes(typeof(DomainValueAttribute), true).Cast<DomainValueAttribute>().SingleOrDefault();
                if (domainValueAttr == null)
                    throw new NotSupportedException(string.Format("Property {0}.{1} is of type DomainValue and is missing a DomainValueAttribute specifying its category.", property.DeclaringType, property.Name));

                var value = property.GetValue(entity, null) as DomainValue;
                if (value == null)
                    return;

                if (value.Category != domainValueAttr.Category)
                    throw new NotSupportedException(string.Format("Property {0}.{1} of {2} has a wrong Category : expected {3}, got {4}.", property.DeclaringType, property.Name, entity, domainValueAttr.Category, value.Category));
            }
        }
    }
}
