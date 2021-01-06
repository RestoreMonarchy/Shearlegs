using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Reports
{
    public interface IReportParameters
    {
        string this[string key] { get; }

        T GetValue<T>(string key) where T : IConvertible;
    }
}
