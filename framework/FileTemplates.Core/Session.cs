using FileTemplates.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTemplates.Core
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
