using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    public static class ModelStateExtensions
    {
        private static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            var elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            var elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
        }

        private static PropertyInfo IterateToProperty(Type objectType, string errorKey)
        {
            int start = errorKey.IndexOf('[');
            if (start > -1)
            {
                var collectionProperty = objectType.GetProperty(errorKey.Substring(0, start));
                int next = errorKey.IndexOf('.', start);
                return next > -1
                    ? IterateToProperty(collectionProperty.PropertyType.GetEnumeratedType(), errorKey.Substring(next + 1))
                    : collectionProperty;
            }
            start = errorKey.IndexOf('.');
            if (start > -1)
            {
                var property = objectType.GetProperty(errorKey.Substring(0, start));
                return property == null
                    ? null
                    : IterateToProperty(property.PropertyType, errorKey.Substring(start + 1));
            }
            return objectType.GetProperty(errorKey);
        }

        // This property should only be called when the automatic validation fails by a ValidationAttribute or when a deserialization error occurs.
        public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> ToParameterizedModelError(this ModelStateDictionary modelState, ActionDescriptor actionDescriptor, Action<string, string, string, object> propertyErrorAction = null)
        {
            var propertyErrors = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            var model = actionDescriptor.Parameters.FirstOrDefault(p => p.BindingInfo.BindingSource == BindingSource.Body || p.BindingInfo.BindingSource == BindingSource.Form);
            if (model == null) { return propertyErrors; }

            foreach (var error in modelState)
            {
                if (error.Value.Errors.Count == 0) { continue; }

                var errorKey = error.Key;
                if (errorKey.Length > 0 && char.IsLower(errorKey[0]))
                {
                    errorKey = char.ToUpper(errorKey[0]) + errorKey.Substring(1);
                }
                var property = IterateToProperty(model.ParameterType, errorKey);
                if (property != null)
                {
                    var errors = new Dictionary<string, Dictionary<string, object>>();
                    var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), false).ToList();
                    foreach (var validation in error.Value.Errors)
                    {
                        bool found = false;
                        for (int i = 0; i < attributes.Count; ++i)
                        {
                            var valAttribute = (ValidationAttribute)attributes[i];
                            if (valAttribute.ErrorMessage == validation.ErrorMessage)
                            {
                                errors.Add(validation.ErrorMessage, valAttribute.GetType().GetProperties().Where(p => p.DeclaringType == valAttribute.GetType() && p.PropertyType.IsSerializable).ToDictionary(p => p.Name, p => p.GetValue(valAttribute)));
                                attributes.RemoveAt(i);
                                found = true;
                                break;
                            }
                        }
                        // When its not found inside any ValidationAttribute error, it should be a deserialization error, so we mark it as "value" error.
                        if (!found)
                        {
                            errors.Add("value", new Dictionary<string, object>());
                        }
                    }
                    propertyErrors.Add(errorKey, errors);
                }
                else
                {
                    var messages = new Dictionary<string, object>();
                    for (int i = 0; i < error.Value.Errors.Count; ++i)
                    {
                        messages.Add($"messages{i}", error.Value.Errors[i].ErrorMessage);
                    }
                    propertyErrors.Add(errorKey, new Dictionary<string, Dictionary<string, object>>() { { "parse", messages } });
                }
            }

            return propertyErrors;
        }

        // Should not be called because of the custom model errors dictionary used in the AppController, but just in case it is used.
        public static void IterateModelStateErrorParameters(this ModelStateDictionary modelState, ActionDescriptor actionDescriptor, Func<string, string, string, object, Dictionary<string, object>> action)
        {
            if (modelState.IsValid) { return; }

            var model = actionDescriptor.Parameters.FirstOrDefault(p => p.BindingInfo.BindingSource == BindingSource.Body);
            if (model == null) { return; }

            foreach (var error in modelState)
            {
                var property = model.ParameterType.GetProperty(error.Key);
                if (property != null && error.Value.Errors.Count > 0)
                {
                    var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), false).ToList();
                    foreach (var validation in error.Value.Errors)
                    {
                        bool empty = true;
                        for (int i = 0; i < attributes.Count; ++i)
                        {
                            var valAttribute = (ValidationAttribute)attributes[i];
                            if (valAttribute.ErrorMessage == validation.ErrorMessage)
                            {
                                foreach (var attrProp in valAttribute.GetType().GetProperties())
                                {
                                    if (attrProp.DeclaringType == valAttribute.GetType() && attrProp.PropertyType.IsSerializable)
                                    {
                                        action(property.Name, validation.ErrorMessage, attrProp.Name, attrProp.GetValue(valAttribute));
                                        empty = false;
                                    }
                                }
                                attributes.RemoveAt(i);
                                break;
                            }
                        }
                        if (empty)
                        {
                            action(property.Name, validation.ErrorMessage, null, null);
                        }
                    }
                }
            }
        }
    }
}
