using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.Jobs.Model
{
    public class JobViewModel
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string TimePosted { get; set; }

        public string LanguagePairs { get; set; }

        public string Volume { get; set; }

        public Discipline Discipline { get; set; }
        
        public string OtherDiscipline { get; set; }

        public string LanguageServices { get; set; }

        public string Next { get; set; }

        public string Previous { get; set; }
    }
}
