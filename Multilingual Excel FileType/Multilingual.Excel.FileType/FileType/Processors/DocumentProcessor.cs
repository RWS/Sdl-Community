using System;
using System.Collections.Generic;
using System.Linq;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class DocumentProcessor
	{
		private readonly EditorController _editorController;
		private readonly SegmentBuilder _segmentBuilder;

		public DocumentProcessor(EditorController editorController, SegmentBuilder segmentBuilder)
		{
			_editorController = editorController;
			_segmentBuilder = segmentBuilder;
			
			editorController.Opening += EditorController_Opening;
		}

		private class ParagraphUnit
		{
			public string ParagraphUnitId { get; set; }

			public List<ISegmentPair> SegmentPairs { get; set; }

			public IParagraphUnitProperties ParagraphUnitProperties { get; set; }
		}

		private void EditorController_Opening(object sender, CancelDocumentEventArgs e)
		{
			if (e.Document == null)
			{
				return;
			}

			var project = e.Document.Project;
			var projectInfo = project.GetProjectInfo();
			var firstDocument = e.Document.Files?.FirstOrDefault();
			if (firstDocument == null)
			{
				return;
			}

			if (string.Compare(firstDocument.FileTypeId, FiletypeConstants.FileTypeDefinitionId,
					StringComparison.OrdinalIgnoreCase) != 0)
			{
				return;
			}

			var settings = project.GetSettings();

			var documentSettings = new DocumentInfoSettings();
			documentSettings.PopulateFromSettingsBundle(settings, FiletypeConstants.FileTypeDefinitionId);

			if (documentSettings.DocumentInfo.Any())
			{
				var documentInfo =
					documentSettings.DocumentInfo.FirstOrDefault(a =>
						string.Compare(a.Id, firstDocument.Id.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0 &&
						string.Compare(a.Language, firstDocument.Language.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
						string.Compare(a.Name, firstDocument.Name, StringComparison.InvariantCultureIgnoreCase) == 0);

				if (documentInfo != null && documentInfo.IsLoadedOnce)
				{
					return;
				}

				documentSettings.DocumentInfo.Add(new DocumentInfo
				{
					Id = firstDocument.Id.ToString(),
					IsLoadedOnce = true,
					Name = firstDocument.Name,
					Language = firstDocument.Language.CultureInfo.Name
				});

				documentSettings.SaveToSettingsBundle(settings, FiletypeConstants.FileTypeDefinitionId);
				project.UpdateSettings(settings);
				project.Save();
			}

			var languageMappingSettings = new LanguageMappingSettings();
			languageMappingSettings.PopulateFromSettingsBundle(settings, FiletypeConstants.FileTypeDefinitionId);

			var sourceLanguage = languageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, projectInfo.SourceLanguage.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (sourceLanguage != null &&
				sourceLanguage.FilterFillColorChecked && sourceLanguage.FilterScope == Common.Enumerators.FilterScope.Lock.ToString())
			{
				var paragraphUnits = new List<ParagraphUnit>();

				foreach (var segmentPair in e.Document.SegmentPairs)
				{
					var paragraphUnitProperties = segmentPair.GetParagraphUnitProperties();
					var multilingualContextInfo = paragraphUnitProperties.Contexts?.Contexts?.FirstOrDefault(a => a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
					if (multilingualContextInfo != null)
					{
						var lockSegments = Convert.ToBoolean(multilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelFilterLockSegmentsSource) ?? "false");
						if (lockSegments)
						{
							segmentPair.Properties.IsLocked = true;

							var paragraphUnit = paragraphUnits.FirstOrDefault(a =>
								a.ParagraphUnitId == paragraphUnitProperties.ParagraphUnitId.Id);

							if (paragraphUnit != null)
							{
								paragraphUnit.SegmentPairs.Add(segmentPair);
							}
							else
							{
								var newParagraphUnit = new ParagraphUnit
								{
									ParagraphUnitId = paragraphUnitProperties.ParagraphUnitId.Id,
									ParagraphUnitProperties = paragraphUnitProperties,
									SegmentPairs = new List<ISegmentPair> { segmentPair }
								};
								paragraphUnits.Add(newParagraphUnit);
							}
						}
					}
				}

				foreach (var paragraphUnit in paragraphUnits)
				{
					foreach (var segmentPair in paragraphUnit.SegmentPairs)
					{
						e.Document.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);
					}

					e.Document.UpdateParagraphUnitProperties(paragraphUnit.ParagraphUnitProperties);
				}

				_editorController.Save(e.Document);
				project.Save();
			}
		}
	}
}
