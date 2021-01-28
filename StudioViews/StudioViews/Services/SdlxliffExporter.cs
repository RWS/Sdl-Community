using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class SdlxliffExporter
	{
		public OutputFile ExportFile(List<SegmentPairInfo> selectedSegments, string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			var segmentBuilder = new SegmentBuilder();
			var contentWriter = new ContentExporter(selectedSegments, segmentBuilder);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();


			var outputFile =new OutputFile
			{
				FilePath = filePathOutput,
				SegmentCount = contentWriter.SegmentPairInfos.Count,
				WordCount = contentWriter.SegmentPairInfos.Sum(a => a.SourceWordCounts.Words)
			};
			

			return outputFile;
		}
	}
}
