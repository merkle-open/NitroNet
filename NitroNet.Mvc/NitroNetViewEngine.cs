using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using NitroNet.ViewEngine;
using Veil;
using IView = System.Web.Mvc.IView;
using IViewEngine = System.Web.Mvc.IViewEngine;
using IViewEngineNitro = NitroNet.ViewEngine.IViewEngine;
using IViewNitro = NitroNet.ViewEngine.IView;
using TemplateInfo = NitroNet.ViewEngine.TemplateInfo;

namespace NitroNet.Mvc
{
    public class NitroNetViewEngine : IViewEngine
    {
        private readonly IViewEngineNitro _viewEngine;
        private readonly ITemplateRepository _templateRepository;
        private readonly IModelTypeProvider _modelTypeProvider;
	    private readonly IComponentRepository _componentRepository;

	    public NitroNetViewEngine(IViewEngineNitro viewEngine, ITemplateRepository templateRepository, IModelTypeProvider modelTypeProvider, IComponentRepository componentRepository)
        {
            _viewEngine = viewEngine;
            _templateRepository = templateRepository;
            _modelTypeProvider = modelTypeProvider;
	        _componentRepository = componentRepository;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            // TODO: check if async possible
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), partialViewName).Result;
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            // TODO: check if async possible
            return GetViewResult(controllerContext.RouteData.Values["controller"].ToString(), viewName).Result;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        private async Task<ViewEngineResult> GetViewResult(string controllerName, string viewName)
        {
			TemplateInfo templateInfo = null;
            var componentDefinition = await _componentRepository.GetComponentDefinitionByIdAsync(viewName).ConfigureAwait(false);
            if (componentDefinition == null)
                componentDefinition = await _componentRepository.GetComponentDefinitionByIdAsync(controllerName).ConfigureAwait(false);

			if (componentDefinition != null)
				templateInfo = componentDefinition.DefaultTemplate;

            if (templateInfo == null)
                templateInfo = await _templateRepository.GetTemplateAsync(viewName).ConfigureAwait(false);

            if (templateInfo == null)
                templateInfo = await _templateRepository.GetTemplateAsync(controllerName).ConfigureAwait(false);

            if (templateInfo != null)
            {
                var modelType = await _modelTypeProvider.GetModelTypeFromTemplateAsync(templateInfo).ConfigureAwait(false);
                if (modelType == null)
                    modelType = typeof (object);

                var view = await _viewEngine.CreateViewAsync(templateInfo, modelType).ConfigureAwait(false);
                if (view != null)
					return new ViewEngineResult(new NitroViewAdapter(CreateAdapter(view), ResolveContext), this);
            }

            return new ViewEngineResult(new[] { viewName, controllerName });
        }

	    protected virtual IViewNitro CreateAdapter(IViewNitro view)
	    {
		    return view;
	    }

	    protected virtual MvcRenderingContext ResolveContext(ViewContext viewContext, IViewDataContainer viewDataContainer, TextWriter writer)
	    {
		    return MvcRenderingContext.Build(viewContext, viewDataContainer, writer);
	    }

        private class NitroViewAdapter : IView, IViewDataContainer
        {
            private readonly IViewNitro _adaptee;
			private readonly Func<ViewContext, IViewDataContainer, TextWriter, RenderingContext> _resolveContext;

	        public NitroViewAdapter(IViewNitro adaptee, Func<ViewContext, IViewDataContainer, TextWriter, RenderingContext> resolveContext)
            {
	            _adaptee = adaptee;
	            _resolveContext = resolveContext;
            }

	        public void Render(ViewContext viewContext, TextWriter writer)
            {
                this.ViewData = viewContext.ViewData;
		        var context = _resolveContext(viewContext, this, writer);
	            _adaptee.Render(viewContext.ViewData.Model, context);
            }

	        public ViewDataDictionary ViewData { get; set; }
        }
    }
}
