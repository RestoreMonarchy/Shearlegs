## Shearlegs
Shearlegs is universal .NET plugin framework

### Features
* Loads plugins and libraries .NET assemblies from `dll` files
* Uses Autofac Dependency Injection
* Provides configuration and translations to plugins
* Logger writes everything to file


### Usage
[SamplePlugin.cs](samples/SamplePlugin/SamplePlugin.cs)
```cs
public class SamplePlugin : Plugin
{
    public override string Name => "SamplePlugin";

    private readonly ILogger logger;
    private readonly SamplePluginConfiguration configuration;
    private readonly IDictionary<string, string> translations;

    public SamplePlugin(ILogger logger, SamplePluginConfiguration configuration, IDictionary<string, string> translations)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.translations = translations;
    }

    [DefaultTranslations]
    private static IDictionary<string, string> DefaultTranslations = new Dictionary<string, string>()
    {
        { "Hello", "World!" }
    };

    public override async Task LoadAsync()
    {
        await logger.LogAsync($"Hello {translations["Hello"]}");
        await logger.LogAsync($"{configuration.HelloWorld}");
        await logger.LogAsync($"Hello folks from {Name}!");
    }

    public override async Task UnloadAsync()
    {
        await logger.LogAsync($"Goodbye folks from {Name}!");
    }
}
```
[SamplePluginConfiguration.cs](samples/SamplePlugin/SamplePluginConfiguration.cs)
```cs
[Configuration(typeof(SamplePlugin))]
public class SamplePluginConfiguration
{
    public string HelloWorld { get; set; } = "Hello World!";
}
```
