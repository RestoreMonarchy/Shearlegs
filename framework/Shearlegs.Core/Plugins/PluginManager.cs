using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Shearlegs.Core.Reports
{
    public class PluginManager : IPluginManager
    {
        private readonly ILogger<PluginManager> logger;

        public PluginManager(ILogger<PluginManager> logger)
        {
            this.logger = logger;
        }

        public T ActivatePlugin<T>(Assembly assembly, string jsonParameters, Action<IServiceCollection> action) where T : IPlugin
        {
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);

            if (pluginType == null)
            {
                logger.LogWarning($"{assembly.GetName().Name} is not valid plugin assembly!");
                return default;
            }

            var services = assembly.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);
            var parameters = assembly.GetTypes().FirstOrDefault(x => x.GetCustomAttribute<ParametersAttribute>() != null);

            // Add plugin as singleton service
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(pluginType);

            // TODO: Add logger

            // Add custom plugin services
            action.Invoke(serviceCollection);

            // Add plugin custom services
            foreach (var service in services)
            {
                serviceCollection.Add(new ServiceDescriptor(service.GetType(), service.GetType(),
                    service.GetCustomAttribute<ServiceAttribute>().Lifetime));
            }

            if (parameters != null)
                serviceCollection.Add(new ServiceDescriptor(parameters, JsonConvert.DeserializeObject(jsonParameters, parameters)));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return (T)serviceProvider.GetRequiredService(pluginType);
        }
    }
}