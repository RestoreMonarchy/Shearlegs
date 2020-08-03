using FileTemplates.API.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileTemplates.Core.Plugins
{
    public class PluginLibrariesManager : IPluginLibrariesManager
    {
        private readonly List<Assembly> loadedLibraries;

        public PluginLibrariesManager()
        {
            loadedLibraries = new List<Assembly>();
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) => LoadedLibraries.FirstOrDefault(x => args.Name == x.FullName);

        public IEnumerable<Assembly> LoadedLibraries => loadedLibraries;

        public Task LoadLibrariesAsync(string directory)
        {
            IEnumerable<FileInfo> libraryFiles = new DirectoryInfo(directory).GetFiles(".dll", SearchOption.AllDirectories);

            foreach (var file in libraryFiles)
            {
                loadedLibraries.Add(Assembly.LoadFile(file.FullName));
            }
            return Task.CompletedTask;
        }
    }
}
