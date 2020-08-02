using FileTemplates.API.Logging;
using System;
using System.Threading.Tasks;

namespace FileTemplates.Core.Logging
{
    public class Logger : ILogger
    {
        public Task LogAsync(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
