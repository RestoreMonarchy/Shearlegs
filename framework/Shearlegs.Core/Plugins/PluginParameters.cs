using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.Core.Plugins
{
    public class PluginParameters
    {
        private readonly string json;
        private readonly JObject data;

        public PluginParameters(string json)
        {
            this.json = json;
            data = JObject.Parse(this.json);
        }

        public T GetValue<T>(string key) where T : IConvertible
        {
            return data[key].ToObject<T>();
        }

        public string this[string key]
        {
            get
            {
                return GetValue<string>(key);
            }
        }
    }
}
