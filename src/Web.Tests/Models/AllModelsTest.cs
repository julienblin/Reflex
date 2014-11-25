// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllModelsTest.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using CGI.Reflex.Core.Tests;
using NUnit.Framework;

namespace CGI.Reflex.Web.Tests.Models
{
    [TestFixture]
    public class AllModelsTest
    {
        [Test]
        public void All_display_attributes_should_have_a_corresponding_resource()
        {
            var missingEntries = new List<KeyValuePair<MemberInfo, string>>();
            var propertiesWithDisplayAttributes = GetModels().SelectMany(t => t.GetProperties())
                                                             .Where(p => p.GetCustomAttributes(typeof(DisplayAttribute), true).Count() > 0);
            foreach (var propertyInfo in propertiesWithDisplayAttributes)
            {
                var displayAttr = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().Single();
                try
                {
                    displayAttr.GetName();
                }
                catch (InvalidOperationException)
                {
                    missingEntries.Add(new KeyValuePair<MemberInfo, string>(propertyInfo, displayAttr.Name));
                }
            }

            if (missingEntries.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following properties are missing a correct Display attribute (probably a resource missing):\n{0}",
                        string.Join("\n", missingEntries.Select(p => string.Format("{0}.{1} (Name: {2})", p.Key.ReflectedType.FullName, p.Key.Name, p.Value)))));
            }
        }

        [Test]
        [SetCulture("fr")]
        [SetUICulture("fr")]
        public void All_display_attributes_should_have_a_corresponding_resource_in_fr_culture()
        {
            All_display_attributes_should_have_a_corresponding_resource();
        }

        [Test]
        public void All_validation_attributes_should_have_a_corresponding_resource()
        {
            var missingEntries = new List<KeyValuePair<MemberInfo, string>>();
            var propertiesWithValidationAttributes = GetModels().SelectMany(t => t.GetProperties())
                                                             .Where(p => p.GetCustomAttributes(typeof(ValidationAttribute), true).Count() > 0);
            foreach (var propertyInfo in propertiesWithValidationAttributes)
            {
                var validationAttributes = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>().Where(va => !(va is DataTypeAttribute));
                foreach (var validationAttribute in validationAttributes)
                {
                    if (validationAttribute.ErrorMessageResourceType == null)
                        missingEntries.Add(new KeyValuePair<MemberInfo, string>(propertyInfo, string.Format("{0} is missing a defined ErrorMessageResourceType", validationAttribute.GetType().Name)));
                    
                    if (string.IsNullOrEmpty(validationAttribute.ErrorMessageResourceName))
                        missingEntries.Add(new KeyValuePair<MemberInfo, string>(propertyInfo, string.Format("{0} is missing a defined ErrorMessageResourceName", validationAttribute.GetType().Name)));

                    try
                    {
                        validationAttribute.FormatErrorMessage(Rand.String());
                    }
                    catch (InvalidOperationException ex)
                    {
                        missingEntries.Add(new KeyValuePair<MemberInfo, string>(propertyInfo, ex.Message));
                    }
                }
            }

            if (missingEntries.Count > 0)
            {
                Assert.Fail(
                    string.Format(
                        "The following properties are missing a correct Validation attribute:\n{0}",
                        string.Join("\n", missingEntries.Select(p => string.Format("{0}.{1}:{2}", p.Key.ReflectedType.FullName, p.Key.Name, p.Value)))));
            }
        }

        [Test]
        [SetCulture("fr")]
        [SetUICulture("fr")]
        public void All_validation_attributes_should_have_a_corresponding_resource_in_fr_culture()
        {
            All_validation_attributes_should_have_a_corresponding_resource();
        }

        private static IEnumerable<Type> GetModels()
        {
            return typeof(MvcApplication).Assembly.GetTypes()
                .Where(t =>
                       !string.IsNullOrEmpty(t.Namespace)
                       && t.Namespace.Contains("Model")
                       && !t.IsAbstract
                       && !t.IsInterface
                       && !t.Name.StartsWith("<"));
        }
    }
}
