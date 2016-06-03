# NitroNet

### What's Nitro?

[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint.  
It provides a proven but flexible structure to develop your frontend code, even in a large team.  
Keep track of your code with a modularized frontend. This app and the suggested [atomic design](http://bradfrost.com/blog/post/atomic-web-design/) and [BEM](https://en.bem.info/method/definitions/) concepts could help.  
Nitro is simple, fast and flexible. It works on OSX, Windows and Linux. Use this app for all your frontend work.

### What's NitroNet ?

NitroNet is a full integration of Nitro-Frontends into ASP.NET. Nitro itself based on the Template-Engine handlebars.js. NitroNet using the same Template-Engine as Nitro with the parsing framework [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) of [Chris Sainty](https://github.com/csainty) and combine the best of both worlds. In summary, NitroNet is a completely new and simple View-Engine for ASP.NET MVC Web Applications. NitroNet is created by [Fabian Geiger](https://github.com/fgeiger).

### Subprojects
As a popular Subproject, we have extend NitroNet to use this .Net View-Engine in the Web Content Management System [Sitecore](http://www.sitecore.net). You can find more informations about this Project under [NitroNetSitecore](https://github.com/namics/NitroNetSitecore).

## Installation

### Preconditions
You need a own Nitro Project as precondition of this installation manuel. Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Create a ASP.NET MVC Application
Create a ASP.NET MVC Solution on your local machine with Visual Studio and compile the solution. 

### Step 2 - Install NitroNet for Sitecore

Please choose between variant **A** with Unity or **B** with another IoC Framework.

#### (A) Directly with Unity IoC Container

##### NuGet Package Installation
There are several ways to install NitroNet in your MVC Application. The easiest way is to use NitroNet together with Unity. Execute following Line in your NuGet Package Manager or search the Package in your NuGet Browser:

`PM >` `Install-Package NitroNet.UnityModules` 

##### Extend your Global.asax
To activate NitroNet it's important to add/register the new View-Engine in your Application. You can do this, with these lines of code ([Gist](https://gist.github.com/daniiiol/62dd61615fcd73dc8386f56b69ed1a06):

	<%@Application Language='C#' %>
	<%@ Import Namespace="NitroNet" %>
	<script RunAt="server">
	    
	    public void Application_Start()
	    {
	        ViewEngines.Engines.Add(DependencyResolver.Current.GetService<NitroNetViewEngine>());
	    }
	</Script>

##### Register the Unity IoC Containers
In this NuGet Package, you got all necessary code classes to configure and register NitroNet with Unity. To Activate NitroNet, please add these lines to your UnityConfig.cs ([Gist](https://gist.github.com/daniiiol/a3c9d214dbe555dcb4550d7642d14c35))

	public static void RegisterTypes(IUnityContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultUnityModule(basePath).Configure(container);
    }

#### (B) Directly without the Unity IoC Framework
You don't like Unity and you design your application with an other IoC Framework? No Problem. In this case, you can install NitroNet only with our Base-Package:

`PM >` `Install-Package NitroNet`

##### Extend your Global.asax
*Please extend your Global.asax in the same way as in scenario (A)* 

##### Register NitroNet with your own IoC Framework
Actually, we only made a Unity-Integration with NitroNet. But it's easy to use an other IoC Framework. Following our Unity-Sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

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