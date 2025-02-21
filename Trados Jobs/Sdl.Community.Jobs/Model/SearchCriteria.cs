namespace Sdl.Community.Jobs.Model
{
    public class SearchCriteria
    {

        public LanguagePair LanguagePair { get; set; }

        public Discipline Discipline { get; set; }

        public string SearchTerm { get; set; }

        public bool Translation { get; set; }

        public bool Interpreting { get; set; }

        public bool Potential { get; set; }
    }
}
