using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;
using Veil;

namespace NitroNet.Mvc
{
    public class MvcNitroTemplateHandler : INitroTemplateHandler
	{
	    private readonly INitroTemplateHandlerUtils _templateHandlerUtils;

	    public MvcNitroTemplateHandler(INitroTemplateHandlerUtils templateHandlerUtils)
	    {
	        _templateHandlerUtils = templateHandlerUtils;
	    }

	    public Task RenderPlaceholderAsync(object model, string key, string index, RenderingContext context)
		{
			return context.Writer.WriteAsync($"Placeholder for: {key}");
		}

		public void RenderPlaceholder(object model, string key, string index, RenderingContext context)
		{
			context.Writer.Write($"Placeholder for: {key}");
		}

        public void RenderComponent(RenderingParameter component, RenderingParameter skin,
            RenderingParameter dataVariation, object model,
            RenderingContext context, IDictionary<string, string> parameters)
        {
            CastRenderingContext(context);

            var subModel = _templateHandlerUtils.FindSubModel(component, skin, dataVariation, model, context);
            var additionalParameters = _templateHandlerUtils.ResolveAdditionalParameters(model, parameters);

            if (subModel.SubModelFound && subModel.Value != null && !(subModel.Value is string))
            {
                _templateHandlerUtils.ApplyResolvedParameters(subModel.Value, additionalParameters);
                _templateHandlerUtils.RenderPartial(subModel.Value, component.Value, skin.Value, context,
                    RenderPartial);
                return;
            }

            _templateHandlerUtils.ThrowErrorIfSubModelFoundAndNull(subModel.SubModelFound, subModel.Value,
                subModel.PropertyName, model);
        }

        //TODO: Rework -> Currently this method doesn't have all features from the normal RenderComponent() method.
        public Task RenderComponentAsync(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
            RenderingContext context)
        {            
            var mvcContext = CastRenderingContext(context);
            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);

            return Task.FromResult(false);
        }

		public void RenderLabel(string key, RenderingContext context)
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

            return mvcContext;
        }
    }
}