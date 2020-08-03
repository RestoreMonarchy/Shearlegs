using FileTemplates.API.Logging;
using FileTemplates.API.Plugins;
using FileTemplates.Core.Logging;
using FileTemplates.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FileTemplates.Runtime
{
    public class Runtime
    {
        private ServiceCollection services;

        public IPluginManager PluginManager { get; private set; }
        public IPluginLibrariesManager PluginLibrariesManager { get; private set; }

        public async Task StartAsync()
        {
            services = new ServiceCollection();
            services.AddSingleton<IPluginManager, PluginManager>();
            services.AddSingleton<IPluginLibrariesManager, PluginLibrariesManager>();
            services.AddTransient<ILogger, Logger>();

            await InitializeAsync(services.BuildServiceProvider());
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            PluginManager = serviceProvider.GetRequiredService<IPluginManager>();
            PluginLibrariesManager = serviceProvider.GetRequiredService<IPluginLibrariesManager>();

            await PluginLibrariesManager.LoadLibrariesAsync("Libraries");
            await PluginManager.LoadPluginsAsync("Plugins");
        }
        
        public async Task StopAsync()
        {
            await PluginManager.UnloadPluginsAsync();
        }
    }
}
