using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shearlegs.Core.Constants
{
    public class DirectoryConstants
    {
        public const string PluginsDirectory = "Plugins";
        public const string LibrariesDirectory = "Libraries";
        public const string LogsDirectory = "Logs";

        public static string LogFile(string sessionId) => Path.Combine(LogsDirectory, $"{sessionId}.log");

        public static string PluginDirectory(string pluginName) => Path.Combine(PluginsDirectory, pluginName);
        public static string PluginConfigurationFile(string pluginName) => Path.Combine(PluginDirectory(pluginName), $"{pluginName}.configuration.json");
        public static string PluginTranslationsFile(string pluginName) => Path.Combine(PluginDirectory(pluginName), $"{pluginName}.translations.json");
    }
}
