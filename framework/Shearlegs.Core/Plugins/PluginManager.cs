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
using System.Runtime.Loader;
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

        private class SimpleAssemblyLoadContext : AssemblyLoadContext 
        {
            internal SimpleAssemblyLoadContext() : base(isCollectible: true)
            {
            }

            protected override Assembly Load(AssemblyName assemblyName) => null;
        }


        public async Task<IReportFile> ExecuteReportPluginAsync(string pluginName, string jsonParameters, byte[] pluginData, ITemplate template, IEnumerable<byte[]> libraries)
        {
            var context = new SimpleAssemblyLoadContext();
            
            foreach (var libraryData in libraries)
                context.LoadFromStream(new MemoryStream(libraryData));

            var pluginAssembly = context.LoadFromStream(new MemoryStream(pluginData));
            var plugin = await ActivatePluginAsync(pluginAssembly, jsonParameters, template) as IReportPlugin;

            IReportFile reportFile = null;
            try
            {
                reportFile = await plugin.GenerateReportAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e);
            }

            context.Unload();
            return reportFile;
        }

        private async Task<IPlugin> ActivatePluginAsync(Assembly assembly, string jsonParameters, ITemplate template)
        {
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);

            if (pluginType == null)
            {
                await logger.LogInformationAsync($"{assembly.GetName().Name} is not valid plugin assembly!");
                return null;
            }

            var services = assembly.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);
            var parameters = assembly.GetTypes().FirstOrDefault(x => x.GetCustomAttribute<ParametersAttribute>() != null);

            // Add plugin as singleton service
            IReportPlugin pluginInstance;
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(pluginType);

            // Add logger service with ISession
            serviceCollection.AddSingleton(session);
            serviceCollection.AddTransient<ILogger, Logger>();

            serviceCollection.AddSingleton(template);

            // Add plugin custom services
            foreach (var service in services)
            {
                serviceCollection.Add(new ServiceDescriptor(service.GetType(), service.GetType(),
                    service.GetCustomAttribute<ServiceAttribute>().Lifetime));
            }

            serviceCollection.Add(new ServiceDescriptor(parameters, JsonConvert.DeserializeObject(jsonParameters, parameters)));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            pluginInstance = serviceProvider.GetRequiredService(pluginType) as IReportPlugin;

            return pluginInstance;
        }
    }
}