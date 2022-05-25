using Sdl.Community.MTEdge.Provider.XliffConverter.SegmentParser;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTEdge.UnitTests
{
	public static class TestTags
    {
        static TestTags()
        {
            BasicSegment = new Segment();
            BasicSegment.Add(UTStringResource.BasicText);
            TaggedSegment = Parser.ParseLine(UTStringResource.TaggedText);
        }

        public static Segment BasicSegment { get; private set; }
        public static Segment TaggedSegment { get; private set; }
    }
}