using System;
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
			return context.Writer.WriteAsync("Placeholder for:" + key);
		}

		public void RenderPlaceholder(object model, string key, string index, RenderingContext context)
		{
			context.Writer.Write("Placeholder for:" + key);
		}

	    public void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
	        RenderingContext context)
	    {
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");

            //todo: get sub model -> and then call renderpartial
	        var propAssigments = _templateHandlerUtils.DoPropertyAssignments(component, skin, dataVariation, model, context);

            if (propAssigments.SubModel != null && !(propAssigments.SubModel is string))
            {
                var componentIdBySkin = _templateHandlerUtils.GetComponentId(component.Value, skin.Value);
                RenderPartial(componentIdBySkin, propAssigments.SubModel, context);
            }

	        _templateHandlerUtils.ThrowExceptionIfPropertyNull(propAssigments.ModelFound,
	            propAssigments.SubModel, propAssigments.PropertyName, model);
        }

        //TODO rework. Currently it doesn't have all features from the normal RenderComponent() method.
        public Task RenderComponentAsync(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model,
            RenderingContext context)
        {            
            var mvcContext = context as MvcRenderingContext;
            if (mvcContext == null)
                throw new InvalidOperationException("MvcNitroTemplateHandler can only be used inside a Mvc application.");

            new HtmlHelper(mvcContext.ViewContext, mvcContext.ViewDataContainer).RenderAction("Index", component.Value);
            return Task.FromResult(false);
        }

		public void RenderLabel(string key, RenderingContext context)
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