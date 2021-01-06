using Newtonsoft.Json.Linq;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.Core.Reports
{
    public class ReportParameters : IReportParameters
    {
        private readonly string json;
        private readonly JObject data;

        public ReportParameters(string json)
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
