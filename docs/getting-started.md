## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Getting started with NitroNet

### Create a Controller

To use a Nitro based component in Sitecore, you have to create a Controller which inherits the normal `System.Web.Mvc.Controller` of ASP.NET MVC:

```csharp
public class TeaserController : System.Web.Mvc.Controller
{
	// GET: Teaser
	public ActionResult Index()
	{
		var model = new TeaserModel
		{
			Headline = "Lorem ipsum",
			Abstract = "Praesent ac massa at ligula laoreet iaculis. Cras id dui.",
			Richtext = "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>",
			ButtonText = "Button text"
		};

		return View("teaser", model);
	}
}
```

The whole magic of NitroNet happens on the returning line `return View("teaser", model);`. The string `"teaser"` must fit the directory path of a Nitro component relative to your `NitroNet.BasePath`.

The guidelines about how to create a corresponding C# model for the selected Nitro component get explained in the next section.

### Create a C# Model
It is quite easy to create a C# model based on the Nitro component.

Let's take a look at a specific example (teaser component from the previous section). In the Nitro folder of your component you find all needed information for the model definition.
In the handlebars file (mostly `.hbs` or `.html`) you will see all the properties of the component:

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

To determine the data types of the individual properties you have to look at the *data.json* of the component. It is located under `./_data/<molecule-name>.json`. Oftentimes there is more than one *data.json* for the different states the component can have.

```json
{
	"headline" : "Lorem ipsum",
	"abstract" : "Praesent ac massa at ligula laoreet iaculis. Cras id dui.",
	"richtext" : "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>",
	"ButtonText" : "Button text"
}
```

In this case every property is of type `string`.

Now let's create the corresponding C# model class. Please make sure that the properties have the same name. The only thing you don't need to worry about is case sensitivity. Possible hyphens are also ignored:

```csharp
public class TeaserModel
{
    public string Headline { get; set; }
    public string Abstract { get; set; }
    public string Richtext { get; set; }
    public string ButtonText { get; set; }
}
```

#### Supported Types

Every possible data type is supported if it corresponds with the Nitro component data structure (as mentioned before the *data.json* is crucial here).

```csharp
public class FooModel
{
    public string Text { get; set; }
    public int Numeric { get; set; }
    public bool Abstract { get; set; }
    public SpecialClassModel Bar { get; set; }
    public IEnumerable<SpecialClassModel> Items { get; set; }
    ...
}
```