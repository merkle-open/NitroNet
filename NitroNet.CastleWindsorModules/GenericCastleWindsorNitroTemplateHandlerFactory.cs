using Castle.Windsor;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.CastleWindsorModules
{
    public class GenericCastleWindsorNitroTemplateHandlerFactory<T> : INitroTemplateHandlerFactory where T : INitroTemplateHandler
    {
        private readonly IWindsorContainer _container;

        public GenericCastleWindsorNitroTemplateHandlerFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public INitroTemplateHandler Create()
        {
            return _container.Resolve<T>();
        }
    }
}