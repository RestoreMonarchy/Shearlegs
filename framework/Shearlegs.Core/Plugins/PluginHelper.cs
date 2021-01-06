using Newtonsoft.Json;
using System;
using System.IO;

namespace Shearlegs.Core.Reports
{
    public class PluginHelper
    {
        public static T DeserializeFromJsonFile<T>(string path) where T : class
        {
            string content;

            using (StreamReader stream = File.OpenText(path))
            {
                content = stream.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public static void SerializeToJsonFile(string path, object obj)
        {
            string content = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(path, false))
            {
                stream.Write(content);
            }
        }
    }
}
