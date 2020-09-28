using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.Toolkit.LanguagePlatform;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Readers
{
	internal class ContentReader : AbstractBilingualContentProcessor
	{
		private readonly TokenVisitor _tokenVisitor;
		private readonly GeneratorSettings _generatorSettings;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly string _originalFilePath;
		private readonly List<string> _contextTypes;
		private IDocumentProperties _documentProperties;
		private WordWriter _wordWriter;
		private bool _exportingSameFile;
		private string _previousTextFunction;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;

		public ContentReader(TokenVisitor tokenVisitor, GeneratorSettings generatorSettings, List<AnalysisBand> analysisBands, string filePath)
		{
			_tokenVisitor = tokenVisitor;
			_generatorSettings = generatorSettings;
			_analysisBands = analysisBands;
			_originalFilePath = filePath;
			_contextTypes = new List<string>();

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
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
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

			//get output file name
			var info = new FileInfo(_originalFilePath);
			var outputFile = info.DirectoryName + Path.DirectorySeparatorChar +
			                 info.Name.Substring(0, info.Name.IndexOf(info.Extension, StringComparison.Ordinal));

			_wordWriter = new WordWriter(
				fileInfo.FileConversionProperties.SourceLanguage.IsoAbbreviation,
				fileInfo.FileConversionProperties.TargetLanguage.IsoAbbreviation);

			_wordWriter.Initialize(fileInfo.FileConversionProperties.FileId.Id, outputFile + ".Export.docx", _generatorSettings);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var relevance = paragraphUnit.Properties.Contexts.Contexts.FirstOrDefault(
				a => a.ContextType == "Recommended" || a.ContextType.StartsWith("Alternative"));

			var textFunction = paragraphUnit.Properties.Contexts.Contexts.FirstOrDefault(
				a => string.Compare(a.DisplayName, "Text Function", StringComparison.CurrentCultureIgnoreCase) == 0);

			if (relevance == null)
			{
				return;
			}

			if (_contextTypes.Count == 0)
			{
				_contextTypes.Add(relevance.ContextType);
				_wordWriter.AddNewTable(relevance.ContextType);
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				if (SkipSegment(segmentPair))
				{
					continue;
				}

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

				AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);

				_tokenVisitor.Process(segmentPair.Source);
				var sourceText = _tokenVisitor.PlainText.ToString();
				var sourceTokens = CloneList(_tokenVisitor.Tokens);

				_tokenVisitor.Process(segmentPair.Target);
				var targetText = _tokenVisitor.PlainText.ToString();
				var targetTokens = CloneList(_tokenVisitor.Tokens);
				var comments = _tokenVisitor.Comments;

				var textFunction1 = textFunction?.Description;
				if (string.IsNullOrEmpty(textFunction1))
				{
					textFunction1 = _previousTextFunction;
				}
				else
				{
					_previousTextFunction = textFunction1;
				}

				//TODO: recover the back-translation tokens
				_wordWriter.WriteEntry(
					segmentPair.Properties.Id.Id,
					paragraphUnit.Properties.ParagraphUnitId.Id,
					relevance.ContextType,
					textFunction1,
					sourceTokens,
					targetTokens,
					segmentPair.Properties,
					sourceTokens);
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
			var origin = segmentPair.Properties.TranslationOrigin;
			var confLevel = segmentPair.Properties.ConfirmationLevel;

			if (_generatorSettings.ExcludeExportType == GeneratorSettings.ExclusionType.Status)
			{
				if (_generatorSettings.ExcludedStatuses.Contains(confLevel))
				{
					return true;
				}
			}
			else
			{
				if (origin == null)
				{
					return _generatorSettings.DontExportNoMatch;
				}

				if (origin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget &&
				    _generatorSettings.DontExportContext)
				{
					return true;
				}

				if (origin.MatchPercent == 100 && _generatorSettings.DontExportExact)
				{
					return true;
				}

				if ((origin.MatchPercent > 0 && origin.MatchPercent < 100) && _generatorSettings.DontExportFuzzy)
				{
					return true;
				}

				if (origin.MatchPercent == 0 && _generatorSettings.DontExportNoMatch)
				{
					return true;
				}
			}
			return false;
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
				var pathInfo = new Toolkit.LanguagePlatform.Models.PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Toolkit.LanguagePlatform.Models.Settings(SourceLanguage, TargetLanguage), pathInfo);

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
 