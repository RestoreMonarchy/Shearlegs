using Autofac;
using FileTemplates.API.Logging;
using FileTemplates.API.Plugins;
using FileTemplates.Core.Logging;
using FileTemplates.Core.Plugins;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileTemplates.Runtime
{
    public class Runtime
    {
        public IContainer Container { get; private set; }

        public IPluginManager PluginManager { get; private set; }
        public IPluginLibrariesManager PluginLibrariesManager { get; private set; }
        
        public IServiceProvider ServiceProvider { get; private set; }

        public async Task InitializeAsync()
        {
            Container = ConfigureContainer();
            Directory.CreateDirectory("Libraries");
            Directory.CreateDirectory("Plugins");

            PluginManager = Container.Resolve<IPluginManager>();
            PluginLibrariesManager = Container.Resolve<IPluginLibrariesManager>();
        }

        private IContainer ConfigureContainer()
        {
            ContainerBuilder cb = new ContainerBuilder();
            cb.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            cb.RegisterType<PluginLibrariesManager>().As<IPluginLibrariesManager>().SingleInstance();
            cb.RegisterType<Logger>().As<ILogger>().InstancePerDependency();

            return cb.Build();
        }

        public async Task StartAsync()
        {
            await PluginLibrariesManager.LoadLibrariesAsync("Libraries");
            await PluginManager.LoadPluginsAsync("Plugins");
        }
        
        public async Task StopAsync()
        {
            await PluginManager.UnloadPluginsAsync();
        }
    }
}
