using Autofac;
using FileTemplates.API;
using FileTemplates.API.Logging;
using FileTemplates.API.Plugins;
using FileTemplates.Core;
using FileTemplates.Core.Constants;
using FileTemplates.Core.Logging;
using FileTemplates.Core.Plugins;
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

        public Task InitializeAsync()
        {
            Directory.CreateDirectory(DirectoryConstants.LibrariesDirectory);
            Directory.CreateDirectory(DirectoryConstants.PluginsDirectory);
            Directory.CreateDirectory(DirectoryConstants.LogsDirectory);

            Container = ConfigureContainer();

            PluginManager = Container.Resolve<IPluginManager>();
            PluginLibrariesManager = Container.Resolve<IPluginLibrariesManager>();
            return Task.CompletedTask;
        }

        private IContainer ConfigureContainer()
        {
            ContainerBuilder cb = new ContainerBuilder();
            cb.RegisterType<Session>().As<ISession>().SingleInstance();
            cb.RegisterType<PluginManager>().As<IPluginManager>().SingleInstance();
            cb.RegisterType<PluginLibrariesManager>().As<IPluginLibrariesManager>().SingleInstance();
            cb.RegisterType<Logger>().As<ILogger>().InstancePerDependency();

            return cb.Build();
        }

        public async Task StartAsync()
        {
            await PluginLibrariesManager.LoadLibrariesAsync();
            await PluginManager.LoadPluginsAsync();
        }
        
        public async Task StopAsync()
        {
            await PluginManager.DeactivatePluginsAsync();
        }
    }
}
