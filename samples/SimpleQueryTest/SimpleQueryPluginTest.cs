using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Reports;
using Shearlegs.Runtime;
using SimpleQuery;
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
            System.Console.WriteLine("hello? noway");
            var serviceProvider = ShearlegsRuntime.BuildServiceProvider();

            string jsonParameters = null;
            
            if (File.Exists("parameters.json"))
            {
                jsonParameters = File.ReadAllText("parameters.json");
            }

            var pluginManager = serviceProvider.GetRequiredService<IPluginManager>();

            var instance = pluginManager.ActivatePlugin(typeof(SimpleQueryPlugin).Assembly, jsonParameters, (_) => { }) as IReportPlugin;

            var reportFile = await instance.GenerateReportAsync();
            if (reportFile != null)
            {
                await File.WriteAllBytesAsync(reportFile.Name, reportFile.Data);
                System.Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), reportFile.Name));
            }

        }
    }
}
