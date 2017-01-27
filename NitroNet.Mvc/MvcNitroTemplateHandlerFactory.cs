using NitroNet.ViewEngine;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.Mvc
{
    public class MvcNitroTemplateHandlerFactory : INitroTemplateHandlerFactory
    {
        private readonly IComponentRepository _componentRepository;

        public MvcNitroTemplateHandlerFactory(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public INitroTemplateHandler Create()
        {
            return new MvcNitroTemplateHandler(_componentRepository);
        }
    }
}