using System.Web;
using Microsoft.Practices.Unity;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.ViewEngines;
using Veil.Compiler;
using Veil.Helper;

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
            container.RegisterType<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>();
            container.RegisterType<INamingRule, NamingRule>();
            container.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>();
            container.RegisterType<IViewEngine, VeilViewEngine>();
            container.RegisterType<ICacheProvider, MemoryCacheProvider>();
            container.RegisterType<IComponentRepository, DefaultComponentRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITemplateRepository, NitroTemplateRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
        }
    }
}
