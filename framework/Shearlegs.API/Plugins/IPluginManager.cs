using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins
{
    public interface IPluginManager
    {
        IPlugin ActivatePlugin(Assembly assembly, string jsonParameters, Action<IServiceCollection> action);
    }
}
