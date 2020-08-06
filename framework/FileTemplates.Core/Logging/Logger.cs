using FileTemplates.API;
using FileTemplates.API.Logging;
using FileTemplates.Core.Constants;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileTemplates.Core.Logging
{
    public class Logger : ILogger
    {
        private readonly ISession session;

        public Logger(ISession session)
        {
            this.session = session;
        }

        public async Task LogAsync(string message)
        {
            message = $"[{DateTime.Now}] {message}";
            using (StreamWriter stream = File.AppendText(DirectoryConstants.LogFile(session.ID)))
            {
                await stream.WriteLineAsync(message);
            }
            Console.WriteLine(message);
        }
    }
}
