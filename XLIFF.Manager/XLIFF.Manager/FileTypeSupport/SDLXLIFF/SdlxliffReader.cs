using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly List<string> _excludeFilterItems;
		private readonly ExportOptions _exportOptions;
		private readonly IMultiFileConverter _multiFileConverter;

		public SdlxliffReader(SegmentBuilder segmentBuilder, List<string> excludeFilterItems, ExportOptions exportOptions)
		{
			_segmentBuilder = segmentBuilder;
			_excludeFilterItems = excludeFilterItems;
			_exportOptions = exportOptions;
		}

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public Xliff ReadFile(string projectId, string filePath)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentReader = new ContentReader(projectId, filePath, false, _segmentBuilder, _excludeFilterItems, _exportOptions);		
			converter.AddBilingualProcessor(contentReader);
			
			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;			

			converter.Parse();
			return contentReader.Xliff;
		}
	}
}
