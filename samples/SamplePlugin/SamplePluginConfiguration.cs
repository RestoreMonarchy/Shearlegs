using Shearlegs.API.Plugins;

namespace SamplePlugin
{
    [Configuration(typeof(SamplePlugin))]
    public class SamplePluginConfiguration
    {
        public string HelloWorld { get; set; } = "Hello World!";
    }
}
