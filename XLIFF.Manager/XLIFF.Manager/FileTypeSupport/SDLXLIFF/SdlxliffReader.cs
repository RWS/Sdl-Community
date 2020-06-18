using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly List<FilterItem> _excludeFilterItems;

		public SdlxliffReader(SegmentBuilder segmentBuilder, List<FilterItem> excludeFilterItems)
		{
			_segmentBuilder = segmentBuilder;
			_excludeFilterItems = excludeFilterItems;
		}

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public Xliff ReadFile(string projectId, string filePath, bool copySourceToTarget)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentReader = new ContentReader(projectId, filePath, false, _segmentBuilder, copySourceToTarget, _excludeFilterItems);		
			converter.AddBilingualProcessor(contentReader);
			
			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;			

			converter.Parse();
			return contentReader.Xliff;
		}
	}
}
