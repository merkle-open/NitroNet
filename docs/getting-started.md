## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNet/releases)
- [Known Issues](known-issues.md)

## Getting started with NitroNet
Before you start take a look at the [documentation of Veil.Handlebars](https://github.com/csainty/Veil/tree/master/src/Veil.Handlebars).  
There you also can see which handlebars features are supported and which not.

### Create a Controller

To use a Nitro based component in Sitecore, you have to create a Controller which inherits the normal `System.Web.Mvc.Controller` of ASP.NET MVC:

```csharp
public class MovieController : System.Web.Mvc.Controller
{
	// GET: Movie
	public ActionResult Index()
	{
		var model = new MovieModel
		{
			Title = "The best movie ever",
			Abstract = "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>"
			ProductionYear = 2007,
			Director = "Michael Bay",
			ImdbRating = 8.2
		};

		return View("movie", model);
	}
}
```

The whole magic of NitroNet happens on the returning line `return View("movie", model);`. The model is mapped on the movie view with the NitroNet view engine.

There are two ways how you can specify a view:
- Just set the name of the component/pattern (as shown in the example above): `return View("movie", model);`
- Set the complete view path (relative to your `Nitro.BasePath`). An example:
	- Your view file location is: `[web app root path]/frontend/patterns/molecules/movie/movie`
	- Your `NitroNet.BasePath` is: `"/frontend"`
	- Your resulting return statement is: `return View("patterns/molecules/movie/movie", model);`

The guidelines about how to create a corresponding C# model for the selected Nitro component get explained in the next section.

### Create a C# Model
It is quite easy to create a C# model based on the Nitro component.

Let's take a look at a specific example (movie component from the previous section). In the Nitro folder of your component you find all needed information for the model definition.
In the handlebars file (mostly `.hbs` or `.html`) you will see all the properties of the component:

```html
<div class="m-movie">
	{{#if title}}
		<h2>
			{{title}}
		</h2>
	{{/if}}

	<div class="m-movie__content">
		<div class="m-movie__abstract">
			{{{abstract}}}
		</div>
		<div class="m-movie__production-year">
			Production Year: {{productionYear}}
		</div>
		<div class="m-movie__director">
			Director: {{director}}
		</div>
		<div class="m-movie__imdb-rating">
			IMDb-Rating: {{imdbRating}}
		</div>
	</div>
</div>
```

If you are using Nitro as your frontend framework there is a *data.json* for each component. This file will help you to determin the data types of the individual properties. It is located under `./_data/<molecule-name>.json`. Oftentimes there is more than one *data.json* for the different states the component can have. If you don't have a *data.json* available then pick the type that suits the situation the best.

```json
{
	"title" : "The best movie ever",
	"abstract" : "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>",
	"productionYear" : 2007,
	"director" : "Michael Bay",
	"imdbRating" : 8.2
}
```

In this case we have properties of type `string`, `int` and `double`.

Now let's create the corresponding C# model class. Please make sure that the properties have the same name as the ones in the corresponding handlebars file. The only thing you don't need to worry about is case sensitivity. Possible hyphens are also ignored:

```csharp
public class MovieModel
{
	public string Title { get; set; }
	public string Abstract { get; set; }
	public int ProductionYear { get; set; }
	public string Director { get; set; }
	public double { get; set; }
}
```

#### Supported Types

Every possible data type is supported if it corresponds with the Nitro component data structure (as mentioned before the *data.json* is crucial here). If you are using a complex type at the lowest level you need to make sure that the `ToString()` method is implemented.

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