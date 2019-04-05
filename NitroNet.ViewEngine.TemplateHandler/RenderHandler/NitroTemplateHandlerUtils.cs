using System;
using System.Globalization;
using System.Linq;
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

        public PropertyAssignments DoPropertyAssignments(RenderingParameter component, RenderingParameter skin,
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

            var modelFound = false;

            if (subModel == null)
            {
                modelFound = GetValueFromObjectHierarchically(model, propertyName, out subModel);
            }

            return new PropertyAssignments
            {
                ModelFound = modelFound,
                PropertyName = propertyName,
                SubModel = subModel
            };
        }

        public bool TryRenderPartial(object subModel, string componentValue, string skinValue,
            RenderingContext renderingContext, Action<string, object, RenderingContext> renderPartial)
        {
            if (subModel != null && !(subModel is string))
            {
                var componentIdBySkin = GetComponentId(componentValue, skinValue);
                renderPartial(componentIdBySkin, subModel, renderingContext);

                return true;
            }

            return false;
        }

        public void LogErrorIfPropertyNull(bool modelFound, object subModel, string propertyName, object model)
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

        public bool GetValueFromObjectHierarchically(object model, string propertyName, out object modelValue)
        {
            modelValue = null;
            if (propertyName.IndexOf(".", StringComparison.Ordinal) <= 0)
            {
                return GetValueFromObject(model, propertyName, out modelValue);
            }

            var subModel = model;
            foreach (var s in propertyName.Split('.'))
            {
                var modelFound = GetValueFromObject(subModel, s, out subModel);
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

        private bool GetValueFromObject(object model, string propertyName, out object modelValue)
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

    public class PropertyAssignments
    {
        public bool ModelFound { get; set; }
        public string PropertyName { get; set; }
        public object SubModel { get; set; }
    }
}
