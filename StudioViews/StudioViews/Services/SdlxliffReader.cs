using System.Collections.Generic;
using System.IO;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class SdlxliffReader
	{
		public List<SegmentPairInfo> GetSegmentPairs(string filePathInput, bool ignoreWordCountInfo)
		{
			var dummyFile = Path.GetTempFileName();
			
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, dummyFile, null);
			
			var contentReader = new ContentReader
			{
				IgnoreWordCountInfo = ignoreWordCountInfo
			};

			converter.AddBilingualProcessor(contentReader);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			DeleteFile(dummyFile);

			return contentReader.SegmentPairInfos;
		}

		private bool DeleteFile(string dummyFile)
		{
			try
			{
				if (File.Exists(dummyFile))
				{
					File.Delete(dummyFile);

					return true;
				}
			}
			catch
			{
				// ignore; catch all
			}

			return false;
		}
	}
}
