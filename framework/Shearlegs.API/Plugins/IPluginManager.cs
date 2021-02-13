using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins
{
    public interface IPluginManager
    {
        T ActivatePlugin<T>(Assembly assembly, string jsonParameters, Action<IServiceCollection> action) where T : IPlugin;
    }
}
