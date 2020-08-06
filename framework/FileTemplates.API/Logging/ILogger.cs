using FileTemplates.API.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileTemplates.API.Logging
{
    public interface ILogger
    {
        Task LogAsync(string message);
        Task LogExceptionAsync(Exception e, string message = null);
    }
}
