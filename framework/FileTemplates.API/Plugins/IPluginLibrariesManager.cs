using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileTemplates.API.Plugins
{
    public interface IPluginLibrariesManager
    {
        IEnumerable<Assembly> LoadedLibraries { get; }
        Task LoadLibrariesAsync();
    }
}
