using Microsoft.Extensions.DependencyInjection;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
{
    public abstract class PluginBase : IPlugin
    {
        public PluginBase()
        {
            Assembly = GetType().Assembly;
            Name = GetType().Name;
            Version = Assembly.GetName().Version.ToString();
        }

        public virtual string Name { get; }
        public virtual string Version { get; }
        public Assembly Assembly { get; }

        public virtual Task ActivateAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DeactivateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
