![NitroNet Logo](docs/imgs/logo.png)

NitroNet is an ASP.NET MVC view engine for [handlebars](http://handlebarsjs.com) with an intelligent built-in file system and the possibility to add custom helpers. It works with ASP.NET MVC web applications and uses [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) from [Chris Sainty](https://github.com/csainty) as the underlying handlebars parser. It is originally based on the project [TerrificNet](https://github.com/merkle-open/TerrificNet).
The support for many of the custom helpers used in [Nitro](https://github.com/merkle-open/generator-nitro/) is already implemented by default.

### NitroNet features
- NitroNet possesses a smart built-in file system which caches all your frontend files and listens to changes. If files are updated on disk they are also updated in the cache. This happens instantly and without an IIS restart.
	- You specify your frontend paths and file extension
	- It's also possible to filter specific paths with regex patterns
- The used handlebars parser Veil is customized and thus allows you to implement and register your own handlebars helpers.
- Out of the box integration as view engine
- Different NuGets for your preferred IoC framework

### Sub projects
As a sub project, NitroNet was extended to be used in the WCMS [Sitecore](http://www.sitecore.net). You can find more informations about this project under [NitroNetSitecore](https://github.com/merkle-open/NitroNetSitecore).

### What's Nitro?
[Nitro](https://github.com/merkle-open/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint. It provides a proven but flexible structure to develop your frontend code, even in a large team.

## How to get started
For a particularly smooth start with NitroNet we advise you to just to through the chapters **Installation**, **Configuration** and **Getting started**.

If you need some code samples you can have a look a chapter **Samples** and if you having some problems or are interested how NitroNet is integrated or can be used, just have a look at our **Demo Integration/Solution**.

You can find all links below under [Table of contents](https://github.com/merkle-open/NitroNet#table-of-contents).

## Future roadmap
[Please look at the milestones regarding the features and time frames of future releases.](https://github.com/merkle-open/NitroNet/milestones)

**Important features planned in the near future:**
- [Auto generation of model classes](https://github.com/merkle-open/NitroNet/issues/20)
- Alternative to the underlying Veil parser?
  - [A first implementation](https://github.com/m-wagn/NitroNet/tree/feature/replace-veil-parser-with-handlebarsdotnet) has been done by [m-wagn](https://github.com/m-wagn) to replace [Veil](https://github.com/csainty/Veil) with the much more popular and supported [Handlebars.Net](https://github.com/rexm/Handlebars.Net)

## Contact / Contributing
If you want to submit a bug or request a feature please feel free to open an issue.

If you want to get in contact with us, just write an email to [Manuel Fischer](https://github.com/hombreDelPez) or [Fabian Geiger](https://github.com/naibafch).

Pull requests are welcome!

## Credits and special thanks
Thanks to all the people who made and released these awesome resources for free!

Special thanks to [Marco Schälle](https://github.com/marcoschaelle) and [Stefan Schälle](https://github.com/schaelle) who fight for a smart way to integrate Frontends into ASP.NET and created the predecessor [TerrificNet](https://github.com/merkle-open/TerrificNet) of NitroNet itself. Marco and Stefan were our opinion leaders of this product.

Also special thanks to [Mark Cassidy](https://github.com/cassidydotdk) for all product commits and propagation of our idea to the whole world.

## Table of contents
- [Installation](docs/installation.md)
- [Configuration](docs/configuration.md)
- [Getting started](docs/getting-started.md)
- [Samples](docs/samples.md)
- [Demo Integration](https://github.com/merkle-open/NitroNet.Demo)
- [Release Notes](https://github.com/merkle-open/NitroNet/releases)
- [Known Issues](docs/known-issues.md)