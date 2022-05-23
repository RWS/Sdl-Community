using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;
using Trados.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Trados.Transcreate.FileTypeSupport.MSOffice.Writers;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Readers
{
	internal class ContentReader : AbstractBilingualContentProcessor
	{
		private readonly string _projectId;
		private readonly TokenVisitor _tokenVisitor;
		private readonly ExportOptions _exportOptions;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly string _originalFilePath;
		private readonly List<string> _contextTypes;
		private readonly string _outputFilePath;
		private readonly string _targetLanguage;
		private IDocumentProperties _documentProperties;
		private WordWriter _wordWriter;
		private bool _exportingSameFile;
		private string _previousTextFunctionDescription;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;


		public ContentReader(string projectId, TokenVisitor tokenVisitor, ExportOptions exportOptions,
			List<AnalysisBand> analysisBands, string filePath, string outputFilePath, string targetLanguage)
		{
			_projectId = projectId;
			_tokenVisitor = tokenVisitor;
			_exportOptions = exportOptions;
			_analysisBands = analysisBands;
			_originalFilePath = filePath;
			_contextTypes = new List<string>();
			_outputFilePath = outputFilePath;
			_targetLanguage = targetLanguage;

			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? new CultureInfo(_targetLanguage);

			//base.Initialize(documentInfo);
		}

		/// <summary>
		/// Start of new file
		/// </summary>
		/// <param name="fileInfo"></param>
		public override void SetFileProperties(IFileProperties fileInfo)
		{
			//handle merged files
			if (_exportingSameFile)
			{
				return;
			}

			_wordWriter = new WordWriter(SourceLanguage.Name, TargetLanguage.Name);

			_wordWriter.Initialize(_projectId, fileInfo.FileConversionProperties.FileId.Id,
				_originalFilePath, _outputFilePath, _exportOptions);

			//base.SetFileProperties(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var priority = paragraphUnit.Properties.Contexts.Contexts.FirstOrDefault(
				a => a.ContextType == "Recommended" || a.ContextType.StartsWith("Alternative"));

			var textFunction = paragraphUnit.Properties.Contexts.Contexts.FirstOrDefault(
				a => string.Compare(a.DisplayName, "Text Function", StringComparison.CurrentCultureIgnoreCase) == 0);

			if (priority == null)
			{
				return;
			}

			if (_contextTypes.Count == 0)
			{
				_contextTypes.Add(priority.ContextType);
				_wordWriter.AddNewTable(priority.ContextType);
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				SegmentPairInfo segmentPairInfo = null;
				try
				{
					segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);
				}
				catch
				{
					// catch all; ignore
				}

				var status = segmentPair.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

				if (SkipSegment(segmentPair))
				{
					AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
					AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);
					AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
					AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);

					continue;
				}

				AddWordCounts(status, ConfirmationStatistics.WordCounts.Processed, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Processed, segmentPairInfo);
				AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);

				_tokenVisitor.Process(segmentPair.Source);
				var sourceText = _tokenVisitor.PlainText.ToString();
				var sourceTokens = CloneList(_tokenVisitor.Tokens);

				_tokenVisitor.Process(segmentPair.Target);
				var targetText = _tokenVisitor.PlainText.ToString();
				var targetTokens = CloneList(_tokenVisitor.Tokens);
				var comments = _tokenVisitor.Comments;

				List<Token> backTranslationTokens = null;
				if (segmentPair.Target.Properties.TranslationOrigin != null &&
					segmentPair.Target.Properties.TranslationOrigin.MetaDataContainsKey("back-translation"))
				{
					//TODO check if we need to create TranslationOrigin??

					var backTranslation =
						segmentPair.Target.Properties.TranslationOrigin.GetMetaData("back-translation");

					backTranslationTokens = JsonConvert.DeserializeObject<List<Token>>(backTranslation);
				}

				var textFunctionDescription = textFunction?.Description;
				if (string.IsNullOrEmpty(textFunctionDescription))
				{
					textFunctionDescription = _previousTextFunctionDescription;
				}
				else
				{
					_previousTextFunctionDescription = textFunctionDescription;
				}

				_wordWriter.WriteEntry(
					segmentPair.Properties.Id.Id,
					paragraphUnit.Properties.ParagraphUnitId.Id,
					priority.ContextType,
					textFunctionDescription,
					sourceTokens,
					targetTokens,
					segmentPair.Properties,
					backTranslationTokens);
			}
		}

		private static void AddWordCounts(string category, ICollection<WordCount> wordCounts, SegmentPairInfo segmentPairInfo)
		{
			var count = wordCounts.FirstOrDefault(a => a.Category == category);
			if (count != null)
			{
				count.Segments++;
				count.Words += segmentPairInfo?.SourceWordCounts?.Words ?? 0;
				count.Characters += segmentPairInfo?.SourceWordCounts?.Characters ?? 0;
				count.Placeables += segmentPairInfo?.SourceWordCounts?.Placeables ?? 0;
				count.Tags += segmentPairInfo?.SourceWordCounts?.Tags ?? 0;
			}
			else
			{
				var wordCount = new WordCount
				{
					Category = category,
					Segments = 1,
					Words = segmentPairInfo?.SourceWordCounts?.Words ?? 0,
					Characters = segmentPairInfo?.SourceWordCounts?.Characters ?? 0,
					Placeables = segmentPairInfo?.SourceWordCounts?.Placeables ?? 0,
					Tags = segmentPairInfo?.SourceWordCounts?.Tags ?? 0
				};

				wordCounts.Add(wordCount);
			}
		}


		private bool SkipSegment(ISegmentPair segmentPair)
		{
			if (_exportOptions.ExcludeFilterIds == null)
			{
				return false;
			}


			var status = segmentPair.Properties.ConfirmationLevel.ToString();
			var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

			return segmentPair.Properties.IsLocked && _exportOptions.ExcludeFilterIds.Exists(a => a == "Locked")
				   || _exportOptions.ExcludeFilterIds.Exists(a => a == status)
				   || _exportOptions.ExcludeFilterIds.Exists(a => a == match);
		}

		private List<Token> CloneList(IEnumerable<Token> list)
		{
			var result = new List<Token>();

			foreach (var item in list)
			{
				result.Add(item);
			}

			return result;
		}


		public override void FileComplete()
		{
			//One file complete, but we may have more
			_exportingSameFile = true;
			base.FileComplete();
		}

		/// <summary>
		/// Using Complete instead of FileComplete for merged files.
		/// </summary>
		public override void Complete()
		{
			_exportingSameFile = false;
			_wordWriter.Complete();
		}

		private SegmentPairProcessor SegmentPairProcessor
		{
			get
			{
				if (_segmentPairProcessor != null)
				{
					return _segmentPairProcessor;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(string.Format("Unable to parse the file; {0} language cannot be null!", SourceLanguage == null ? "Source" : "Target"));
				}


				var productName = GetProductName();
				var pathInfo = new Trados.Community.Toolkit.LanguagePlatform.Models.PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Trados.Community.Toolkit.LanguagePlatform.Models.Settings(SourceLanguage, TargetLanguage), pathInfo);

				return _segmentPairProcessor;
			}
		}

		private string GetProductName()
		{
			if (!string.IsNullOrEmpty(_productName))
			{
				return _productName;
			}

			var studioVersionService = new StudioVersionService();
			var studioVersion = studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				_productName = studioVersion.StudioDocumentsFolderName;
			}

			return _productName;
		}
	}
}
