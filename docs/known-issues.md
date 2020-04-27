## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Limitations
Unfortunately not all handlerbars features are supported in NitroNet. This is due to the underlying handlebars parser [Handlebars.Net](https://github.com/rexm/Handlebars.Net) we are using.

To have an overview of what works and what not you can consult the [documentation of Handlebars.Net](https://github.com/rexm/Handlebars.Net).


## Currently not implemented

### Nitro features

#### Flexible attributes on component helper
You will find the Nitro documentation for the flexible attribute [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns).

```
{{pattern 'example' modifier='blue'}}
```

In the current version NitroNet does not support this feature. You need to pass another data variation:
```
{{pattern 'example' data='example-blue'}}
```

#### Render patterns with children
You will find the Nitro documentation for patterns with children [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns-with-children).

```
{{#pattern 'box'}}
    {{pattern 'example'}}
{{/pattern}}
```

In the current version NitroNet does not support this feature. Please use the placeholder feature for this use case.

#### Translation handlebars helper
Nitro ships with a Handlebars helper called `t`. This is not supported in NitroNet as there is no need for such a helper in ASP.NET. You can just use a string expressions in its place and fill the according model property dynamically with data from any source.