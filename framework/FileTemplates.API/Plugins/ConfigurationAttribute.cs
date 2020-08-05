using System;
using System.Collections.Generic;
using System.Text;

namespace FileTemplates.API.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigurationAttribute : Attribute
    {
        public Type PluginType { get; }

        public ConfigurationAttribute(Type pluginType)
        {
            PluginType = pluginType;
        }
    }
}
