using Shearlegs.API;
using Shearlegs.API.Logging;
using Shearlegs.Core.Constants;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Shearlegs.Core.Logging
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
            Console.WriteLine(message);
            await LogToFileAsync(message);
        }

        public async Task LogExceptionAsync(Exception e, string message = null)
        {
            if (message != null)
                await LogInformationAsync(message);
            ConsoleWriteLineColor(e, ConsoleColor.Red);
            await LogToFileAsync(e.ToString());
        }

        public async Task LogInformationAsync(string message)
        {
            ConsoleWriteLineColor(message, ConsoleColor.Yellow);
            await LogToFileAsync(message);
        }

        private void ConsoleWriteLineColor(object value, ConsoleColor consoleColor)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(value);
            Console.ForegroundColor = previousColor;
        }

        private async Task LogToFileAsync(string message)
        {
            using (StreamWriter stream = File.AppendText(DirectoryConstants.LogFile(session.ID)))
            {
                await stream.WriteLineAsync(message);
            }
        }
    }
}
