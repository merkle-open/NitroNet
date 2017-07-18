![NitroNet Logo](docs/logo.png)

NitroNet is an ASP.NET MVC view engine for [handlebars](http://handlebarsjs.com) with an intelligent built-in file system and the possibility to add custom helpers. It works with ASP.NET MVC web applications and uses [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) from [Chris Sainty](https://github.com/csainty) as the underlying handlebars parser. It is originally based on the project [TerrificNet](https://github.com/namics/TerrificNet).
The support for the custom helpers used in [Nitro](https://github.com/namics/generator-nitro/) are already implemented by default.

### NitroNet features
- NitroNet possesses a smart built-in file system which caches all your frontend files and listens to changes. If files are updated on disk they are also updated in the cache. This happens instantly and without an IIS restart.
	- You specify your frontend paths and file extension
	- It's also possible to filter specific paths with regex patterns
- The used handlebars parser Veil is customized and thus allows you to implement and register your own handlebars helpers.
- Out of the box integration as view engine
- Different NuGets for your preferred IoC framework

### Sub projects
As a sub project, NitroNet was extended to be used in the WCMS [Sitecore](http://www.sitecore.net). You can find more informations about this project under [NitroNetSitecore](https://github.com/namics/NitroNetSitecore).

### What's Nitro?
[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint. It provides a proven but flexible structure to develop your frontend code, even in a large team.

## Table of contents
- [Installation](docs/installation.md)
- [Configuration](docs/configuration.md)
- [Getting started](docs/getting-started.md)
- [Samples](docs/samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](docs/known-issues.md)

## Future roadmap
- General features:
	- [ ] Make the underlying handlebars parser updatable.
	- [ ] Make registering other handlebars helpers easier.

- Nitro support:
	- [ ] Flexible attributes on component helper. [Link to the Nitro documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns).
	- [ ] Generate and validate model classes from the schema.json
	- [ ] Render patterns with children. [Link to the Nitro documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns-with-children).

## Credits and special thanks
Thanks to all the people who made and released these awesome resources for free!

Special thanks to [Marco Schälle](https://github.com/marcoschaelle) and [Stefan Schälle](https://github.com/schaelle) who fight for a smart way to integrate Frontends into ASP.NET and created the predecessor [TerrificNet](https://github.com/namics/TerrificNet) of NitroNet itself. Marco and Stefan were our opinion leaders of this product.

Also special thanks to [Mark Cassidy](https://github.com/cassidydotdk) for all product commits and propagation of our idea to the whole world.