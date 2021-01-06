using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Reports
{
    public interface IReportParameters
    {
        string Data { get; }
        
        string this[string key] { get; }
        T GetValue<T>(string key) where T : IConvertible;
        bool TryGetValue<T>(string key, out T value) where T : IConvertible;
        T GetValueOrDefault<T>(string key, T defaultValue = default) where T : IConvertible;
    }
}
