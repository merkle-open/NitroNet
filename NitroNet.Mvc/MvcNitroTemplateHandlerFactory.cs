using NitroNet.ViewEngine.TemplateHandler.RenderHandler;

namespace NitroNet.Mvc
{
    public class MvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly INitroTemplateHandlerUtils _templateHandlerUtils;

        public MvcNitroTemplateHandlerFactory(INitroTemplateHandlerUtils templateHandlerUtils)
        {
            _templateHandlerUtils = templateHandlerUtils;
        }

        public INitroTemplateHandler Create()
        {
            return new MvcNitroTemplateHandler(_templateHandlerUtils);
        }
    }
}