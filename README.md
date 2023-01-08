# Blazor Component Registry
![Build](https://github.com/andrew-gaston/BlazorComponentRegistry/actions/workflows/build.yml/badge.svg)

![BlazorComponentRegistry User Interface](https://raw.githubusercontent.com/andrew-gaston/BlazorComponentRegistry/master/images/blazor-component-registry-ui.png)

This library allows you to view which of your Blazor components are active on the page, as well as their current parameter values. The intended functionality is similar to React Dev Tools and Vue Dev Tools.

## Installation
```
dotnet add package BlazorComponentRegistry
```
### Add the following to your _Imports.razor file
```csharp
@using BlazorComponentRegistry
@using BlazorComponentRegistry.Services
@using BlazorComponentRegistry.UI
```

### Register the ComponentRegistryService in Program.cs

```csharp
// Blazor Webassembly Program.cs
public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    builder.Services.AddSingleton<IComponentRegistryService, ComponentRegistryService>();
    await builder.Build().RunAsync();
}
```

```csharp
// Blazor Server Program.cs
public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddScoped<IComponentRegistryService, ComponentRegistryService>();
    var app = builder.Build();

    ...

    app.Run();
}
```
## Example Usage

### Automatically Register and Unregister a component by inheriting from RegisterableComponent

```csharp
@inherits RegisterableComponent
```

### Manually Register and Unregister a component with the ComponentRegistryService 
Your component should implement IDisposable so that the component instance becomes untracked when the component is disposed.

``` csharp
@implements IDisposable
...
@code {
    [Parameter] public string ParentComponentGuid { get; set; }
    private string componentGuid;

    protected override void OnInitialized()
    {
        componentGuid = componentRegistry.RegisterComponent(ParentComponentGuid, GetType()).ComponentGuid;
    }

    public void Dispose()
    {
        componentRegistry.UnregisterComponent(componentGuid);
    }
}
```

The RegisterComponent method will automatically generate a GUID string to uniquely identify the instance of the component

However, you can manually set its GUID if you wish. If a component with a duplicate GUID is registered, the RegisterComponent method will throw an exception.

```csharp
@implements IDisposable
...
@code {
    [Parameter] public string ParentComponentGuid { get; set; }
    private string componentGuid = Guid.NewGuid().ToString();

    protected override void OnInitialized()
    {
        componentRegistry.RegisterComponent(ParentComponentGuid, GetType(), componentGuid);
    }

    public void Dispose()
    {
        componentRegistry.UnregisterComponent(componentGuid);
    }
}
```

To view the currently active, registered components, use the ComponentRegistry component.
```
@* I recommend putting it in your MainLayout if you want it to display on every page *@
<ComponentRegistry/>
```

Make sure you pass the ParentGuid from the parent to all registered child components.

## Limitations
- Each component you wish to monitor must either be registered manually in the component's code OR inherit from the RegisterableComponent base class provided by this library.
- External Blazor Components, such as those provided by NuGet package, can't be tracked directly.
- Recursive (self-referencing) Components are not supported.

### Notes
- Open Github Issue for Blazor Dev Tools: https://github.com/dotnet/aspnetcore/issues/44825
- Steve Sanderson on limitations and building your own component tree: https://github.com/dotnet/aspnetcore/issues/26826#issuecomment-708403993