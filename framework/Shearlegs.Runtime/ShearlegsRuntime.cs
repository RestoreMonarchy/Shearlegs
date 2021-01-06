using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shearlegs.API;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.Core;
using Shearlegs.Core.Constants;
using Shearlegs.Core.Logging;
using Shearlegs.Core.Reports;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shearlegs.Runtime
{
    public class ShearlegsRuntime : IHostedService
    {
        private readonly IPluginManager pluginManager;
        private readonly IPluginLibrariesManager pluginLibrariesManager;

        public ShearlegsRuntime(IPluginManager pluginManager, IPluginLibrariesManager pluginLibrariesManager)
        {
            this.pluginManager = pluginManager;
            this.pluginLibrariesManager = pluginLibrariesManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(DirectoryConstants.LibrariesDirectory);
            Directory.CreateDirectory(DirectoryConstants.PluginsDirectory);
            Directory.CreateDirectory(DirectoryConstants.LogsDirectory);

            await pluginLibrariesManager.LoadLibrariesAsync();
            await pluginManager.LoadPluginsAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await pluginManager.DeactivatePluginsAsync();
        }

        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISession, Session>();
            serviceCollection.AddSingleton<IPluginManager, PluginManager>();
            serviceCollection.AddSingleton<IPluginLibrariesManager, PluginLibrariesManager>();
            serviceCollection.AddTransient<ILogger, Logger>();
        }
    }
}
