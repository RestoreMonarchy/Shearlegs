using Autofac;
using FileTemplates.API.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FileTemplates.ConsoleDemo
{
    class Program
    {
        public static Program Instance { get; private set; }
        public Runtime.Runtime Runtime { get; private set; }
        
        private ILogger Logger { get; set; }

        static void Main(string[] args)
        {
            Instance = new Program();
            Instance.Runtime = new Runtime.Runtime();
            Instance.MainAsync().GetAwaiter().GetResult();            
        }

        async Task MainAsync()
        {
            await Runtime.InitializeAsync();

            Logger = Runtime.Container.Resolve<ILogger>();
            Runtime.PluginManager.OnPluginActivated += async (plugin) =>
            {
                await Logger.LogAsync($"{plugin.Name} {plugin.Version} has been loaded!");
            };

            Runtime.PluginManager.OnPluginDeactivated += async (plugin) =>
            {
                await Logger.LogAsync($"{plugin.Name} {plugin.Version} has been deactivated!");
            };

            await Runtime.StartAsync();

            await Logger.LogAsync($"Loaded Plugins: {Runtime.PluginManager.LoadedPlugins.Count()}");
            await Logger.LogAsync($"Activated Plugins: {Runtime.PluginManager.ActivatedPlugins.Count()}");

            await Task.Delay(-1);
        }
    }
}
