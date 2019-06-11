## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Demo Integration](https://github.com/namics/NitroNet.Demo)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Templating
Please visit the [handlebars documentation](http://handlebarsjs.com) about informations and samples.  
Or you can check out the [Nitro documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md) if you want to build your frontend with this application.


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

## Nitro helpers

### Components

#### A component with subcomponents
Nested components are handled by one Controller action and don't invoke a new Controller action for each subcomponent. But it is necessary that you provide a model of the subcomponent.

In the following example we will look more detailed into that:
The `LocationController` has the responsibility to create all parts of the `LocationModel` which also includes data of the sub component `Bubble`.

#### Handlebars markup

```html
<div class="m-location" data-t-name="Location">
    <a href="#">{{selectedLocation}}</a>
    <div>
        {{component name="Bubble" data="bubbleLocation"}}
        <ul>
            {{#each locations}}
                <li>
                    <a data-location-key="{{locationKey}}" href="{{target}}">{{name}}</a>
                </li>
            {{/each}}
        </ul>
    </div>
</div>

<div class="a-bubble" data-t-name="Bubble">
    {{description}}
	<a data-location-key="{{key}}" href="{{target}}">{{name}}</a>
</div>
```

#### C# Models

```csharp
public class LocationModel
{
    public string SelectedLocation { get; set; }
    public BubbleLocationModel BubbleLocation { get; set; }
    public IEnumerable<LocationModel> Locations { get; set; }
}

public class BubbleModel
{
    public string Description { get; set; }
    public string Key { get; set; }
    public string Target { get; set; }
    public string Name { get; set; }
}
```

You need to make sure that there is always a property defined in the model for each subcomponent. It holds the data which is then passed to the subcomponent. Make sure that it either matches the `name` or `data` (but only when the `data` attribute is present) attribute of the corresponding component helper. You don't need to worry about case sensitivity and hyphens.

##### Situation A - Component with name
View snippet:

```
{{component name="Bubble"}}
```

Model snippet (maps the `name` attribute)

```csharp
public BubbleModel Bubble { get; set; }
```

##### Situation B - Component with name and data
View snippet:
```
{{component name="Bubble" data="bubbleLocation"}}
```

Model snippet (maps the `data` attribute)

```csharp
public BubbleModel BubbleLocation { get; set; }
```

## Render Handlebars via Service

Sometimes it's useful to directly render a handlebars template to a string which than can be used to be displayed in a Razor file or some other context.

### Example
Demo Implementation:
```csharp
public class HandlebarsService
{
    private readonly MemberLocatorFromNamingRule _memberLocatorFromNamingRule;
    private readonly ITemplateRepository _templateRepository;
    private readonly ICacheProvider _provider;

    public HandlebarsService(INamingRule namingRule, ITemplateRepository templateRepository, ICacheProvider provider)
    {
        _templateRepository = templateRepository;
        _memberLocatorFromNamingRule = new MemberLocatorFromNamingRule(namingRule);
        _provider = provider;
    }

    public string Render(string templateId, object model)
    {
        var templateInfo = _templateRepository.GetTemplateAsync(templateId.ToLowerInvariant()).Result;

        var hash = string.Concat("template_", templateInfo.Id, templateInfo.ETag);

        if (!_provider.TryGet(hash, out Action<RenderingContext, object> compiledTemplate))
        {
            string content;
            using (var reader = new StreamReader(templateInfo.Open()))
            {
                content = reader.ReadToEnd();
            }

            compiledTemplate = new VeilEngine(new IHelperHandler[]{}, _memberLocatorFromNamingRule)
                .CompileNonGeneric(templateInfo.Id, new HandlebarsParser()), new StringReader(content), typeof(object));

            _provider.Set(hash, compiledTemplate, DateTimeOffset.Now.AddHours(24));
        }

        using (var writer = new StringWriter())
        {
            compiledTemplate(new RenderingContext(writer, null), model);
            return writer.ToString();
        }
    }
}
```
Consider the following Handlebars template:
```handlebars 
<p>Hello World</p>
{{#if additionalText}}
    <p>{{additionalText}}</p>
{{/if}}
<p>{{subModel.content}}</p>
```
Register your `HandlebarService` in DI and let it resolve where you need it (the other types are already registered via DI if you install a DI package for NitroNet, e.g. NitroNet.UnityModules).

```csharp
HandlebarsService handlebarsService = ResolveViaDI();
var renderedHandlebars = handlebarsService.Render("myHelloWorld", new {additionalText = "More", subModel = new {content="SubModel"}});
```

The rendered string will look the following:

```html
<p>Hello World</p>
<p>More</p>
<p>SubModel</p>
```

### Restrictions

This implementation only covers simple Handlebars templates. As you can see in this call:
```csharp
compiledTemplate = new VeilEngine(new IHelperHandler[]{}, _memberLocatorFromNamingRule)
                .CompileNonGeneric(templateInfo.Id, new HandlebarsParser()), new StringReader(content), typeof(object));
```

no additional helpers are passed to the VeilEngine, so only basic features which are supported by Veil.Handlebars are supported.