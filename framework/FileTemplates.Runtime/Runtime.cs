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

        public async Task StartAsync()
        {
            services = new ServiceCollection();
            services.AddSingleton<IPluginManager, PluginManager>();
            services.AddTransient<ILogger, Logger>();

            await InitializeAsync(services.BuildServiceProvider());
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<IPluginManager>();
        }
        
        public async Task StopAsync()
        {

        }
    }
}
