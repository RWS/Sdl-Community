using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.MTCloud.Provider.Model
{
    public class MTCloudSegment
    {
	    public long Index { get; set; }
	    public Segment Segment { get; set; }
	    public Segment Translation { get; set; }
	    public SearchResult SearchResult { get; set; }
    }
}
