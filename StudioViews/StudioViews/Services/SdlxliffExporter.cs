using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using StudioViews.Model;

namespace StudioViews.Services
{
	public class SdlxliffExporter
	{
		public bool ExportFile(List<SegmentPairContext> selectedSegments, string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			var segmentBuilder = new SegmentBuilder();
			var contentWriter = new ContentExporter(selectedSegments, segmentBuilder);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();
				
			return true;
		}
	}
}
