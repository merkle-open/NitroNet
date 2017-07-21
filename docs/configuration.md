## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Configuration

### Requirements regarding the file structure of your frontend application
If you are building your frontend with [Nitro](https://github.com/namics/generator-nitro/) you already have a valid file structure and there is nothing to consider.

If you build your handlebars frontend yourself or with another generator, there are the following requirements the frontend has to meet:
- Every partial respectively component must have its own file.
- The partial/component name has to be identical to the file name of this component. And every partial/component must have a unique name!
	- Example: The file for a teaser partial/component has to be named `teaser.[file extension]`
- Every file name respectively partial/component name has to be unique.
- The base partial/component has to be in a folder with the same name.
	- Example: The `teaser` partial/component has to be in `[components folder]/teaser/teaser.[file extension]`
- The variations respectively skins of your base partial/component have to be in the same folder as the base  partial/component. You can distinguish your variations with a `-` following the name of the variation.
	- Example: the `teaser-blue` partial/component has to be in `[components folder]/teaser/teaser-blue.[file extension]`
	- Example: the `teaser-red` partial/component has to be in `[components folder]/teaser/teaser-red.[file extension]`

### Change the location of your frontend application
The location of your frontend application can be configured very flexibly.

As default it is set to be located at the root folder of your web application. If you like to change it, you can add an AppSetting with the Key-Value *NitroNet.BasePath* in your *Web.config*:

```xml
<configuration>
  <appSettings>
    <add key="NitroNet.BasePath" value="[path of your choice]" />
  </appSettings>
</configuration>
```

### Change the frontend file paths
In addition, you got a new `nitronet-config.json.example`-File in the root directory of the website project after installation of NitroNet for Sitecore. Rename it to `nitronet-config.json` to activate the config.

```json
{
  "viewPaths": [
    "frontend/views/"
  ],
  "partialPaths": [
    "frontend/views/_partials",
  ],
  "componentPaths": [
    "frontend/patterns/atoms",
    "frontend/patterns/molecules",
    "frontend/patterns/organisms",
  ],
  "extensions": [
    "hbs",
    "html"
  ],
    "filters": [
    ".*?\\/template\\/([\\w][^\\/]+)$",
    ".*?\\/spec\\/([\\w][^\\/]+)$"
 ]
}
```

Explanation to the individual settings/properties:
* **viewPaths**: The file path to your views, starting at your `NitroNet.BasePath`
* **partialPaths**: The file path to your partials, starting at your `NitroNet.BasePath`
* **componentPaths**: The file path to your components, starting at your `NitroNet.BasePath`
* **extensions**: The extensions of your handlebar files.
* **filters**: File paths which match with the `filters` regex are being ignored

### *Optional:* Add and register your own custom helpers
As mentioned before NitroNet comes with support for the [Nitro](https://github.com/namics/generator-nitro/) custom handlebars helpers by default.

But you can add your own helpers as well. You can achieve this by doing the following steps (the instructions are shown using the Unity IoC framework):

1.) Create your own helpers by implementing the `Veil.Helper.IHelperHandler`. See the classes in `NitroNet.ViewEngine.TemplateHandler` for reference.

2.) Create your own implementation of the `Veil.Helper.IHelperHandlerFactory`. You can take the `NitroNet.ViewEngine.TemplateHandler.DefaultRenderingHelperHandlerFactory` as a basis for your code.

```csharp
public class YourOwnHelperHandlerFactory : IHelperHandlerFactory
{
    public IEnumerable<IHelperHandler> Create()
    {
        //your own helpers
    }
}
```

3.) Register your implementation of `IHelperHandlerFactory` to your IoC container. The following example is for Unity and is added to the `RegisterTypes()` method in the `UnityConfig` class:

```csharp
container.RegisterType<IHelperHandlerFactory, YourOwnHelperHandlerFactory>(new ContainerControlledLifetimeManager());

```

after the following line:


```csharp
new DefaultUnityModule(basePath).Configure(container);
```