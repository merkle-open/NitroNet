using NitroNet.ViewEngine.TemplateHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NitroNet.ViewEngine.TemplateHandler.Utils;

namespace NitroNet.Mvc
{
    using Sitecore.Mvc.Presentation;
    using System.Linq;
    public class MvcNitroTemplateHandler : INitroTemplateHandler
	{
	    private readonly INitroTemplateHandlerUtils _templateHandlerUtils;

	    public MvcNitroTemplateHandler(INitroTemplateHandlerUtils templateHandlerUtils)
	    {
	        _templateHandlerUtils = templateHandlerUtils;
	    }

        public Task RenderPlaceholderAsync(object model, string key, string index, Veil.RenderingContext context)
		{
			return context.Writer.WriteAsync($"Placeholder for: {key}");
		}

		public void RenderPlaceholder(object model, string key, string index, Veil.RenderingContext context)
		{
			context.Writer.Write($"Placeholder for: {key}");
		}

	    public void RenderPlaceholder(object model, string key, string index, TextWriter writer, ViewContext viewContext)
	    {
	        writer.Write("Placeholder for:" + key);
        }

	    public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
	        object model, ViewContext viewContext)
	    {
	        const string thisIdentifier = "this";

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
	            var componentIdBySkin = GetComponentId(component.Value, skin.Value);
	            RenderPartial(componentIdBySkin, model, viewContext);

                return;
	        }
        }

	    //TODO: duplicate function -> remove
        [Obsolete(
            "Deprecated. Use method RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,RenderingContext context, IDictionary<string, string> parameters) instead.")]
        public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
            object model, RenderingContext context)
        {
            RenderComponent(new Dictionary<string, RenderingParameter>
            {
                { ComponentConstants.Name, component },
                { ComponentConstants.DataParameter, dataVariation},
                { ComponentConstants.SkinParameter, skin}
            }, model, context, new Dictionary<string, string>());
        }

        public void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,
            RenderingContext context, IDictionary<string, string> parameters)
        {
            CastRenderingContext(context);

            var component = renderingParameters[ComponentConstants.Name];
            var skin = renderingParameters[ComponentConstants.SkinParameter];

            var subModel = _templateHandlerUtils.FindSubModel(renderingParameters, model, context);
            var additionalParameters = _templateHandlerUtils.ResolveAdditionalArguments(model, parameters, new HashSet<string>(renderingParameters.Keys));

            if (_templateHandlerUtils.TryCreateModel(subModel, additionalParameters, out var finalModel))
            {
                _templateHandlerUtils.RenderPartial(finalModel, component.Value, skin.Value, context, RenderPartial);
                return;
            }

            _templateHandlerUtils.ThrowErrorIfSubModelFoundAndNull(subModel.SubModelFound, subModel.Value,
                subModel.PropertyName, model);
        }

        //TODO: Rework -> Currently this method doesn't have all features from the normal RenderComponent() method.
        public Task RenderComponentAsync(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
            Veil.RenderingContext context)
        {            
            var mvcContext = CastRenderingContext(context);
            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);

            return Task.FromResult(false);
        }

		public void RenderLabel(string key, Veil.RenderingContext context)
		{
			throw new NotImplementedException();
		}

	    public void RenderLabel(string key, ViewContext context)
	    {
	        throw new NotImplementedException();
	    }

		public void RenderPartial(string template, object model, RenderingContext context)
		{
            var mvcContext = CastRenderingContext(context);
            var htmlHelper = new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer);
            
            htmlHelper.RenderPartial(template, model);
		}

        private MvcRenderingContext CastRenderingContext(RenderingContext context)
        {
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
            {
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");
            }

	    public void RenderPartial(string template, object model, ViewContext context)
	    {
	        HtmlHelper a = new HtmlHelper(context, new ViewDataContainer(context.ViewData));
	        a.RenderPartial(template, model);
	    }

            return mvcContext;
        }
    }
}