# NitroNet

### What's Nitro?

[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint.
It provides a proven but flexible structure to develop your frontend code, even in a large team.
Keep track of your code with a modularized frontend. This app and the suggested [atomic design](http://bradfrost.com/blog/post/atomic-web-design/) and [BEM](https://en.bem.info/method/definitions/) concepts could help.

Nitro is simple, fast and flexible. It works on OSX, Windows and Linux. Use this app for all your frontend work.

### What's NitroNet ?

NitroNet is a full integration of Nitro frontends into ASP.NET. It is based on the project [TerrificNet](https://github.com/namics/TerrificNet) which uses [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) of [Chris Sainty](https://github.com/csainty) in order to parse handlebars templates. Handlebars.js is the primary template engine of Nitro. In summary, NitroNet is a completely new and simple view engine for ASP.NET MVC web applications.

You can get more informations about NitroNet on our seperate [Git-Hub Project Page of NitroNet](https://github.com/namics/NitroNet).
NitroNet is created by [Fabian Geiger](https://github.com/NaibafCH).

### Sub projects
As a popular sub project, we have extended NitroNet to use this .NET view engine in the WCMS [Sitecore](http://www.sitecore.net). You can find more informations about this project under [NitroNetSitecore](https://github.com/namics/NitroNetSitecore).


## Table of contents
- [Installation](docs/installation.md)
- [Configuration](docs/configuration.md)
- [Getting started](docs/getting-started.md)
- [Samples](docs/samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](docs/known-issues.md)

## Future roadmap
- [ ] Flexible attributes on component helper. [Link to the Nitro documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns).
- [ ] Render patterns with children. [Link to the Nitro documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns-with-children).
- [ ] Partials with handlebars expressions.
- [ ] Clean up of unused IO classes
- [ ] Move the logic for template/data controller parameters from NitroNet.Sitecore to NitroNet
- [ ] Make the underlying handlebars parser updatable. Implement a abstraction layer. Update to the newest Handlerbars.Net or Veil.
- [ ] Generate and validate model classes from the schema.json

## Credits and special thanks

Thanks to all the people who made and released these awesome resources for free!

Special thanks to [Marco Schälle](https://github.com/marcoschaelle) and [Stefan Schälle](https://github.com/schaelle) who fight for a smart way to integrate Frontends into ASP.NET and create the predecessor [TerrificNet](https://github.com/namics/TerrificNet) of NitroNet itself. Marco and Stefan were our opinion leaders of this product.

Also special thanks to [Mark Cassidy](https://github.com/cassidydotdk) for all product commits and propagation of our idea to the whole world.