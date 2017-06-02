# NitroNet

### What's Nitro?

[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint.  
It provides a proven but flexible structure to develop your frontend code, even in a large team.  
Keep track of your code with a modularized frontend. This app and the suggested [atomic design](http://bradfrost.com/blog/post/atomic-web-design/) and [BEM](https://en.bem.info/method/definitions/) concepts could help.  
Nitro is simple, fast and flexible. It works on OSX, Windows and Linux. Use this app for all your frontend work.

### What's NitroNet ?

NitroNet is a full integration of Nitro frontends into ASP.NET. It is based on the project [TerrificNet](https://github.com/namics/TerrificNet) which uses [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) of [Chris Sainty](https://github.com/csainty) in order to parse handlebars templates. Handlebars.js is the primary template engine of Nitro. In summary, NitroNet is a completely new and simple view engine for ASP.NET MVC web applications. NitroNet is created by [Fabian Geiger](https://github.com/fgeiger).

### Sub projects
As a popular sub project, we have extended NitroNet to use this .Net view engine in the Web Content Management System [Sitecore](http://www.sitecore.net). You can find more informations about this project under [NitroNetSitecore](https://github.com/namics/NitroNetSitecore).

## Installation

### Preconditions
You need your own Nitro project as precondition of this installation manual. Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Create a ASP.NET MVC application
Create a ASP.NET MVC solution on your local machine with Visual Studio and compile the solution. 

### Step 2 - Install NitroNet

Please choose between variant **A** with Unity/CastleWindsor or **B** with another IoC Framework.

#### (A) Directly with IoC container

##### NuGet Package installation
There are several ways to install NitroNet into Sitecore. The easiest way is to use NitroNet together with Unity or CastleWindsor. Execute the following line in your NuGet Package Manager to install NitroNet for Sitecore:

`PM >` `Install-Package NitroNet.UnityModules`

Optionally, we recommend to install the [Unity.Mvc](https://www.nuget.org/packages/Unity.Mvc/) which is a lightweight Unity bootstrapper for MVC applications.

*or*

`PM >` `Install-Package NitroNet.CastleWindsorModules` 

##### Extend your Global.asax
To activate NitroNet it's important to add/register the new view engine in your application. You can do this, with these lines of code ([Gist](https://gist.github.com/daniiiol/62dd61615fcd73dc8386f56b69ed1a06):

	<%@Application Language='C#' %>
	<%@ Import Namespace="NitroNet" %>
	<script RunAt="server">
	    
	    public void Application_Start()
	    {
	        ViewEngines.Engines.Add(DependencyResolver.Current.GetService<NitroNetViewEngine>());
	    }
	</Script>

##### Register the IoC containers
In this NuGet package, you got all necessary code classes to configure and register NitroNet with Unity. To activate NitroNet, please add these lines to your application ([Gist](https://gist.github.com/daniiiol/a3c9d214dbe555dcb4550d7642d14c35))

	public static void RegisterTypes(IUnityContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultUnityModule(basePath).Configure(container);
    }

Also you got all necessary code classes to configure and register NitroNet with CastleWindsor. To activate NitroNet add these lines to your application (https://gist.github.com/daniiiol/0f5c8f3cc8725d329e27e4c7bb6bb16f)

	public static void RegisterTypes(IWindsorContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultCastleWindsorModule(basePath).Configure(container);
    }

#### (B) Directly without the Unity IoC framework
You don't like Unity and you design your application with an other IoC framework? No Problem. In this case, you can install NitroNet only with our base package:

`PM >` `Install-Package NitroNet`

##### Extend your Global.asax
*Please extend your Global.asax in the same way as in scenario (A)* 

##### Register NitroNet with your own IoC framework
Actually, we only made a Unity integration with NitroNet. But it's easy to use another IoC Framework. Please follow our Unity sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

###### DefaultUnityModule

	using Microsoft.Practices.Unity;
	using NitroNet;
	using NitroNet.Mvc;
	using NitroNet.ViewEngine;
	using NitroNet.ViewEngine.Cache;
	using NitroNet.ViewEngine.Config;
	using NitroNet.ViewEngine.IO;
	using NitroNet.ViewEngine.TemplateHandler;
	using NitroNet.ViewEngine.ViewEngines;
	using System.Web;
	using Veil.Compiler;
	using Veil.Helper;
	
	namespace NitroNet.UnityModules
	{
	  public class DefaultUnityModule : IUnityModule
	  {
	    private readonly string _basePath;
	
	    public DefaultUnityModule(string basePath)
	    {
	      this._basePath = basePath;
	    }
	
	    public void Configure(IUnityContainer container)
	    {
	      this.RegisterConfiguration(container);
	      this.RegisterApplication(container);
	    }
	
	    protected virtual void RegisterConfiguration(IUnityContainer container)
	    {
	      INitroNetConfig nitroNetConfig = ConfigurationLoader.LoadNitroConfiguration(this._basePath);
	      UnityContainerExtensions.RegisterInstance<INitroNetConfig>(container, nitroNetConfig);
	      UnityContainerExtensions.RegisterInstance<IFileSystem>(container, (IFileSystem) new FileSystem(this._basePath, nitroNetConfig));
	    }
	
	    protected virtual void RegisterApplication(IUnityContainer container)
	    {
	      UnityContainerExtensions.RegisterInstance<AsyncLocal<HttpContext>>(container, new AsyncLocal<HttpContext>(), (LifetimeManager) new ContainerControlledLifetimeManager());
	      UnityContainerExtensions.RegisterType<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
	      UnityContainerExtensions.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>(container);
	      UnityContainerExtensions.RegisterType<INamingRule, NamingRule>(container);
	      UnityContainerExtensions.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>(container);
	      UnityContainerExtensions.RegisterType<IViewEngine, VeilViewEngine>(container);
	      UnityContainerExtensions.RegisterType<ICacheProvider, MemoryCacheProvider>(container);
	      UnityContainerExtensions.RegisterType<IComponentRepository, DefaultComponentRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
	      UnityContainerExtensions.RegisterType<ITemplateRepository, NitroTemplateRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
	      UnityContainerExtensions.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
	    }
	  }
	}


### Step 3
*Oh sorry... there's no Step 3 to work with NitroNet :)*


## Configuration
... comming soon.


## Credits and special thanks

Thanks to all the people who made and released these awesome resources for free!

Special thanks to [Marco Schälle](https://github.com/marcoschaelle) and [Stefan Schälle](https://github.com/schaelle) who fight for a smart way to integrate Frontends into ASP.NET and create the predecessor [TerrificNet](https://github.com/namics/TerrificNet) of NitroNet itself. Marco and Stefan were our opinion leaders of this product.

Also special thanks to [Mark Cassidy](https://github.com/cassidydotdk) for all product commits and propagation of our idea to the whole world.