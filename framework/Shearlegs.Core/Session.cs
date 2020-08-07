using Shearlegs.API;
using System;

namespace Shearlegs.Core
{
    public class Session : ISession
    {
        public string ID { get; set; }

        public Session()
        {
            ID = Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
