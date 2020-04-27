using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.HandlebarsNet;
using NitroNet.ViewEngine.TemplateHandler.Utils;
using NitroNet.ViewEngine.ViewEngines;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using Veil.Compiler;

namespace NitroNet.CastleWindsorModules
{
    public class DefaultCastleWindsorModule : ICastleWindsorModule
    {
        private readonly string _basePath;

        public DefaultCastleWindsorModule(string basePath)
        {
            _basePath = basePath;
        }

        public void Configure(IWindsorContainer container)
        {
            RegisterConfiguration(container);
            RegisterApplication(container);
        }

        protected virtual void RegisterConfiguration(IWindsorContainer container)
        {
            var config = ConfigurationLoader.LoadNitroConfiguration(_basePath);
            container.Register(Component.For<INitroNetConfig>().Instance(config));
            container.Register(Component.For<IFileSystem>().Instance(new FileSystem(_basePath, config)));
        }

        protected virtual void RegisterApplication(IWindsorContainer container)
        {
            container.Register(Component.For<IHandlebarsNetHelperHandlerFactory>().ImplementedBy<HandlebarsNetHelperHandlerFactory>().LifestyleSingleton());
            container.Register(Component.For<IMemberLocator>().ImplementedBy<MemberLocatorFromNamingRule>().LifestyleSingleton());
            container.Register(Component.For<INamingRule>().ImplementedBy<NamingRule>().LifestyleSingleton());
            container.Register(Component.For<IModelTypeProvider>().ImplementedBy<DefaultModelTypeProvider>().LifestyleSingleton());
            container.Register(Component.For<IViewEngine>().ImplementedBy<HandlebarsNetViewEngine>().LifestyleSingleton());
            container.Register(Component.For<IHandlebarsNetEngine>().ImplementedBy<HandlebarsNetEngine>().LifestyleSingleton());
            container.Register(Component.For<ICacheProvider>().ImplementedBy<MemoryCacheProvider>().LifestyleSingleton());
            container.Register(Component.For<IComponentRepository>().ImplementedBy<DefaultComponentRepository>().LifestyleSingleton());
            container.Register(Component.For<ITemplateRepository>().ImplementedBy<NitroTemplateRepository>().LifestyleSingleton());
            container.Register(Component.For<INitroTemplateHandlerFactory>().ImplementedBy<MvcNitroTemplateHandlerFactory>().LifestyleSingleton());
            container.Register(Component.For<INitroTemplateHandlerUtils>().ImplementedBy<NitroTemplateHandlerUtils>().LifestyleSingleton());
            container.Register(Component.For<IMemberFilterFactory>().ImplementedBy<MemberFilterFactory>().LifestyleSingleton());
        }
    }
}
