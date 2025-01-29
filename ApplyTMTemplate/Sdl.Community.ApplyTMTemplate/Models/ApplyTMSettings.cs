using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ApplyTMTemplate.Models
{
    public class ApplyTMSettings
    {
        public string LanguageResourcePath { get; set; } = string.Empty;

        public List<string> TMPathCollection { get; set; } = new();

        public Settings Settings { get; set; } = new Settings();
    }
}
