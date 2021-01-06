using Shearlegs.API.Plugins;
using Shearlegs.Core.Translations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        protected IServiceProvider serviceProvider;

        public PluginBase(IServiceProvider serviceProvider)
        {
            Assembly = GetType().Assembly;
            Name = GetType().Name;
            Version = Assembly.GetName().Version.ToString();

            this.serviceProvider = serviceProvider;
        }

        public virtual string Name { get; }
        public virtual string Version { get; }
        public Assembly Assembly { get; }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
