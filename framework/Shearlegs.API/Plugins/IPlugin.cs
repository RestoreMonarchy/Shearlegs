using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        Assembly Assembly { get; }        
    }
}
