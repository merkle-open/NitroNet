using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json.Serialization;
using NitroNet.ViewEngine.TemplateHandler;
using Veil;

namespace NitroNet.Mvc
{
    using System.Linq;

    using NitroNet.ViewEngine;

    public class MvcNitroTemplateHandler : INitroTemplateHandler
	{
        private readonly IComponentRepository _componentRepository;

        public MvcNitroTemplateHandler(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public Task RenderPlaceholderAsync(object model, string key, string index, RenderingContext context)
		{
			return context.Writer.WriteAsync("Placeholder for:" + key);
		}

		public void RenderPlaceholder(object model, string key, string index, RenderingContext context)
		{
			context.Writer.Write("Placeholder for:" + key);
		}

	    public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
	        RenderingContext context, IDictionary<string, string> parameters)
	    {
            const string thisIdentifier = "this";

            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");

            //todo: get sub model -> and then call renderpartial
            if (string.IsNullOrEmpty(dataVariation.Value))
            {
                dataVariation.Value = component.Value;
            }

            var propertyName = CleanName(dataVariation.Value);

            object subModel = null;

            if (dataVariation.Value.Equals(thisIdentifier))
            {
                subModel = model;
            }

	        if (subModel == null)
            {
                GetValueFromObjectHierarchically(model, propertyName, out subModel);
            }

            if (subModel != null && !(subModel is string))
            {
                TrySettingPassedArguments(model, subModel, parameters);

                var componentIdBySkin = GetComponentId(component.Value, skin.Value);
                RenderPartial(componentIdBySkin, subModel, context);
                return;
            }

	        //new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);
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
                var parameterValue = CleanName(parameter.Value);
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

        //TODO: duplicate function -> remove
        private string GetComponentId(string componentId, string skin)
        {
            //TODO: componentDefinition.DefaultTemplate must not be NULL!!!! -> fix it
            var componentDefinition = _componentRepository.GetComponentDefinitionByIdAsync(componentId).Result;
            if (componentDefinition != null)
            {
                FileTemplateInfo templateInfo;
                if (string.IsNullOrEmpty(skin) || componentDefinition.Skins == null ||
                    !componentDefinition.Skins.TryGetValue(skin, out templateInfo))
                    templateInfo = componentDefinition.DefaultTemplate;

                return templateInfo.Id;
            }

            return null;
        }

        //TODO: duplicate function -> remove
        private string CleanName(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Replace(" ", string.Empty).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
        }

        //TODO: duplicate function -> remove
        private bool GetValueFromObjectHierarchically(object model, string propertyName, out object modelValue)
        {
            modelValue = null;
            //TODO: Check if this if-clause is still needed. I think there are no situations like this.
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

        //TODO: duplicate function -> remove
        private bool GetValueFromObject(object model, string propertyName, out object modelValue)
        {
            modelValue = null;
            var dataProperty =
                model.GetType().GetProperties().FirstOrDefault(prop => prop.Name.ToLower(CultureInfo.InvariantCulture).Equals(propertyName));
            if (dataProperty == null)
            {
                return false;
            }

            modelValue = dataProperty.GetValue(model);
            return true;
        }

        public Task RenderComponentAsync(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
            RenderingContext context)
        {            
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");

            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);
            return Task.FromResult(false);
        }

		public Task RenderLabelAsync(string key, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public void RenderLabel(string key, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public Task RenderPartialAsync(string template, object model, RenderingContext context)
		{
			throw new NotImplementedException();
		}

		public void RenderPartial(string template, object model, RenderingContext context)
		{
            //todo: implemnt! htmlhelper.partial
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");

            HtmlHelper a = new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer);
            a.RenderPartial(template, model);
		}

    }
}