using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.TargetWordCount.Models
{
	public class SegmentCountInfo
    {
        public SegmentCountInfo(ITranslationOrigin translationOrigin, CountData countData, bool isLocked, int spaceCount)
        {
            TranslationOrigin = translationOrigin;
            CountData = countData;
            IsLocked = isLocked;
            SpaceCount = spaceCount;
        }

        public CountData CountData { get; }

        public bool IsLocked { get; }

        public ITranslationOrigin TranslationOrigin { get; }

        public int SpaceCount { get; }
    }
}