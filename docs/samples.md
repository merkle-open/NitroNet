## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Templating
Please visit the [Nitro Documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md) about informations and samples for working with Nitro.


## Standard Handlebars helpers

### Partials

Partials are simple includes of static markup into a base file and could be used for reducing the complexity of large view files.

NitroNet supports partials with the keyword `partial` or with the keyword `>` (normal convention):

```
{{partial name="head"}}

{{> head}}
```


### Components

#### An easy example
This sample shows a simple Nitro HTML view (handlebars markup) of a *Teaser* component.

##### Handlebars markup

```html
<div class="m-teaser">
	<div class="m-teaser__wrapper-left">

		<h2 class="font-page-title m-teaser__headline">
			{{#if headline}}
			<span class="m-teaser__headline-text">{{headline}}</span>
			{{/if}}
		</h2>
	</div>

	<div class="l-tile__content">
		{{#if abstract}}
		<h3 class="font-big-title m-teaser__abstract">{{{abstract}}}</h3>
		{{/if}}
		{{#if richtext}}
		<div class="font-copy-text m-teaser__rte">
				{{{richtext}}}
		</div>
		{{/if}}
	</div>
	<a href="#" class="a-button a-button--primary m-teaser__button">{{buttonText}}</a>
</div>
```

##### C# Model
The corresponding C# model is the following:

```csharp
public class TeaserModel
{
    public string Headline { get; set; }
    public string Abstract { get; set; }
    public string Richtext { get; set; }
    public string ButtonText { get; set; }
}
```

#### A component with repeating subentities

##### Handlebars markup

```html
<ul class="m-link-list">
    {{#each links}}
        <li class="m-link-list__item font-meta-navi"><a class="a-link" href="{{target}}">{{linkText}}</a></li>
    {{/each}}
</ul>
```

##### C# Models

In this case, we need a model class with an `IEnumerable<>` property called `links`. The parent model itself doesn't have any other properties:

```csharp
public class LinkListModel
{
    public IEnumerable<LinkModel> Links { get; set; }
}

public class LinkModel
{
    public string Target { get; set; }
    public string LinkText { get; set; }
}
```

#### A component with subcomponents
TODO