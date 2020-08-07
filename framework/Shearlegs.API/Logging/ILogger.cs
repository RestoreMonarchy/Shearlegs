using Shearlegs.API.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.API.Logging
{
    public interface ILogger
    {
        Task LogAsync(string message);
        Task LogExceptionAsync(Exception e, string message = null);
        Task LogInformationAsync(string message);
    }
}
