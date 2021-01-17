using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shearlegs.API;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Delegates;
using Shearlegs.API.Reports;
using Shearlegs.Core.Constants;
using Shearlegs.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
{
    public class PluginManager : IPluginManager
    {
        private readonly ILogger logger;
        private readonly ISession session;

        public PluginManager(ILogger logger, ISession session)
        {
            this.logger = logger;
            this.session = session;
        }

        public async Task<IReportFile> ExecuteReportPluginAsync(string pluginName, string jsonParameters, byte[] pluginData, IEnumerable<byte[]> libraries)
        { 
            var appDomain = AppDomain.CreateDomain(pluginName);

            foreach (var libraryData in libraries)
                appDomain.Load(libraryData);

            var pluginAssembly = appDomain.Load(pluginData);
            var plugin = await ActivatePluginAsync(pluginAssembly, jsonParameters) as IReportPlugin<object>;

            IReportFile reportFile = null;
            try
            {
                reportFile = await plugin.GenerateReportAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e);
            }

            AppDomain.Unload(appDomain);
            return reportFile;
        }

        private async Task<IPlugin> ActivatePluginAsync(Assembly assembly, string jsonParameters)
        {
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);

            if (pluginType == null)
            {
                await logger.LogInformationAsync($"{assembly.GetName().Name} is not valid plugin assembly!");
                return null;
            }

            var services = assembly.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

            // Add plugin as singleton service
            IPlugin pluginInstance;
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(pluginType);

            // Add logger service with ISession
            serviceCollection.AddSingleton(session);
            serviceCollection.AddTransient<ILogger, Logger>();            
            
            // Add plugin custom services
            foreach (var service in services)
            {
                serviceCollection.Add(new ServiceDescriptor(service.GetType(), service.GetType(),
                    service.GetCustomAttribute<ServiceAttribute>().Lifetime));
            }

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            pluginInstance = serviceProvider.GetRequiredService(pluginType) as IPlugin;
            
            UpdatePluginParameters(pluginInstance, jsonParameters);

            return pluginInstance;
        }

        private void UpdatePluginParameters(IPlugin pluginInstance, string jsonString)
        {
            var plugin = pluginInstance as IReportPlugin<object>;

            var parameters = JsonConvert.DeserializeObject(jsonString, plugin.Parameters.GetType());

            plugin.GetType().GetProperty("Parameters", BindingFlags.Public | BindingFlags.Instance).SetValue(plugin, parameters);
        }
    }
}