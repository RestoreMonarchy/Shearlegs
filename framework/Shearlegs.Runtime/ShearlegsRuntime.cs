using Microsoft.Extensions.DependencyInjection;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Plugins;
using Shearlegs.Core.Reports;
using System;

namespace Shearlegs.Runtime
{
    public class ShearlegsRuntime
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPluginManager, PluginManager>();
            serviceCollection.AddSingleton<IReportPluginManager, ReportPluginManager>();
        }

        public static IServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            RegisterServices(services);
            return services.BuildServiceProvider();
        }
    }
}
