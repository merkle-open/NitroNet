using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public class NitroTemplateHandlerUtils : INitroTemplateHandlerUtils
    {
        private readonly IComponentRepository _componentRepository;

        public NitroTemplateHandlerUtils(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
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

            var subModelFound = false;

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

        public bool TryRenderPartial(object model, object subModel, string componentValue,
            string skinValue, RenderingContext renderingContext, IDictionary<string, string> parameters, 
            Action<string, object, RenderingContext> renderPartial)
        {
            if (subModel != null && !(subModel is string))
            {
                TrySettingPassedArguments(model, subModel, parameters);
                var componentIdBySkin = GetComponentId(componentValue, skinValue);
                renderPartial(componentIdBySkin, subModel, renderingContext);

                return true;
            }

            return false;
        }

        private void TrySettingPassedArguments(object model, object subModel, IDictionary<string, string> parameters)
	    {
	        /*
             1.) Search for a property in the subModel with the key of a parameter (key maybe has to be cleaned beforehand)
             2.) If found, try to assign this parameter value to the property of the subModel
               2.1) Now you have to distinguish between strings and properties of parent objects
                 2.1.1) If it is a string, you can just assign it
                 2.1.2) If it is a property of a parent object, you have to search for it and pass it on
             3.) Also implement ExceptionHandling and make it as robust as possible
             4.) Do refactorings
            */

            var defaultKeys = new HashSet<string> {"name", "template", "data"};
	        var filteredParameters = parameters.Where(p => !defaultKeys.Contains(p.Key))
	            .ToDictionary(p => p.Key, p => p.Value);

            foreach (var parameter in filteredParameters)
            {
                var parameterValue = parameter.Value;
                var subModelProperties = subModel.GetType().GetProperties();
                var subModelProperty = subModelProperties.FirstOrDefault(p => p.Name.ToLower().Equals(parameter.Key, StringComparison.InvariantCultureIgnoreCase));

                if (parameterValue.StartsWith("\"") || parameterValue.StartsWith("'"))
                {                   
                    parameterValue = parameterValue.Trim('"', '\'');

                    subModelProperty?.SetValue(subModel, parameterValue);
                }
                else
                {
                    var propertyHierarchy = parameterValue.Split('.');

                    PropertyInfo modelProperty = null;
                    object subPropertyValue = model;

                    for (int i = 0; i < propertyHierarchy.Length; i++)
                    {
                        PropertyInfo[] modelProperties;
                        var propName = propertyHierarchy.ElementAt(i);

                        if (i == 0)
                        {
                            modelProperties = model.GetType().GetProperties();
                        }
                        else
                        {
                            subPropertyValue = modelProperty?.GetValue(model);
                            modelProperties = subPropertyValue?.GetType().GetProperties();
                        }
                        modelProperty = modelProperties?.FirstOrDefault(p => p.Name.ToLower(CultureInfo.InvariantCulture).Equals(propName));

                        if (i == propertyHierarchy.Length - 1)
                        {
                            subModelProperty?.SetValue(subModel, modelProperty?.GetValue(subPropertyValue));
                        }
                    }
                }
            }
        }

        public void LogErrorIfSubModelFoundAndNull(bool modelFound, object subModel, string propertyName, object model)
        {
            if (modelFound && subModel == null)
            {
                // TODO: Use logging instead of exceptions
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
                .FirstOrDefault(prop => prop.Name.ToLower(CultureInfo.InvariantCulture).Equals(propertyName));
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

                if (string.IsNullOrEmpty(skin)|| 
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
}
