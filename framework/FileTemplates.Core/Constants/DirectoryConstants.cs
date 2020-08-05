using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileTemplates.Core.Constants
{
    public class DirectoryConstants
    {
        public const string PluginsDirectory = "Plugins";
        public const string LibrariesDirectory = "Libraries";

        public static string PluginDirectory(string pluginName) => Path.Combine(PluginsDirectory, pluginName);
        public static string PluginConfigurationFile(string pluginName) => Path.Combine(PluginDirectory(pluginName), pluginName + ".configuration.json");
    }
}
