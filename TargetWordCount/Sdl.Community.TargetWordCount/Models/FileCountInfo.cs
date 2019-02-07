using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.TargetWordCount.Models
{
	public sealed class FileCountInfo
    {
        public FileCountInfo(List<SegmentCountInfo> segmentCounts, Language[] languages, IRepetitionsTable repTable)
        {
            Contract.Requires<ArgumentNullException>(segmentCounts != null);
            Contract.Requires<ArgumentNullException>(languages != null);
            Contract.Requires<ArgumentNullException>(languages.Length > 1);
            Contract.Requires<ArgumentNullException>(repTable != null);

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