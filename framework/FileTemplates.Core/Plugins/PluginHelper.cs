using FileTemplates.API.Plugins;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileTemplates.Core.Plugins
{
    public class PluginHelper
    { 
        public static T ReadPluginConfiguration<T>(string path) where T : class
        {
            if (!File.Exists(path))
                SavePluginConfiguration<T>(path, null);
            
            return DeserializeFromJsonFile<T>(path);
        }

        public static void SavePluginConfiguration<T>(string path, T obj) where T : class
        {
            if (obj == null)
                obj = Activator.CreateInstance<T>();

            SerializeToJsonFile(path, obj);
        }

        public static IDictionary<string, string> ReadPluginTranslations(string path, IDictionary<string, string> defaultTranslations)
        {
            if (!File.Exists(path))
            {
                SerializeToJsonFile(path, defaultTranslations);
                return defaultTranslations;
            }

            return DeserializeFromJsonFile<IDictionary<string, string>>(path);
        }

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
