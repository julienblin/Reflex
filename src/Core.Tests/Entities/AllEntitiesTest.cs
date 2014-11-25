// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllEntitiesTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using CGI.Reflex.Core.Attributes;
using CGI.Reflex.Core.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CGI.Reflex.Core.Tests.Entities
{
    [TestFixture]
    public class AllEntitiesTest
    {
        [Test]
        public void All_properties_should_have_a_display_attribute()
        {
            var propertiesWithoutDisplay = new List<MemberInfo>();

            foreach (var entity in GetEntities())
            {
                propertiesWithoutDisplay.AddRange(
                    GetProperties(entity).Union(GetEnumMemberInfo(entity)).Where(p =>
                        p.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().SingleOrDefault() == null));
            }

            if (propertiesWithoutDisplay.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following properties are missing a Display attribute:\n{0}",
                        string.Join("\n", propertiesWithoutDisplay.Select(p => string.Format("{0}.{1}", p.ReflectedType.Name, p.Name)))));
            }
        }

        [Test]
        public void All_display_attribute_should_have_a_valid_get_name()
        {
            var missingEntries = new List<KeyValuePair<MemberInfo, string>>();

            foreach (var entity in GetEntities())
            {
                foreach (var property in GetProperties(entity).Union(GetEnumMemberInfo(entity)))
                {
                    var displayAttr =
                        property.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().SingleOrDefault();
                    if (displayAttr != null)
                    {
                        try
                        {
                            displayAttr.GetName();
                        }
                        catch (InvalidOperationException)
                        {
                            missingEntries.Add(new KeyValuePair<MemberInfo, string>(property, displayAttr.Name));
                        }
                    }
                }
            }

            if (missingEntries.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following properties are missing a correct Display attribute (probably a resource missing):\n{0}",
                        string.Join("\n", missingEntries.Select(p => string.Format("{0}.{1} (Name: {2})", p.Key.ReflectedType.Name, p.Key.Name, p.Value)))));
            }
        }

        [Test]
        [SetCulture("fr")]
        [SetUICulture("fr")]
        public void All_display_attribute_should_a_corresponding_entry_in_fr_culture()
        {
            All_display_attribute_should_a_corresponding_entry_in_current_culture();
        }

        public void All_display_attribute_should_a_corresponding_entry_in_current_culture()
        {
            var missingEntries = new List<KeyValuePair<MemberInfo, string>>();

            foreach (var entity in GetEntities())
            {
                foreach (var property in GetProperties(entity).Union(GetEnumMemberInfo(entity)))
                {
                    var displayAttr =
                        property.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().SingleOrDefault();
                    if (displayAttr != null)
                    {
                        if (displayAttr.ResourceType == null)
                        {
                            missingEntries.Add(new KeyValuePair<MemberInfo, string>(property, displayAttr.Name));
                        }
                        else
                        {
                            var resourceManager = new ResourceManager(displayAttr.ResourceType);
                            var resourcesSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                            if (!resourcesSet.Cast<DictionaryEntry>().Any(de => (string)de.Key == displayAttr.Name))
                                missingEntries.Add(new KeyValuePair<MemberInfo, string>(property, displayAttr.Name));
                        }
                    }
                }
            }

            if (missingEntries.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following properties are missing a valid name with '{0}' culture (probably a resource missing):\n{1}",
                        CultureInfo.CurrentUICulture,
                        string.Join("\n", missingEntries.Select(p => string.Format("{0}.{1} (Name: {2})", p.Key.ReflectedType.Name, p.Key.Name, p.Value)))));
            }
        }

        [Test]
        public void All_auditable_attribute_should_have_a_valid_get_name()
        {
            var missingTypes = new List<Type>();

            foreach (var entity in GetEntities())
            {
                var auditableAttribute = entity.GetCustomAttributes(typeof(AuditableAttribute), true).Cast<AuditableAttribute>().SingleOrDefault();
                if (auditableAttribute != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(auditableAttribute.GetName()))
                            missingTypes.Add(entity);
                    }
                    catch (InvalidOperationException)
                    {
                        missingTypes.Add(entity);
                    }
                }
            }

            if (missingTypes.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following types with Auditable attribute are missing a valid Name (probably a resource missing):\n{0}",
                        string.Join("\n", missingTypes.Select(m => m.Name))));
            }
        }

        [Test]
        [SetCulture("fr")]
        [SetUICulture("fr")]
        public void All_auditable_attribute_should_have_a_corresponding_entry_in_fr_culture()
        {
            All_auditable_attribute_should_have_a_corresponding_entry_in_current_culture();
        }

        public void All_auditable_attribute_should_have_a_corresponding_entry_in_current_culture()
        {
            var missingTypes = new List<Type>();

            foreach (var entity in GetEntities())
            {
                var auditableAttribute = entity.GetCustomAttributes(typeof(AuditableAttribute), true).Cast<AuditableAttribute>().SingleOrDefault();
                if (auditableAttribute != null)
                {
                    if (auditableAttribute.ResourceType == null)
                    {
                        missingTypes.Add(entity);
                    }
                    else
                    {
                        var resourceManager = new ResourceManager(auditableAttribute.ResourceType);
                        var resourcesSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                        if (!resourcesSet.Cast<DictionaryEntry>().Any(de => (string)de.Key == auditableAttribute.Name))
                            missingTypes.Add(entity);
                    }
                }
            }

            if (missingTypes.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following types with Auditable attribute are missing a valid Name with '{0}' culture(probably a resource missing):\n{1}",
                        CultureInfo.CurrentUICulture,
                        string.Join("\n", missingTypes.Select(m => m.Name))));
            }
        }

        [Test]
        public void All_entities_should_override_ToString()
        {
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                var instance = Activator.CreateInstance(entity);
                instance.ToString().Should().NotBe(instance.GetType().FullName);
            }
        }

        private static IEnumerable<Type> GetEntities()
        {
            return typeof(Role).Assembly.GetTypes()
                .Where(t =>
                       t.Namespace == typeof(Role).Namespace
                       && !t.IsAbstract
                       && !t.IsInterface
                       && !t.Name.StartsWith("<")
                       && t != typeof(CoreResources)
                       && t != typeof(UserAuthentication)
                       && t != typeof(LogEntry));
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type entity)
        {
            return entity.GetProperties()
                .Where(p =>
                       p.Name != "Id"
                    && p.Name != "LastUpdatedAtUTC"
                    && p.Name != "ConcurrencyVersion"
                    && p.Name != "AllIds"
                    && p.Name != "AllParentIds"
                    && p.Name != "HierarchicalLevel");
        }

        private static IEnumerable<MemberInfo> GetEnumMemberInfo(Type entity)
        {
            if (!entity.IsEnum)
                return Enumerable.Empty<MemberInfo>();

            var enumValues = Enum.GetNames(entity);
            return entity.GetMembers()
                .Where(m => enumValues.Contains(m.Name));
        }
    }
}
