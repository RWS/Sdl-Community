using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Builds <see cref="Segment"/>s in memory for unit tests — no files, no network, no live API.
    /// Tags are created with explicit anchors via <c>new Tag(TagType, tagId, anchor)</c> (the SDL
    /// constructor; paired Start/End tags share an anchor, self-closing tags get their own). This
    /// mirrors exactly what <see cref="LanguageWeaverProvider.Services.SegmentSerializer"/> keys on
    /// when it emits <c>&lt;g id="anchor"&gt;</c>/<c>&lt;x id="anchor"&gt;</c> markup.
    /// </summary>
    public sealed class TestSegmentBuilder
    {
        private readonly Segment _segment = new();

        public static TestSegmentBuilder New() => new();

        /// <summary>Appends a run of plain text.</summary>
        public TestSegmentBuilder Text(string value)
        {
            _segment.Add(value);
            return this;
        }

        /// <summary>Appends a paired Start tag (<c>&lt;g&gt;</c> open) with the given anchor.</summary>
        public TestSegmentBuilder StartTag(int anchor)
        {
            _segment.Add(new Tag(TagType.Start, anchor.ToString(), anchor));
            return this;
        }

        /// <summary>Appends a paired End tag (<c>&lt;g&gt;</c> close) matching <paramref name="anchor"/>.</summary>
        public TestSegmentBuilder EndTag(int anchor)
        {
            _segment.Add(new Tag(TagType.End, anchor.ToString(), anchor));
            return this;
        }

        /// <summary>Appends a self-closing placeholder tag (<c>&lt;x&gt;</c>) with the given anchor.</summary>
        public TestSegmentBuilder Placeholder(int anchor)
        {
            _segment.Add(new Tag(TagType.Standalone, anchor.ToString(), anchor));
            return this;
        }

        /// <summary>Wraps <paramref name="value"/> in a paired Start/End tag sharing <paramref name="anchor"/>.</summary>
        public TestSegmentBuilder Paired(int anchor, string value)
            => StartTag(anchor).Text(value).EndTag(anchor);

        public Segment Build() => _segment;
    }
}
