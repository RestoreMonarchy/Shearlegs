using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Helpers
{
    internal class SimpleAssemblyLoadContext : AssemblyLoadContext
    {
        internal SimpleAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName) => null;
    }
}
