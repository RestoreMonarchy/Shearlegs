using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins
{
    public class ReportPluginManager : IReportPluginManager
    {
        private readonly ILogger<ReportPluginManager> logger;
        private readonly IPluginManager pluginManager;

        public ReportPluginManager(ILogger<ReportPluginManager> logger, IPluginManager pluginManager)
        {
            this.logger = logger;
            this.pluginManager = pluginManager;
        }

        public async Task<ExecuteReportPluginResult> ExecuteReportPluginAsync(ExecuteReportPluginArguments args)
        {
            var context = new SimpleAssemblyLoadContext();

            foreach (var libraryData in args.LibrariesData)
                context.LoadFromStream(new MemoryStream(libraryData));

            var pluginAssembly = context.LoadFromStream(new MemoryStream(args.PluginData));

            void AddServices(IServiceCollection services)
            {
                if (args.ReportTemplate != null)
                    services.AddSingleton(args.ReportTemplate);
            }

            var plugin = pluginManager.ActivatePlugin(pluginAssembly, args.JsonParameters, AddServices) as IReportPlugin;

            var result = new ExecuteReportPluginResult();
            if (plugin == null)
            {
                result.IsSuccess = false;
                result.Message = $"{args.PluginName} is not a valid IReportPlugin!";
                logger.LogInformation(result.Message);
            }
            else
            {
                try
                {
                    result.ReportFile = await plugin.GenerateReportAsync();
                    result.IsSuccess = true;
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.Message = $"Failed to generate report for plugin {plugin.Name}";
                    result.Exception = e;
                    logger.LogError(e, result.Message);
                }
            }

            context.Unload();

            return result;
        }
    }
}
