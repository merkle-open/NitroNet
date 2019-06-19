# Additional Arguments

Additional Arguments are based on the [Literals](https://handlebarsjs.com/#literals) concept of [Handlbars.js](https://handlebarsjs.com). This feature allows you to pass literal values a helper.

In NitroNet this feature has been extended to not only pass literal values but also a values from the current context as well as a sub context which gets passed to the helper together with additional arguments which overwrite or extend the data from the sub context.

## Configuration

As the introduction of this feature changes the behaviour of previous versions, a multiple flags have been added to enable and steer this feature via the already known nitronet-config.json file explained in [Configuration](configuration.md).

By default or if not present, the feature is disabled. This is equal as setting literalParsingMode to none and additionalArgumentsOnlyComponents to false.

<table>
    <tr>
        <th align="left">Setting</th>
        <th align="left">Default</th>
        <th align="left">Description</th>
    </tr>
    <tr>
        <td>literalParsingMode</td>
        <td>none</td>
        <td>
            Which literals should be parsed?
            <ul>
            <li>none: The feature is completely disabled.</li>
            <li>staticliteralsonly: Only static strings, numbers, booleans and null/undefined are allowed</li>
            <li>full: Next to the static strings also object identfiers are allowed</li>
            </ul>
            See examples for more detail.
        </td>
    </tr>
    <tr>
        <td>additionalArgumentsOnlyComponents</td>
        <td>false</td>
        <td>This feature only has an effect if literalParsingMode is staticvaluesonly or full.<br>If enabled, the component helper is being called with the given additional arguments only even if no suiting sub model is available on the current context. This is especially important in the current NitroNet.Sitecore implementation whereas the component handler tries to call a Sitecore component or a controller if no suiting sub model is available. If you want to use the named feature this setting must stay disabled.</td>
    </tr>
</table>

```json
{
    "viewPaths": [
        "..."
    ],
    "partialPaths": [
        "..."
    ],
    "componentPaths": [
        "..."
    ],
    "extensions": [
        "..."
    ],
    "filters": [
        "..."
    ],
    "literalParsingMode":"none|full|staticliteralsonly",
    "additionalArgumentsOnlyComponents": true
}
```

## Examples
To explain the functionality of the feature we start with a set of C# models and some simple Handlebars templates and change it accordingly to demonstrate the extent of the new feature.
#### Models:
```csharp
public class MovieModel
{
	public string Title { get; set; }
	public string Abstract { get; set; }
	public int ProductionYear { get; set; }
	public DirectorModel Director { get; set; }
	public double ImdbRating { get; set; }
}

public class DirectorModel {
    public string FirstName {get; set;}
    public string Surname {get; set;}
}
```
#### Handlebars:
##### movie.hbs
```hbs
<div>
    <p>{{title}} - ({{productionYear}})</p>
    <p> IMDb Rating: {{imdbRating}}</p>
    <p>{{abstract}}</p>
    {{ pattern name="director"}}
</div>
```
##### director.hbs
```hbs
<p>{{firstName}} {{surname}}</p>
```
### LiteralsParsingMode: None
This is the classic way, create your C# models suiting the Handlebars templates and let it render. So far so good, nothing new here.

### LiteralsParsingMode: StaticLiteralsOnly
Now we want to extend the director.hbs with an additional property
#### Extend director.hbs
```hbs
<p>{{firstName}} {{surname}} {{#if profession}}- Profession: {{profession}}{{/if}}</p>
```

#### The movie.hbs gets a change too
As we new it will always be a director, we can add an additional argument to the pattern helper.
```hbs
<div>
    <p>{{title}} - ({{productionYear}})</p>
    <p> IMDb Rating: {{imdbRating}}</p>
    <p>{{abstract}}</p>
    {{ pattern name="director" role="Director" }}
</div>
```
As you can see, the string "Director" is a static value. To get it correctly passed to the person Handlebars template. We need to enable **LiteralsParsingMode** at least in the **StaticLiteralsOnly**-mode. This passes the *DirectorModel* and additionally the static value "Director" with the property name *profession* to the director.hbs pattern.

This would result in a html like this:
```html
<div>
    <p>Star Wars: Episode IV - A New Hope - (1977)</p>
    <p> IMDb Rating: 8.6</p>
    <p>Luke Skywalker...</p>
    <p>George Lucas - Role: Director</p>
</div>
```
The mode **StaticLiteralsOnly** supports strings, numbers, booleans, null and undefined. Strings must be contained in either "" or ''.

<span style="color:red">**Attention:**</span> Dynamic properties as described in the following chapter are ignored in this mode.

### LiteralsParsingMode: Full
If you want to use dynamic values to pass to the pattern, this is possible if you change the **LiteralsParsingMode** to **full**.
```csharp
public class MovieModel
{
    public string Director { get; set; } = "Director";
    public string Title { get; set; }
    public string Abstract { get; set; }
    public int ProductionYear { get; set; }
    public DirectorModel Director { get; set; }
    public double ImdbRating { get; set; }
}
```
Change the movie.hbs and instead of "Director" we write it this way:
```hbs
<div>
    <p>{{title}} - ({{productionYear}})</p>
    <p> IMDb Rating: {{imdbRating}}</p>
    <p>{{abstract}}</p>
    {{ pattern name="director" role=director }}
</div>
```
The mode **Full** resolves properties from the current context. Also nested properties are possible, e.g. director.firstName. 

<span style="color:red">**Attention:**</span> If the mode is set to full and the provided property is not resolvable an **Exception** is thrown, e.g. director.someUnknownProperty is not possible!

### Render patterns without a model a.k.a. *additionalArgumentsOnlyComponents*
So far if you want to use the pattern/components helper you had to provide a model matching the *name* parameter on pattern or pass it explicitely via the *data* parameter.
If you enable **additionalArgumentsOnlyComponents** you don't need this anymore. You can provide every parameter you want to pass to the helper inline and the pattern gets rendered with this data.

To show the principle we change our model. We want to add actors to the *movie.hbs*. Actors have the same properties a a director as they are persons too. So we replace the *DirectorModel* with the *PersonModel* and add a list of actors to the *MovieModel*.

#### Models
```csharp
public class MovieModel
{
    public const string Director = "Director"
    public string Title { get; set; }
    public string Abstract { get; set; }
    public int ProductionYear { get; set; }
    public PersonModel Director { get; set; }
    public IEnumerable<PersonModel> Actors {get; set;}
    public double ImdbRating { get; set; }
}

public class PersonModel {
    public string FirstName {get; set;}
    public string Surname {get; set;}
}
```
The Handlebars templates get a change too:
#### Handlebars
##### movie.hbs
```hbs
<div>
    <p>{{title}} - ({{productionYear}})</p>
    <p> IMDb Rating: {{imdbRating}}</p>
    <p>{{abstract}}</p>
    {{ pattern name="person" firstName=director.firstName surname=director.surname role=director }}
    {{#each actors}}
        {{ pattern name="person" firstName=firstName surname=surname role="Actor" }}
    {{/each}}
    <p>(and many more...)</p>
</div>
```
##### Rename director.hbs to person.hbs
```hbs
<p>{{firstName}} {{surname}}{{#if role}} - Role: {{role}}{{/if}}</p>
```
#### Example Result HTML:
```html
<div>
    <p>Star Wars: Episode IV - A New Hope - (1977)</p>
    <p> IMDb Rating: 8.6</p>
    <p>Luke Skywalker...</p>
    <p>George Lucas - Role: Director</p>
    <p>Mark Hamill - Role: Actor</p>
    <p>Carry Fisher - Role: Actor</p>
    <p>(and many more...)</p>
</div>
```

As you can see in the Handlebars templates, with **LiteralsParsingMode:Full** in combination with **additionalArgumentsOnlyComponents:true** you have full flexibility how to pass values to your patterns.

<span style="color:red">**Attention:**</span> Without **LiteralsParsingMode** in mode **Full** or **StaticLiteralsOnly** the **additionalArgumentsOnlyComponents** setting has no effect as it only makes sense if additional arguments are parsed in some way.

