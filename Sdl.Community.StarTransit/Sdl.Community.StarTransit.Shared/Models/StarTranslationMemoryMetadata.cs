using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class StarTranslationMemoryMetadata
    {
        public Guid Id { get; set; }
        public string SourceFile { get; set; }
        public string TargetFile { get; set; }
    }
}
