using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StudioIntegrationApiSample
{
    public class ProjectRequest
    {
        public string Name
        {
            get;
            set;
        }

        public string[] Files
        {
            get; set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
