using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.MTCloud.Provider.Model
{
    public class BeGlobalSegment
    {
	    public int Index { get; set; }
	    public Segment Segment { get; set; }
	    public Segment Translation { get; set; }
	    public SearchResult SearchResult { get; set; }
    }
}
