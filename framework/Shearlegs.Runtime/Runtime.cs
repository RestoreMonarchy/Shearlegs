using Autofac;
using Shearlegs.API;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.Core;
using Shearlegs.Core.Constants;
using Shearlegs.Core.Logging;
using Shearlegs.Core.Plugins;
using System.IO;
using System.Threading.Tasks;

namespace Shearlegs.Runtime
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
