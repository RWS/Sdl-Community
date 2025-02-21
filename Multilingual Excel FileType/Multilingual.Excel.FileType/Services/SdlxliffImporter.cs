using System;
using System.Collections.Generic;
using System.IO;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Multilingual.Excel.FileType.Services
{
	public class SdlxliffImporter
	{
		private readonly FilterItemService _filterItemService;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly SegmentVisitor _segmentVisitor;

		public SdlxliffImporter(FilterItemService filterItemService, SegmentBuilder segmentBuilder, SegmentVisitor segmentVisitor)
		{
			_filterItemService = filterItemService;
			_segmentBuilder = segmentBuilder;
			_segmentVisitor = segmentVisitor;
		}

		public ImportResult UpdateFile(List<IParagraphUnit> updatedParagraphUnits, MultilingualExcelImportSettings settings, string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			var contentWriter = new ContentImporter(updatedParagraphUnits, settings, _filterItemService, _segmentBuilder, _segmentVisitor);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return new ImportResult
			{
				Success = true,
				UpdatedSegments = contentWriter.UpdatedSegments,
				ExcludedSegments = contentWriter.ExcludedSegments,
				FilePath = filePathInput,
				UpdatedFilePath = filePathOutput,
				BackupFilePath = GetUniqueFileName(filePathInput, "Backup")
			};
		}

		private string GetUniqueFileName(string filePath, string suffix)
		{
			var directoryName = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			var fileExtension = Path.GetExtension(fileName);
			var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName, fileExtension);

			var index = 1;
			var uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
															 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
															 + index.ToString().PadLeft(4, '0') + fileExtension);

			if (File.Exists(uniqueFilePath))
			{
				while (File.Exists(uniqueFilePath))
				{
					index++;
					uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
																 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
																 + index.ToString().PadLeft(4, '0') + fileExtension);
				}
			}

			return uniqueFilePath;
		}

		private string GetFileNameWithoutExtension(string fileName, string extension)
		{
			if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
			{
				return fileName;
			}

			if (extension.Length > fileName.Length || !fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return fileName;
			}

			return fileName.Substring(0, fileName.Length - extension.Length);
		}
	}
}
