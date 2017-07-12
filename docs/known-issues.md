## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)


## Currently not implemented

### General features

#### Partials with handlebars expressions
Currently it is not possible to have partials with handlebars expressions. Only static markup is supported.

Use the `pattern` handlebars helper to achieve this functionality:
```
{{pattern name='head'}}
```

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