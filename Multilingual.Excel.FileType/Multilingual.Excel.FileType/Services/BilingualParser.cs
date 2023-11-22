﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Multilingual.Excel.FileType.Common;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.EmbeddedContent;
using Multilingual.Excel.FileType.Extensions;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.Settings.Entities;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Services.Entities;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class BilingualParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware, IPublishSubContent
	{
		private IFileProperties _fileProperties;

		private LanguageMapping _sourceLanguageMapping;

		private readonly SegmentBuilder _segmentBuilder;

		private readonly EntityContext _entityContext;

		private readonly EntityService _entityService;

		private readonly ExcelReader _excelReader;

		private UniqueEntitySettings _entitySettings;

		public Language DefaultSourceLanguage { get; set; }


		public BilingualParser(SegmentBuilder segmentBuilder, EntityContext entityContext, EntityService entityService, ExcelReader excelReader)
		{
			_segmentBuilder = segmentBuilder;
			_entityContext = entityContext;
			_entityService = entityService;
			_excelReader = excelReader;
		}

		private void LoadEntityMappings()
		{
			_entityContext.EnableEntityMappingsSetting = _entitySettings.ConvertEntities;
			_entityContext.ConvertNumericEntitiesToPlaceholder = _entitySettings.ConvertNumericEntityToPlaceholder;
			_entityContext.SkipConversionInsideLockedContent = _entitySettings.SkipConversionInsideLockedContent;

			foreach (var entitySet in _entitySettings.EntitySets.Values)
			{
				foreach (var entityMapping in entitySet.Mappings)
				{
					_entityContext.AddSettingsEntity(entityMapping);
				}
			}
		}

		public event EventHandler<ProgressEventArgs> Progress;

		public IDocumentProperties DocumentProperties { get; set; }

		public IBilingualContentHandler Output { get; set; }

		public LanguageMappingSettings LanguageMappingSettings { get; private set; }

		public EmbeddedContentSettings EmbeddedContentSettings { get; private set; }

		public PlaceholderPatternSettings PlaceholderPatternSettings { get; private set; }

		public CommentMappingSettings CommentMappingSettings { get; private set; }

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			LanguageMappingSettings = new LanguageMappingSettings();
			LanguageMappingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			EmbeddedContentSettings = new EmbeddedContentSettings();
			EmbeddedContentSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			PlaceholderPatternSettings = new PlaceholderPatternSettings();
			PlaceholderPatternSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			CommentMappingSettings = new CommentMappingSettings();
			CommentMappingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			_entitySettings = new UniqueEntitySettings();
			_entitySettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			LoadEntityMappings();
		}

		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;

			_fileProperties.FileConversionProperties.SourceLanguage = _fileProperties.FileConversionProperties.SourceLanguage.CultureInfo != null
				? _fileProperties.FileConversionProperties.SourceLanguage
				: DefaultSourceLanguage;

			if (_fileProperties.FileConversionProperties?.SourceLanguage?.CultureInfo == null)
			{
				throw new Exception("Source language cannot be null!");
			}

			_sourceLanguageMapping = LanguageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, _fileProperties.FileConversionProperties.SourceLanguage.CultureInfo.Name,
					StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public void StartOfInput()
		{
			OnProgress(0);

			Output.Initialize(DocumentProperties);
			Output.SetFileProperties(_fileProperties);
		}

		public bool ParseNext()
		{
			var filePath = _fileProperties.FileConversionProperties.InputFilePath;

			var excelOptions = new ExcelOptions
			{
				ReadAllWorkSheets = LanguageMappingSettings.LanguageMappingReadAllWorkSheets,
				FirstRowIndex = LanguageMappingSettings.LanguageMappingFirstRowIndex,
				FirstRowIndexIsHeading = LanguageMappingSettings.LanguageMappingFirstRowIsHeading
			};

			var excelColumns = GetExcelColumns();
			var excelSheets = _excelReader.GetExcelSheets(filePath, excelOptions, excelColumns);

			var totalRows = excelSheets.Sum(a => a.Rows.Count);
			var index = 0;

			foreach (var excelSheet in excelSheets.OrderBy(a => a.Index))
			{
				var structureInfoAddedToFirstParagraph = false;
				foreach (var excelRow in excelSheet.Rows)
				{
					// Create Paragraph from excelRow
					var content = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.ContentColumn);

					// Apply filter on fill color
					var lockSegments = false;
					if (_sourceLanguageMapping.FilterFillColorChecked)
					{
						var filterFillColors = FilterFillColors(_sourceLanguageMapping.FilterFillColor);
						var excelCellFillColor = NormalizeHexCode(content?.Background);
						var action = (Common.Enumerators.FilterScope)Enum.Parse(typeof(Common.Enumerators.FilterScope), _sourceLanguageMapping.FilterScope, true);

						var containsColor = ContainsColor(filterFillColors, excelCellFillColor);
						if (containsColor)
						{
							switch (action)
							{
								case Enumerators.FilterScope.Ignore:
									Console.WriteLine(@"Ignored: Color {0} Sheet {1} Row {2}, Column {3} Content {4}",
										excelCellFillColor,
										excelSheet.Name,
										excelRow.Index,
										content?.Column.Name,
										content?.Value);
									continue;
								case Enumerators.FilterScope.Lock:
									lockSegments = true;
									break;
							}
						}
						else if (action == Enumerators.FilterScope.Import)
						{
							Console.WriteLine(@"Not Imported: Color {0} Sheet {1} Row {2}, Column {3} Content {4}",
								excelCellFillColor,
								excelSheet.Name,
								excelRow.Index,
								content?.Column.Name,
								content?.Value);

							continue;
						}
					}

					//if (!string.IsNullOrEmpty(content?.Value))
					//{
					var contentWithOutCdata = content.Clone() as ExcelCell;

					var IsCDATA = ContentIsCDATA(content);
					if (IsCDATA)
					{
						contentWithOutCdata.Value = content.Value.Substring(9);
						contentWithOutCdata.Value = contentWithOutCdata.Value.Substring(0, contentWithOutCdata.Value.Length - 3);
					}

					if (EmbeddedContentSettings.EmbeddedContentProcess &&
						//ContainsTags(content) &&
						(EmbeddedContentSettings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.All ||
						 (IsCDATA && EmbeddedContentSettings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.CDATA)))
					{
						var paragraphUnit = AddStructureParagraph(excelSheet, excelRow, IsCDATA, lockSegments);
						if (!structureInfoAddedToFirstParagraph)
						{
							paragraphUnit.Properties.Contexts.StructureInfo = GetStructureInfo(excelSheet);
							structureInfoAddedToFirstParagraph = true;
						}

						Output.ProcessParagraphUnit(paragraphUnit);

						PublishEmbeddedSubContent(EmbeddedContentSettings.EmbeddedContentProcessorId,
							(IsCDATA ? contentWithOutCdata.Value : content.Value));
					}
					else
					{
						var paragraphUnit = CreateParagraphUnit(excelSheet, excelRow, lockSegments);
						if (!structureInfoAddedToFirstParagraph)
						{
							paragraphUnit.Properties.Contexts.StructureInfo = GetStructureInfo(excelSheet);
							structureInfoAddedToFirstParagraph = true;
						}

						Output.ProcessParagraphUnit(paragraphUnit);
					}
					//}

					index++;
					OnProgress(Convert.ToByte(Math.Round(100m * ((index + 1m) / totalRows), 0)));
				}
			}

			return false;
		}

		private List<string> FilterFillColors(string fillColors)
		{
			if (fillColors == null)
			{
				return null;
			}

			var filterFillColors = new List<string>();
			foreach (var fillColor in fillColors.Split(';'))
			{
				var hexCode = NormalizeHexCode(fillColor);
				if (!string.IsNullOrEmpty(hexCode))
				{
					filterFillColors.Add(hexCode);
				}
			}

			return filterFillColors;
		}

		private static bool ContainsColor(IReadOnlyCollection<string> filterFillColors, string excelCellFillColor)
		{
			if (filterFillColors == null || excelCellFillColor == null)
			{
				return false;
			}

			return filterFillColors.Any(
				fillColor => string.Compare(fillColor, excelCellFillColor, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		private static string NormalizeHexCode(string fillColor)
		{
			var hexCode = fillColor?.Replace("#", string.Empty);
			hexCode = hexCode?.Replace(";", string.Empty);
			hexCode = hexCode?.Trim();

			if (hexCode == null || hexCode.Length < 6)
			{
				return null;
			}

			// compare only the rgb hex code
			return hexCode.Substring(hexCode.Length - 6);
		}

		private IStructureInfo GetStructureInfo(ExcelSheet excelSheet)
		{
			var paragraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(StandardContextTypes.WorkSheetName);
			paragraphContextInfo.DisplayName = excelSheet.Name;
			var structureInfo = ItemFactory.PropertiesFactory.CreateStructureInfo(paragraphContextInfo, true, null);
			return structureInfo;
		}

		private bool ContentIsCDATA(ExcelCell content)
		{
			if (content.Value == null)
			{
				return false;
			}

			var IsCDATA = content.Value.StartsWith("<![CDATA[", StringComparison.InvariantCultureIgnoreCase)
						  && content.Value.EndsWith("]]>", StringComparison.InvariantCultureIgnoreCase);
			return IsCDATA;
		}

		public void EndOfInput()
		{
			Output.FileComplete();
			Output.Complete();

			OnProgress(100);
		}

		private List<ExcelColumn> GetExcelColumns()
		{
			var excelColumns = new List<ExcelColumn>();
			if (!string.IsNullOrEmpty(_sourceLanguageMapping.ContentColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.ContentColumn
				});
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.ContextColumn))
			{
				foreach (var columnName in GetColumnNames(_sourceLanguageMapping.ContextColumn))
				{
					excelColumns.Add(new ExcelColumn
					{
						Name = columnName
					});
				}
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.CommentColumn))
			{
				foreach (var columnName in GetColumnNames(_sourceLanguageMapping.CommentColumn))
				{
					excelColumns.Add(new ExcelColumn
					{
						Name = columnName
					});
				}
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.CharacterLimitationColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.CharacterLimitationColumn
				});
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.PixelLimitationColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.PixelLimitationColumn
				});
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.PixelFontFamilyColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.PixelFontFamilyColumn
				});
			}

			if (!string.IsNullOrEmpty(_sourceLanguageMapping.PixelFontSizeColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.PixelFontSizeColumn
				});
			}

			return excelColumns;
		}

		private List<string> GetColumnNames(string columnName)
		{
			var columns = new List<string>();
			if (columnName == null)
			{
				return columns;
			}

			columns.AddRange(columnName.Split(';').Where(column => !string.IsNullOrEmpty(column)));

			return columns;
		}

		private IParagraphUnit CreateParagraphUnit(ExcelSheet excelSheet, ExcelRow excelRow, bool lockSegments)
		{
			var content = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.ContentColumn);
			//var lockType = (lockSegments ? LockTypeFlags.Manual : LockTypeFlags.Unlocked);
			var structureParagraphUnit = GetStructureParagraphUnit(excelSheet, excelRow, LockTypeFlags.Unlocked, false, lockSegments);
			var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
			segmentPairProperties.IsLocked = lockSegments;
			if (_sourceLanguageMapping == null)
			{
				return structureParagraphUnit;
			}

			var segment = GetSegment(content, segmentPairProperties);
			segment.Properties.IsLocked = lockSegments;

			IAbstractMarkupDataContainer currentContainer = structureParagraphUnit.Source;
			IAbstractMarkupDataContainer previousContainer = null;
			foreach (var abstractMarkupData in segment)
			{
				if (abstractMarkupData is IAbstractMarkupDataContainer container)
				{
					currentContainer = container;
				}

				if (previousContainer != null)
				{
					if (previousContainer == abstractMarkupData.Parent)
					{
						previousContainer = currentContainer;
						continue;
					}
				}

				if (!currentContainer.Contains(abstractMarkupData))
				{
					structureParagraphUnit.Source.Add(abstractMarkupData.Clone() as IAbstractMarkupData);
				}

				previousContainer = currentContainer;
			}

			AddContextToParagraph(excelRow, structureParagraphUnit);
			AddCommentsToParagraph(excelRow, structureParagraphUnit);

			return structureParagraphUnit;
		}

		private ISegment GetSegment(ExcelCell content, ISegmentPairProperties segmentPairProperties)
		{
			var xmlNode = GetXmlNodeFromContent(content);
			var segment = xmlNode == null
				? _segmentBuilder.CreateSegment(content?.Value, segmentPairProperties)
				: _segmentBuilder.CreateSegment(xmlNode, segmentPairProperties, SegmentationHint.MayExclude);

			return segment;
		}

		private XmlNode GetXmlNodeFromContent(ExcelCell content)
		{
			if (string.IsNullOrEmpty(content?.Value))
			{
				return null;
			}

			var cellContent = _entityService.MarkupKnownEntities(content.Value);
			cellContent = _entityService.EscapeXmlCharacters(cellContent);

			try
			{
				var xmlNode = GetXmlDocumentRootNode(cellContent);
				if (xmlNode != null)
				{
					return xmlNode;
				}

				// attempt to markup html self closing tags (e.g. br) + reprocess
				cellContent = _entityService.MarkupSelfClosingHtmlTags(cellContent);

				return GetXmlDocumentRootNode(cellContent);
			}
			catch (Exception ex)
			{
				// catch all; ignore
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}

			return null;
		}

		private static XmlNode GetXmlDocumentRootNode(string cellContent)
		{
			try
			{
				var xmlContent = new XmlDocument();
				xmlContent.LoadXml("<ExcelRoot>" + cellContent + "</ExcelRoot>");

				return xmlContent.FirstChild;
			}
			catch (Exception ex)
			{
				// catch all; ignore
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}

			return null;
		}

		private IParagraphUnit AddStructureParagraph(ExcelSheet excelSheet, ExcelRow excelRow, bool isCDATA, bool lockSegments)
		{
			var structureParagraphUnit = GetStructureParagraphUnit(excelSheet, excelRow, LockTypeFlags.Structure, isCDATA, lockSegments);

			AddContextToParagraph(excelRow, structureParagraphUnit);
			AddCommentsToParagraph(excelRow, structureParagraphUnit);

			return structureParagraphUnit;
		}

		private IParagraphUnit GetStructureParagraphUnit(ExcelSheet excelSheet, ExcelRow excelRow, LockTypeFlags lockType, bool IsCDATA, bool lockSegments)
		{
			var structureParagraphUnit = ItemFactory.CreateParagraphUnit(lockType);
			structureParagraphUnit.Properties.Comments = PropertiesFactory.CreateCommentProperties();

			var contextProperties = ItemFactory.PropertiesFactory.CreateContextProperties();

			var paragraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
			contextProperties.Contexts.Add(paragraphContextInfo);

			var multilingualParagraphContextInfo = _segmentBuilder.CreateMultilingualParagraphContextInfo();
			contextProperties.Contexts.Add(multilingualParagraphContextInfo);

			structureParagraphUnit.Properties.Contexts = contextProperties;

			var content = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.ContentColumn);
			var charLimit = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.CharacterLimitationColumn);
			var pixelLimit = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.PixelLimitationColumn);
			var pixelFontName = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.PixelFontFamilyColumn);
			var pixelFontSize = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.PixelFontSizeColumn);

			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelSheetIndex, excelSheet.Index.ToString());
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelSheetName, excelSheet.Name);
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelRowIndex, excelRow.Index.ToString());
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationSource, charLimit?.Value ?? "0");
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationSource, pixelLimit?.Value ?? "0");
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameSource, pixelFontName?.Value ?? string.Empty);
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeSource, pixelFontSize?.Value ?? "0");
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.IsCDATA, IsCDATA.ToString());
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelFilterBackgroundColorSource, content?.Background ?? "0");
			multilingualParagraphContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelFilterLockSegmentsSource, lockSegments.ToString());

			return structureParagraphUnit;
		}

		private void AddContextToParagraph(ExcelRow excelRow, IParagraphUnit structureParagraphUnit)
		{
			var contextCells = GetColumnCells(excelRow, _sourceLanguageMapping.ContextColumn);
			if (contextCells.Count > 0)
			{
				foreach (var cell in contextCells)
				{
					if (!string.IsNullOrEmpty(cell?.Value?.Trim()))
					{
						var displayName = cell.Column?.Text?.Trim();
						if (string.IsNullOrEmpty(displayName))
						{
							displayName = FiletypeConstants.ExcelContextInformationDisplayName;
						}
						var propertyParagraphContextInfo = _segmentBuilder.CreateExcelContextParagraphContextInfo(displayName, cell.Value);
						structureParagraphUnit.Properties.Contexts.Contexts.Add(propertyParagraphContextInfo);
					}
				}
			}
		}

		private void AddCommentsToParagraph(ExcelRow excelRow, IParagraphUnit structureParagraphUnit)
		{
			if (!structureParagraphUnit.Source.Any() && structureParagraphUnit.Properties.LockType != LockTypeFlags.Structure)
			{
				return;
			}

			var commentCells = GetColumnCells(excelRow, _sourceLanguageMapping.CommentColumn);
			if (commentCells.Count > 0)
			{
				foreach (var cell in commentCells)
				{
					if (string.IsNullOrEmpty(cell?.Value?.Trim()))
					{
						continue;
					}

					if (structureParagraphUnit.Properties.LockType == LockTypeFlags.Structure)
					{
						// These comments will be added to the target paragraph from the NormalizeEmbeddedContentContextProcessor
						var newComment = _segmentBuilder.CreateComment(cell.Value, WindowsUserId,
							Severity.Low, DateTime.Now, "1.0");
						structureParagraphUnit.Properties.Comments.Add(newComment);
					}
					else
					{
						var newComment = _segmentBuilder.CreateCommentContainer(cell.Value, WindowsUserId,
							Severity.Low, DateTime.Now, "1.0") as IAbstractMarkupDataContainer;

						structureParagraphUnit.Source.MoveAllItemsTo(newComment);
						structureParagraphUnit.Source.Add(newComment as IAbstractMarkupData);
					}
				}
			}
		}

		private List<ExcelCell> GetColumnCells(ExcelRow excelRow, string column)
		{
			var contextCells = new List<ExcelCell>();
			foreach (var columnName in GetColumnNames(column))
			{
				var excelCell = excelRow.Cells.FirstOrDefault(a => a.Column.Name == columnName);
				if (excelCell != null)
				{
					contextCells.Add(excelCell);
				}
			}

			return contextCells;
		}

		public static string WindowsUserId => (Environment.UserDomainName + "\\" + Environment.UserName).Trim();

		//private bool ContainsTags(ExcelCell content)
		//{
		//	var xmlNode = GetXmlNodeFromContent(content);
		//	if (xmlNode == null)
		//	{
		//		return !string.IsNullOrEmpty(content.Value);
		//	}

		//	return xmlNode.ChildNodes.Cast<XmlNode>().Any(
		//		childNode => childNode.NodeType == XmlNodeType.Element
		//					 || childNode.NodeType == XmlNodeType.EndElement
		//					 || childNode.NodeType == XmlNodeType.CDATA);
		//}

		//private bool ContainsTags(XmlNode xmlNode)
		//{
		//	if (xmlNode == null)
		//	{
		//		return false;
		//	}

		//	return xmlNode.ChildNodes.Cast<XmlNode>().Any(
		//		childNode => childNode.NodeType == XmlNodeType.Element
		//					 || childNode.NodeType == XmlNodeType.EndElement
		//					 || childNode.NodeType == XmlNodeType.CDATA);
		//}

		public void Dispose()
		{
		}

		protected virtual void OnProgress(byte percent)
		{
			Progress?.Invoke(this, new ProgressEventArgs(percent));
		}

		private void PublishEmbeddedSubContent(string embeddedProcessorId, string text)
		{
			using (var memoryStream = new MemoryStream())
			{
				var content = new StringBuilder();
				var fragments = _entityService.ConvertKnownEntitiesInMarkupData(text, EntityRule.Parser);

				foreach (var fragment in fragments)
				{
					if (fragment is IText textFragment)
					{
						if (textFragment.Properties?.Text != null)
						{
							textFragment.Properties.Text = ProtextSoftReturns(textFragment.Properties.Text);
							content.Append(textFragment.Properties.Text);
						}
					}
					else
					{
						content.Append(fragment);
					}
				}

				memoryStream.WriteString(content.ToString());

				PublishSubContent(embeddedProcessorId, memoryStream);
			}
		}

		private static string ProtextSoftReturns(string text)
		{
			text = text?.Replace("\n", EntityConstants.SoftReturnEntityRefEscape);
			return text;
		}

		private void PublishSubContent(string embeddedProcessorId, Stream subContentStream)
		{
			if (string.IsNullOrEmpty(embeddedProcessorId))
			{
				return;
			}

			var subContentEventArgs = new ProcessSubContentEventArgs(embeddedProcessorId, subContentStream);

			PublishContent(subContentEventArgs);
		}

		public event EventHandler<ProcessSubContentEventArgs> ProcessSubContent;

		public void PublishContent(ProcessSubContentEventArgs eventArgs)
		{
			ProcessSubContent?.Invoke(this, eventArgs);
		}
	}
}
