using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shearlegs.API.Plugins.Attributes;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Plugins.Reports;
using Shearlegs.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestingApp
{
    class Program
    {
        private static Program instance;
        private IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            instance = new Program();
            instance.BuildServiceProvider();
            instance.InitializeAsync().GetAwaiter().GetResult();
        }

        private void BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            ShearlegsRuntime.RegisterServices(services);

            serviceProvider = services.BuildServiceProvider();
        }

        async Task InitializeAsync()
        {
            
            Console.WriteLine("Enter your plugin bin directory (leave empty for current): ");
            string input = Console.ReadLine();

            string dir = null;
            if (string.IsNullOrEmpty(input))
            {
                dir = Directory.GetCurrentDirectory();
            } else
            {
                dir = input;
            }

            Console.WriteLine("Enter your plugin name: ");
            
            var args = new ExecuteReportPluginArguments();
            args.PluginName = Console.ReadLine().Replace(".dll", string.Empty);
            args.PluginData = await File.ReadAllBytesAsync(Path.Combine(dir, args.PluginName + ".dll"));

            //var parametersType = assembly.GetTypes().FirstOrDefault(x => x.GetCustomAttribute<ParametersAttribute>() != null);

            //var jObject = new JObject();
            //if (parametersType != null)
            //{
            //    Console.WriteLine("Enter parameters values");
            //    foreach (var property in parametersType.GetProperties())
            //    {
            //        Console.Write($"{property.Name}:");
            //        jObject.Add(property.Name, Console.ReadLine());
            //        Console.WriteLine();
            //    }
            //}

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("hello");

            string parametersPath = Path.Combine(dir, "parameters.json");
            if (File.Exists(parametersPath))
            {
                args.JsonParameters = File.ReadAllText(parametersPath);
            }
            Console.WriteLine("hi1");
            var template = Directory.GetFiles(dir).FirstOrDefault(x => x.StartsWith("template"));
            if (template != null)
            {
                var file = new FileInfo(template);
                args.ReportTemplate = new ReportTemplate()
                {
                    FileName = file.Name,
                    MimeType = MimeTypes.GetMimeType(file.Name),
                    Data = await File.ReadAllBytesAsync(file.FullName)
                };
            }

            Console.WriteLine($"hi2 {args.JsonParameters == null}");
            Console.WriteLine(args.JsonParameters);

            List<byte[]> librariesData = new List<byte[]>();
            foreach (var filePath in Directory.GetFiles(dir, "*.dll"))
            {
                librariesData.Add(await File.ReadAllBytesAsync(filePath));
            }

            args.LibrariesData = librariesData;

            Console.WriteLine("hi3");
            var pluginManager = serviceProvider.GetRequiredService<IReportPluginManager>();

            Console.WriteLine($"hi4 {pluginManager == null}");
            var result = await pluginManager.ExecuteReportPluginAsync(args);
            Console.WriteLine("hi5");

            Console.WriteLine("================== RESULT ==================");
            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully generated report");
                await File.WriteAllBytesAsync(result.ReportFile.Name, result.ReportFile.Data);
                Console.WriteLine($"Report file saved to {result.ReportFile.Name}!");
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to generate report");
                Console.WriteLine($"Message: {result.Message}");
                await File.WriteAllTextAsync("exception.log", result.Exception.ToString());
                Console.WriteLine($"Exception saved to exception.log file");
                Console.WriteLine(result.Exception);
                
            }

            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
