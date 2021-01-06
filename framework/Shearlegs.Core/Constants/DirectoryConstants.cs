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
    }
}
