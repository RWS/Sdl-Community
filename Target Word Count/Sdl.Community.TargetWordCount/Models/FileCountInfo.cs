using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.TargetWordCount.Models
{
	public sealed class FileCountInfo
    {
        public FileCountInfo(List<SegmentCountInfo> segmentCounts, Language[] languages, IRepetitionsTable repTable)
        {
            SegmentCounts = segmentCounts;

            SourceInfo = languages[0];
            TargetInfo = languages[1];

            RepetitionsTable = repTable;
        }

        public IRepetitionsTable RepetitionsTable { get; }

        public List<SegmentCountInfo> SegmentCounts { get; }

        public Language SourceInfo { get; }

        public Language TargetInfo { get; }
    }
}