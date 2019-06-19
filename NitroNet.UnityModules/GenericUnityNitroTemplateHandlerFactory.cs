using Microsoft.Practices.Unity;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.UnityModules
{
    public class GenericUnityNitroTemplateHandlerFactory<T> : INitroTemplateHandlerFactory
        where T : INitroTemplateHandler
    {
        private readonly IUnityContainer _container;

        public GenericUnityNitroTemplateHandlerFactory(IUnityContainer container)
        {
            _container = container;
        }

        public INitroTemplateHandler Create()
        {
            return _container.Resolve<T>();
        }
    }
}