using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public ShearlegsRuntime Runtime { get; private set; }

        static void Main(string[] args)
        {
            Instance = new Program();
            Instance.MainAsync().GetAwaiter().GetResult();
        }

        void ConfigureServices(IServiceCollection services)
        {
            ShearlegsRuntime.RegisterServices(services);
            services.AddHostedService<ShearlegsRuntime>();
        }

        async Task MainAsync()
        {
            IHostBuilder hostBuilder = new HostBuilder();
            await hostBuilder.ConfigureServices(ConfigureServices).RunConsoleAsync();
        }
    }
}
