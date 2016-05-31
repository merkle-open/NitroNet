using System;
using System.Web;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Mvc
{
    public class MvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly AsyncLocal<HttpContext> _asyncLocal;
        private static readonly string SlotName = Guid.NewGuid().ToString("N");

        public MvcNitroTemplateHandlerFactory(AsyncLocal<HttpContext> asyncLocal)
        {
            _asyncLocal = asyncLocal;
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

            templateHandler = new MvcNitroTemplateHandler();

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[SlotName] = templateHandler;
            }

            return templateHandler;
        }
    }
}
