using System.Collections.Generic;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class SdlxliffExporter
	{
		public bool ExportFile(List<SegmentPairInfo> selectedSegments, string filePathInput, string filePathOutput)
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
