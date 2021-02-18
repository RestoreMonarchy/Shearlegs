using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Plugins.Reports;
using Shearlegs.Core.Reports;
using Shearlegs.Runtime;
using SimpleQuery;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleQueryTest
{
    [TestClass]
    public class SimpleQueryPluginTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            System.Console.WriteLine($"test started at {DateTime.Now}");

            var serviceProvider = ShearlegsRuntime.BuildServiceProvider();

            string jsonParameters = null;
            
            if (File.Exists("parameters.json"))
            {
                jsonParameters = File.ReadAllText("parameters.json");
            }

            IReportTemplate template = null;
            if (File.Exists("template.xlsx"))
            {
                template = new ReportTemplate() 
                { 
                    FileName = "template.xlsx",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Data = File.ReadAllBytes("template.xlsx")
                };
            }

            var pluginManager = serviceProvider.GetRequiredService<IPluginManager>();

            var instance = pluginManager.ActivatePlugin<IReportPlugin>(typeof(SimpleQueryPlugin).Assembly, jsonParameters, 
                (sp) => 
                {
                    if (template != null)
                        sp.AddSingleton(template);
                });

            var reportFile = await instance.GenerateReportAsync();
            if (reportFile != null)
            {
                await File.WriteAllBytesAsync(reportFile.Name, reportFile.Data);
                System.Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), reportFile.Name));
            }

        }
    }
}
