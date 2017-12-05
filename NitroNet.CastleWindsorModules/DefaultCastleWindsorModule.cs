using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NitroNet.ModelBuilder.Controllers;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.ViewEngines;
using Veil.Compiler;
using Veil.Helper;

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
            container.Register(Component.For<ModelBuilderController>().Instance(new ModelBuilderController(new ModelBuilder.Generator.ModelBuilder(_basePath, config))));
        }

        protected virtual void RegisterApplication(IWindsorContainer container)
        {
            container.Register(Component.For<IHelperHandlerFactory>().ImplementedBy<DefaultRenderingHelperHandlerFactory>().LifestyleSingleton());
            container.Register(Component.For<IMemberLocator>().ImplementedBy<MemberLocatorFromNamingRule>().LifestyleSingleton());
            container.Register(Component.For<INamingRule>().ImplementedBy<NamingRule>().LifestyleSingleton());
            container.Register(Component.For<IModelTypeProvider>().ImplementedBy<DefaultModelTypeProvider>().LifestyleSingleton());
            container.Register(Component.For<IViewEngine>().ImplementedBy<VeilViewEngine>().LifestyleSingleton());
            container.Register(Component.For<ICacheProvider>().ImplementedBy<MemoryCacheProvider>().LifestyleSingleton());
            container.Register(Component.For<IComponentRepository>().ImplementedBy<DefaultComponentRepository>().LifestyleSingleton());
            container.Register(Component.For<ITemplateRepository>().ImplementedBy<NitroTemplateRepository>().LifestyleSingleton());
            container.Register(Component.For<INitroTemplateHandlerFactory>().ImplementedBy<MvcNitroTemplateHandlerFactory>().LifestyleSingleton());
        }
    }
}
