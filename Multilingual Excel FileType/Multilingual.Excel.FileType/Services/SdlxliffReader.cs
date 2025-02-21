using System.Collections.Generic;
using System.IO;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Multilingual.Excel.FileType.Services
{
	public class SdlxliffReader
	{
		public List<ParagraphUnitInfo> GetParagraphUnitInfos(string filePathInput)
		{
			var contentReader = ParseFile(filePathInput);

			return contentReader.ParagraphUnitInfos;
		}

		private ContentReader ParseFile(string filePathInput)
		{
			var dummyFile = Path.GetTempFileName();

			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, dummyFile, null);

			var contentReader = new ContentReader();

			converter.AddBilingualProcessor(contentReader);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			DeleteFile(dummyFile);
			return contentReader;
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
