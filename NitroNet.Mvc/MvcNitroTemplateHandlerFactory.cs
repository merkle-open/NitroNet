using System;
using System.Web;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Mvc
{
    public class MvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly AsyncLocal<HttpContext> _asyncLocal;
        private readonly IComponentRepository _componentRepository;
        private static readonly string SlotName = Guid.NewGuid().ToString("N");

        public MvcNitroTemplateHandlerFactory(AsyncLocal<HttpContext> asyncLocal, IComponentRepository componentRepository)
        {
            _asyncLocal = asyncLocal;
            _componentRepository = componentRepository;
        }

        public INitroTemplateHandler Create()
        {
            INitroTemplateHandler templateHandler;

            if (HttpContext.Current == null)
            {
                HttpContext.Current = _asyncLocal.Value;
            }

            if (HttpContext.Current != null)
            {
                templateHandler = HttpContext.Current.Items[SlotName] as INitroTemplateHandler;
                if (templateHandler != null)
                    return templateHandler;
            }

            templateHandler = new MvcNitroTemplateHandler(_componentRepository);

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[SlotName] = templateHandler;
            }

            return templateHandler;
        }
    }
}
