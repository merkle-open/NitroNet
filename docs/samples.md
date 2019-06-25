## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
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

```hbs
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

<span style="color:red">**New:**</span> Checkout the newest extension to the Compoments Helper -> [Additional Arguments](additional-arguments.md)

#### A component with subcomponents
Nested components are handled by one Controller action and don't invoke a new Controller action for each subcomponent. But it is necessary that you provide a model of the subcomponent.

In the following example we will look more detailed into that:
The `LocationController` has the responsibility to create all parts of the `LocationModel` which also includes data of the sub component `Bubble`.

#### Handlebars markup

```hbs
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

You need to make sure that there is always a property defined in the model for each subcomponent (exception: [Situation C](#situation-c---component-with-name-and-additional-arguments)). It holds the data which is then passed to the subcomponent. Make sure that it either matches the `name` or `data` (but only when the `data` attribute is present) attribute of the corresponding component helper. You don't need to worry about case sensitivity and hyphens.

##### Situation A - Component with name
View snippet:

```hbs
{{component name="Bubble"}}
```

Model snippet (maps the `name` attribute)

```csharp
public BubbleModel Bubble { get; set; }
```

##### Situation B - Component with name and data
View snippet:
```hbs
{{component name="Bubble" data="bubbleLocation"}}
```

Model snippet (maps the `data` attribute)

```csharp
public BubbleModel BubbleLocation { get; set; }
```

##### Situation C - Component with name and additional arguments
This situation only can occur when the feature [Additional Arguments](additional-arguments.md) is enabled (either **Full** or **StaticLiteralsOnly** mode) and the **enableAdditionalArgumentsOnly** is true.

View snippet:

```hbs
{{component name="Bubble" description=bubbleLocation.description key=bubbleLocation.key target=bubbleLocation.target name=bubbleLocation.name}}
```

Model snippet (doesn't map the model but uses it to resolve the values in the hbs)

```csharp
public BubbleModel BubbleLocation { get; set; }
```

The values from the `BubbleLocation` model are getting passed to the component.