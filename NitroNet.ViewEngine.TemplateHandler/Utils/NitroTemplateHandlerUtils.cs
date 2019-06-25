using System;
using System.Collections.Generic;
using System.Linq;
using NitroNet.Common.Exceptions;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.TemplateHandler.Models;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.Utils
{
    public class NitroTemplateHandlerUtils : INitroTemplateHandlerUtils
    {
        private readonly IComponentRepository _componentRepository;
        private readonly INitroNetConfig _config;

        public NitroTemplateHandlerUtils(IComponentRepository componentRepository, INitroNetConfig config)
        {
            _componentRepository = componentRepository;
            _config = config;
        }

        public SubModel FindSubModel(IDictionary<string, RenderingParameter> renderingParameters, object model, RenderingContext context)
        {
            var dataVariation = renderingParameters[ComponentConstants.DataParameter];
            if (string.IsNullOrEmpty(dataVariation.Value))
            {
                dataVariation.Value = renderingParameters[ComponentConstants.Name].Value;
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

        public bool TryCreateModel(SubModel submodel, IDictionary<string, ResolvedAdditionalArgument> additionalArguments, out object model)
        {
            //classic way without using any values from additionalArguments
            if (_config.AdditionalArgumentsParsingMode == AdditionalArgumentsParsingMode.None)
            {
                model = submodel.Value;
                return IsValid(submodel);
            }

            //this means on the parent context was a matching submodel, either by naming convention or passed via data attribute
            if (IsValid(submodel))
            {
                //the values of this submodel are used to build a new model together with the resolved additionalArguments
                if (additionalArguments.Any())
                {
                    model = BuildSubmodelDictionary(submodel.Value, additionalArguments);
                }
                else
                {
                    //if no additional arguments are present just return the submodel
                    model = submodel.Value;
                }

                return true;

            }

            //if no submodel was found use additional parameters, if enabled
            if (_config.EnableAdditionalArgumentsOnly && additionalArguments.Any())
            {
                model = CleanAdditionalArguments(additionalArguments);
                return true;
            }

            model = null;
            return false;
        }

        private IDictionary<string, object> CleanAdditionalArguments(IDictionary<string, ResolvedAdditionalArgument> additionalArguments)
        {
            return additionalArguments.ToDictionary(pair => CleanName(pair.Key), pair => pair.Value.Value);
        }

        /// <summary>
        /// Adds properties from submodel to additional arguments if not already contained in additionalArguments.
        /// </summary>
        /// <param name="submodel"></param>
        /// <param name="additionalArguments"></param>
        /// <returns></returns>
        private IDictionary<string, object> BuildSubmodelDictionary(object submodel, IDictionary<string, ResolvedAdditionalArgument> additionalArguments)
        {
            var cleanedParameters = CleanAdditionalArguments(additionalArguments);
            foreach (var propertyInfo in submodel.GetType().GetProperties())
            {
                if (cleanedParameters.ContainsKey(propertyInfo.Name.ToLowerInvariant()))
                {
                    continue;
                }

                cleanedParameters.Add(propertyInfo.Name.ToLowerInvariant(), propertyInfo.GetValue(submodel));
            }

            return cleanedParameters;
        }

        /// <summary>
        /// Applies additional arguments from dictionary to target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="additionalArguments"></param>
        public void ApplyResolvedArgumentsToObject(object target, IDictionary<string, ResolvedAdditionalArgument> additionalArguments)
        {
            if (_config.AdditionalArgumentsParsingMode == AdditionalArgumentsParsingMode.None)
            {
                return;
            }

            foreach (var resolvedadditionalArgumentsPair in additionalArguments)
            {
                var cleanedKey = CleanName(resolvedadditionalArgumentsPair.Key);
                var subModelProperty = target.GetType().GetProperties().FirstOrDefault(p =>
                    p.Name.Equals(cleanedKey, StringComparison.InvariantCultureIgnoreCase));
                if (subModelProperty == null)
                {
                    throw new NitroNetComponentException(
                        $"Missing property on sub model. Cannot find property with name {resolvedadditionalArgumentsPair.Key} on model {target.GetType()}.");
                }

                if (resolvedadditionalArgumentsPair.Value.Value != null)
                {
                    
                    if (subModelProperty.PropertyType.IsAssignableFrom(resolvedadditionalArgumentsPair.Value.ValueType))
                    {
                        subModelProperty.SetValue(target, resolvedadditionalArgumentsPair.Value.Value);
                        continue;
                    }

                    throw new NitroNetComponentException(
                        $"Type mismatch in model {target.GetType()}. Cannot assign {resolvedadditionalArgumentsPair.Value.ValueType} to property {subModelProperty.Name}.");
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

        public bool IsValid(SubModel subModel)
        {
            return subModel.SubModelFound && subModel.Value != null && !(subModel.Value is string);
        }

        public IDictionary<string, ResolvedAdditionalArgument> ResolveAdditionalArguments(object model, IDictionary<string, string> parameters, ISet<string> reservedKeys)
        {
            if (_config.AdditionalArgumentsParsingMode == AdditionalArgumentsParsingMode.None)
            {
                return new Dictionary<string, ResolvedAdditionalArgument>();
            }

            var dictionary = new Dictionary<string, ResolvedAdditionalArgument>();
            var filteredParameters = parameters.Where(p => !reservedKeys.Contains(p.Key))
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var parameter in filteredParameters)
            {
                // handle string
                if (parameter.Value.IndexOfAny(new[] {'"', '\''}) == 0)
                {
                    dictionary.Add(parameter.Key, new ResolvedAdditionalArgument
                    {
                        Value = parameter.Value.Trim('"', '\''),
                        ValueType = typeof(string)
                    });
                    continue;
                }

                // handle int
                if (int.TryParse(parameter.Value, out var parameterValueAsInt))
                {
                    dictionary.Add(parameter.Key, new ResolvedAdditionalArgument
                    {
                        Value = parameterValueAsInt,
                        ValueType = typeof(int)
                    });
                    continue;
                }

                //handle bool
                if (bool.TryParse(parameter.Value, out var parameterValueAsBool))
                {
                    dictionary.Add(parameter.Key, new ResolvedAdditionalArgument
                    {
                        Value = parameterValueAsBool,
                        ValueType = typeof(bool)
                    });
                    continue;
                }

                //handle null or undefined
                if (parameter.Value.Equals("null") || parameter.Value.Equals("undefined"))
                {
                    dictionary.Add(parameter.Key, new ResolvedAdditionalArgument
                    {
                        Value = null,
                        ValueType = typeof(object)
                    });

                    continue;
                }

                //only try to resolve objects if enabled
                //if resolving hasn't been successful so far it must be an object literal so skip it
                if (_config.AdditionalArgumentsParsingMode == AdditionalArgumentsParsingMode.StaticLiteralsOnly)
                {
                    continue;
                }

                //resolve objects from model
                HandleObject(model, parameter.Key, parameter.Value, dictionary);

            }

            return dictionary;
        }

        private void HandleObject(object model, string parameterKey, string parameterValue,
            IDictionary<string, ResolvedAdditionalArgument> parameterDictionary)
        {
            if (!GetPropertyValueFromObjectHierarchically(model, CleanName(parameterValue), out var value))
            {
                throw new NitroNetComponentException(
                    $"Missing property on model. Cannot find property with name {parameterValue} on model {model.GetType()}.");
            }

            if (value == null)
            {
                parameterDictionary.Add(parameterKey, new ResolvedAdditionalArgument
                {
                    Value = null,
                    ValueType = typeof(object)
                });

                return;
            }

            parameterDictionary.Add(parameterKey, new ResolvedAdditionalArgument
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

            return text.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();
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

        private static bool GetPropertyValueFromObject(object model, string propertyName, out object modelValue)
        {
            modelValue = null;

            if (model is Dictionary<string, object> asDictionary)
            {
                modelValue = asDictionary[propertyName];
                return modelValue != null;
            }

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
}
