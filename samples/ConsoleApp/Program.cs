using Microsoft.Extensions.DependencyInjection;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.Runtime;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        public static Program Instance { get; private set; }
        public Runtime Runtime { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }


        private ILogger Logger { get; set; }

        static void Main(string[] args)
        {
            Instance = new Program();
            Instance.MainAsync().GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            IServiceCollection services = new ServiceCollection();
            Runtime.RegisterServices(services);
            services.AddSingleton<Runtime>();
            ServiceProvider = services.BuildServiceProvider();

            Runtime = ServiceProvider.GetRequiredService<Runtime>();
            Logger = ServiceProvider.GetRequiredService<ILogger>();
            var pluginManager = ServiceProvider.GetRequiredService<IPluginManager>();

            pluginManager.OnPluginActivated += async (plugin) =>
            {
                await Logger.LogAsync($"{plugin.Name} {plugin.Version} has been loaded!");
            };

            pluginManager.OnPluginDeactivated += async (plugin) =>
            {
                await Logger.LogAsync($"{plugin.Name} {plugin.Version} has been deactivated!");
            };

            await Runtime.StartAsync(CancellationToken.None);

            await Logger.LogAsync($"Loaded Plugins: {pluginManager.LoadedPlugins.Count()}");
            await Logger.LogAsync($"Activated Plugins: {pluginManager.ActivatedPlugins.Count()}");

            await Task.Delay(-1);
        }
    }
}
