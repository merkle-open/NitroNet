using Microsoft.Practices.Unity;
using NitroNet.HandlebarsNet.Handlers;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.Mvc;
using NitroNet.Mvc.Context;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Utils;

namespace NitroNet.UnityModules
{
    public class DefaultUnityModule : IUnityModule
    {
        private readonly string _basePath;

        public DefaultUnityModule(string basePath)
        {
            _basePath = basePath;
        }

        public void Configure(IUnityContainer container)
        {
            RegisterConfiguration(container);
            RegisterApplication(container);
        }

        protected virtual void RegisterConfiguration(IUnityContainer container)
        {
            var config = ConfigurationLoader.LoadNitroConfiguration(_basePath);
            container.RegisterInstance(config);

            container.RegisterInstance<IFileSystem>(new FileSystem(_basePath, config));
        }

        protected virtual void RegisterApplication(IUnityContainer container)
        {
            container.RegisterType<IHandlebarsNetHelperHandlerFactory, HandlebarsNetHelperHandlerFactory>(
                new ContainerControlledLifetimeManager());
            //container.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>();
            //container.RegisterType<IMemberFilterFactory, MemberFilterFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<INamingRule, NamingRule>();
            container.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>();
            container.RegisterType<IViewEngine, HandlebarsNetViewEngine>();
            container.RegisterType<IHandlebarsNetEngine, HandlebarsNetEngine>();
            container.RegisterType<ICacheProvider, MemoryCacheProvider>();
            container.RegisterType<IComponentRepository, DefaultComponentRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITemplateRepository, NitroTemplateRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerUtils, NitroTemplateHandlerUtils>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IRenderingContextFactory, MvcRenderingContextFactory>(
                new ContainerControlledLifetimeManager());
        }
    }
}
