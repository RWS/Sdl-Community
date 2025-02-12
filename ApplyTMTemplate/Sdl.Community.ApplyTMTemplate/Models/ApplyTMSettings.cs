using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ApplyTMTemplate.Models
{
    public class ApplyTMSettings
    {
        public string TMSettingsFilePath { get; set; }

        public string LanguageResourcePath { get; set; } = string.Empty;

        public List<TranslationMemoryEntry> TMPathCollection { get; set; } = new();

        public ApplyTMMethod SelectedMethod { get; set; }

        public Settings Settings { get; set; } = new Settings();
    }
}
