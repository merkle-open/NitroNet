using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NitroNet.Common.Exceptions;
using NitroNet.ViewEngine.Config;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public class NitroTemplateHandlerUtils : INitroTemplateHandlerUtils
    {
        /// <summary>
        /// Default keys which should not be parsed as additional parameters.
        /// </summary>
        private static readonly HashSet<string> defaultKeys = new HashSet<string> {"name", "template", "data"};

        private readonly IComponentRepository _componentRepository;
        private readonly INitroNetConfig _config;

        public NitroTemplateHandlerUtils(IComponentRepository componentRepository, INitroNetConfig config)
        {
            _componentRepository = componentRepository;
            _config = config;
        }

        public SubModel FindSubModel(RenderingParameter component, RenderingParameter skin,
            RenderingParameter dataVariation, object model, RenderingContext context)
        {
            if (string.IsNullOrEmpty(dataVariation.Value))
            {
                dataVariation.Value = component.Value;
            }

            var propertyName = CleanName(dataVariation.Value);

            object subModel = null;

            if (dataVariation.Value.Equals(ComponentConstants.ThisIdentifier))
            {
                subModel = model;
            }

            var subModelFound = true;

            if (subModel == null)
            {
                subModelFound = GetPropertyValueFromObjectHierarchically(model, propertyName, out subModel);
            }

            return new SubModel
            {
                SubModelFound = subModelFound,
                PropertyName = propertyName,
                Value = subModel
            };
        }

        public void ApplyResolvedParameters(object target, IDictionary<string, ResolvedParameter> parameters)
        {
            if (!_config.EnableLiteralResolving)
            {
                return;
            }

            foreach (var resolvedParameterPair in parameters)
            {
                var cleanedKey = CleanName(resolvedParameterPair.Key);
                var subModelProperty = target.GetType().GetProperties().FirstOrDefault(p =>
                    p.Name.Equals(cleanedKey, StringComparison.InvariantCultureIgnoreCase));
                if (subModelProperty == null)
                {
                    throw new NitroNetComponentException(
                        $"Missing property on sub model. Cannot find property with name {resolvedParameterPair.Key} on model {target.GetType()}.");
                }

                if (resolvedParameterPair.Value.Value != null)
                {
                    
                    if (subModelProperty.PropertyType.IsAssignableFrom(resolvedParameterPair.Value.ValueType))
                    {
                        subModelProperty.SetValue(target, resolvedParameterPair.Value.Value);
                        continue;
                    }

                    throw new NitroNetComponentException(
                        $"Type mismatch in model {target.GetType()}. Cannot assign {resolvedParameterPair.Value.ValueType} to property {subModelProperty.Name}.");
                }

                switch (subModelProperty.PropertyType)
                {
                    case Type type when type == typeof(string):
                        subModelProperty.SetValue(target, string.Empty);
                        break;
                    case Type type when type == typeof(int):
                        subModelProperty.SetValue(target, 0);
                        break;
                    case Type type when type == typeof(bool):
                        subModelProperty.SetValue(target, false);
                        break;
                    default:
                        subModelProperty.SetValue(target, null);
                        break;
                }
            }
        }

        public void RenderPartial(object subModel, string componentValue, string skinValue,
            RenderingContext renderingContext, Action<string, object, RenderingContext> renderPartial)
        {
            var componentIdBySkin = GetComponentId(componentValue, skinValue);
            renderPartial(componentIdBySkin, subModel, renderingContext);
        }

        public IDictionary<string, ResolvedParameter> ResolveAdditionalParameters(object model,
            IDictionary<string, string> parameters)
        {
            if (!_config.EnableLiteralResolving)
            {
                return new Dictionary<string, ResolvedParameter>();
            }

            var dictionary = new Dictionary<string, ResolvedParameter>();
            var filteredParameters = parameters.Where(p => !defaultKeys.Contains(p.Key))
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var parameter in filteredParameters)
            {
                // handle string
                if (parameter.Value.IndexOfAny(new[] {'"', '\''}) == 0)
                {
                    dictionary.Add(parameter.Key, new ResolvedParameter
                    {
                        Value = parameter.Value.Trim('"', '\''),
                        ValueType = typeof(string)
                    });
                    continue;
                }

                // handle int
                if (int.TryParse(parameter.Value, out var parameterValueAsInt))
                {
                    dictionary.Add(parameter.Key, new ResolvedParameter
                    {
                        Value = parameterValueAsInt,
                        ValueType = typeof(int)
                    });
                    continue;
                }

                //handle bool
                if (bool.TryParse(parameter.Value, out var parameterValueAsBool))
                {
                    dictionary.Add(parameter.Key, new ResolvedParameter
                    {
                        Value = parameterValueAsBool,
                        ValueType = typeof(bool)
                    });
                    continue;
                }

                HandleObjectOrNull(model, parameter.Key, parameter.Value, dictionary);

            }

            return dictionary;
        }

        private void HandleObjectOrNull(object model, string parameterKey, string parameterValue,
            IDictionary<string, ResolvedParameter> parameterDictionary)
        {
            if (parameterValue.Equals("null") || parameterValue.Equals("undefined"))
            {
                parameterDictionary.Add(parameterKey, new ResolvedParameter
                {
                    Value = null,
                    ValueType = typeof(object)
                });

                return;
            }

            if (!GetPropertyValueFromObjectHierarchically(model, CleanName(parameterValue), out var value))
            {
                throw new NitroNetComponentException(
                    $"Missing property on model. Cannot find property with name {parameterValue} on model {model.GetType()}.");
            }

            if (value == null)
            {
                parameterDictionary.Add(parameterKey, new ResolvedParameter
                {
                    Value = null,
                    ValueType = typeof(object)
                });

                return;
            }

            parameterDictionary.Add(parameterKey, new ResolvedParameter
            {
                Value = value,
                ValueType = value.GetType()
            });
        }

        public void ThrowErrorIfSubModelFoundAndNull(bool modelFound, object subModel, string propertyName, object model)
        {
            if (modelFound && subModel == null)
            {
                // TODO: Rename and use logging instead of exceptions
                // _log.Error($"Property {propertyName} of model {model.GetType().Name} is null.", this)
                throw new Exception($"Property {propertyName} of model {model.GetType().Name} is null.");
            }
        }

        public string CleanName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Replace(" ", string.Empty).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
        }

        public bool GetPropertyValueFromObjectHierarchically(object model, string propertyName, out object modelValue)
        {
            modelValue = null;
            if (propertyName.IndexOf(".", StringComparison.Ordinal) <= 0)
            {
                return GetPropertyValueFromObject(model, propertyName, out modelValue);
            }

            var subModel = model;
            foreach (var s in propertyName.Split('.'))
            {
                var modelFound = GetPropertyValueFromObject(subModel, s, out subModel);
                if (!modelFound)
                {
                    return false;
                }

                if (subModel == null)
                {
                    break;
                }
            }

            modelValue = subModel;
            return true;
        }

        private bool GetPropertyValueFromObject(object model, string propertyName, out object modelValue)
        {
            modelValue = null;

            var dataProperty = model.GetType().GetProperties()
                .FirstOrDefault(prop => prop.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (dataProperty == null)
            {
                return false;
            }

            modelValue = dataProperty.GetValue(model);
            return true;
        }

        private string GetComponentId(string componentId, string skin)
        {
            // TODO: componentDefinition.DefaultTemplate must not be NULL! -> Fix it
            var componentDefinition = _componentRepository.GetComponentDefinitionByIdAsync(componentId).Result;
            if (componentDefinition != null)
            {
                FileTemplateInfo templateInfo;

                if (string.IsNullOrEmpty(skin) ||
                    componentDefinition.Skins == null ||
                    !componentDefinition.Skins.TryGetValue(skin, out templateInfo))
                {
                    templateInfo = componentDefinition.DefaultTemplate;
                }

                return templateInfo.Id;
            }

            return null;
        }
    }

    public class SubModel
    {
        public bool SubModelFound { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }
    }

    public class ResolvedParameter
    {
        public Type ValueType { get; set; }
        public object Value { get; set; }
    }
}
