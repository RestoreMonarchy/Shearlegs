using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace FileTemplates.API.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        Assembly Assembly { get; }

        Task LoadAsync();
        Task UnloadAsync();
    }
}
