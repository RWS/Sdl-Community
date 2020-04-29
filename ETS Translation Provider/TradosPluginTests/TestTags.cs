using Sdl.Community.Toolkit.LanguagePlatform.SegmentParser;
using Sdl.LanguagePlatform.Core;

namespace TradosPluginTests
{
	public static class TestTags
    {
        static TestTags()
        {
            BasicSegment = new Segment();
            BasicSegment.Add(StringResource.BasicText);
            TaggedSegment = Parser.ParseLine(StringResource.TaggedText);
        }

        public static Segment BasicSegment { get; private set; }
        public static Segment TaggedSegment { get; private set; }
    }
}