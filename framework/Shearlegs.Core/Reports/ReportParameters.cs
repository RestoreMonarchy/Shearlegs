using Newtonsoft.Json.Linq;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.Core.Reports
{
    public class ReportParameters : IReportParameters
    {
        public string Data { get; }
        private readonly JObject obj;

        public ReportParameters(string data)
        {
            Data = data;
            obj = JObject.Parse(data);
        }

        public T GetValue<T>(string key) where T : IConvertible
        {
            if (TryGetValue(key, out T value))
            {
                return value;
            }
            throw new ArgumentException($"Parameter key {key} not found");
        }

        public bool TryGetValue<T>(string key, out T value) where T : IConvertible
        {
            value = default;
            if (obj.TryGetValue(key, out JToken token))
            {
                value = token.ToObject<T>();
                return true;
            }
            return false;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default) where T : IConvertible
        {
            if (TryGetValue(key, out T value))
            {
                return value;
            }
            return defaultValue;
        }

        public string this[string key] => GetValue<string>(key);
    }
}
