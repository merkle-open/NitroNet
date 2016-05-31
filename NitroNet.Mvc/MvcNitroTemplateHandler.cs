using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
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
	        RenderingContext context)
	    {
            const string ThisIdentifier = "this";

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

            if (dataVariation.Value.Equals(ThisIdentifier))
            {
                subModel = model;
            }

            var modelFound = false;

            if (subModel == null)
            {
                modelFound = GetValueFromObjectHierarchically(model, propertyName, out subModel);
            }

            if (subModel != null && !(subModel is string))
            {
                var componentIdBySkin = GetComponentId(component.Value, skin.Value);
                RenderPartial(componentIdBySkin, subModel, context);
                return;
            }

            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);
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

            return text.Replace(" ", string.Empty).Replace("-", string.Empty).ToLower();
        }

        //TODO: duplicate function -> remove
        private bool GetValueFromObjectHierarchically(object model, string propertyName, out object modelValue)
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

        //TODO: duplicate function -> remove
        private bool GetValueFromObject(object model, string propertyName, out object modelValue)
        {
            modelValue = null;
            var dataProperty =
                model.GetType().GetProperties().FirstOrDefault(prop => prop.Name.ToLower().Equals(propertyName));
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
            //.RenderAction("Index", component.Value);
            a.RenderPartial(template);
			//throw new NotImplementedException();
		}

    }
}