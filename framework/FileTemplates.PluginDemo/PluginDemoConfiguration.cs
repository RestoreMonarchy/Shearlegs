using FileTemplates.API.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTemplates.PluginDemo
{
    [Configuration(typeof(PluginDemo))]
    public class PluginDemoConfiguration
    {
        public string SampleConfigProperty { get; set; } = "Hello World!";
    }
}
