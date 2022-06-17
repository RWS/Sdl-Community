using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;
using Trados.Transcreate.FileTypeSupport.MSOffice.Readers;
using Trados.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Writers
{
	internal class ContentWriter : AbstractBilingualContentProcessor
	{
		private readonly ImportOptions _importOptions;
		private readonly string _updatedFilePath;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly string _targetLanguage;
		private readonly SegmentBuilder _segmentBuilder;
		private IDocumentProperties _documentProperties;
		private Dictionary<string, UpdatedSegmentContent> _updatedSegments;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;

		public ContentWriter(ImportOptions importOptions, List<AnalysisBand> analysisBands, string updatedFilePath, string targetLanguage,
			SegmentBuilder segmentBuilder)
		{
			_importOptions = importOptions;
			_analysisBands = analysisBands;
			_updatedFilePath = updatedFilePath;
			_targetLanguage = targetLanguage;
			_segmentBuilder = segmentBuilder;

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

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			//base.SetFileProperties(fileInfo);
			//Read the updated DOCX file and collect all the segments
			var reader = new WordReader(_importOptions, SourceLanguage.Name, TargetLanguage.Name);
			_updatedSegments = reader.ReadFile(_updatedFilePath);

			base.SetFileProperties(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
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

				var targetSegment = segmentPair.Target;

				//capture if segment contains tracked changes
				//var hasTrackedChanges = false;

				var segmentIdentifier = string.Empty;
				var updatedSegment = GetUpdatedSegmentContent(paragraphUnit.Properties.ParagraphUnitId.Id,
					segmentPair.Properties.Id.Id, ref segmentIdentifier);

				if (updatedSegment != null)
				{
					var canOverwriteTranslation = _importOptions.OverwriteTranslations || !segmentPair.Target.Any();
					var canOverwriteBackTranslations = _importOptions.OverwriteTranslations
								|| segmentPair.Target.Properties.TranslationOrigin == null
								|| !segmentPair.Target.Properties.TranslationOrigin.MetaDataContainsKey("back-translation")
								|| string.IsNullOrEmpty(segmentPair.Target.Properties.TranslationOrigin.GetMetaData("back-translation"));

					var excludeFilter = false;
					if (_importOptions.ExcludeFilterIds != null)
					{
						excludeFilter = (segmentPair.Properties.IsLocked && _importOptions.ExcludeFilterIds.Exists(a => a == "Locked"))
										|| _importOptions.ExcludeFilterIds.Exists(a => a == status)
										|| _importOptions.ExcludeFilterIds.Exists(a => a == match);
					}

					if (excludeFilter || (!canOverwriteTranslation && !canOverwriteBackTranslations))
					{
						if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
						{
							var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
							var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

							segmentPair.Target.Properties.ConfirmationLevel = statusTranslationNotUpdated;
							segmentPair.Properties.ConfirmationLevel = statusTranslationNotUpdated;

							status = segmentPair.Properties.ConfirmationLevel.ToString();
							match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);
						}

						AddWordExcludedCounts(status, segmentPairInfo, match);

						continue;
					}

					var tokenVisitor = new TokenVisitor();
					tokenVisitor.VisitSegment(targetSegment);
					var originalText = tokenVisitor.PlainText.ToString();

					try
					{
						if (segmentPair.Target.Properties.TranslationOrigin == null)
						{
							targetSegment.Properties.TranslationOrigin = _segmentBuilder.ItemFactory.CreateTranslationOrigin();
						}

						if (canOverwriteTranslation)
						{
							var currentTranslationOrigin = (ITranslationOrigin)targetSegment.Properties.TranslationOrigin.Clone();
							targetSegment.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;

							SetTranslationOrigin(targetSegment);

							if (updatedSegment.TranslationTokens.Count > 0)
							{
								targetSegment = _segmentBuilder.GetUpdatedSegment(targetSegment,
									updatedSegment.TranslationTokens, segmentPair.Source);
							}
						}

						if (canOverwriteBackTranslations && updatedSegment.BackTranslationTokens.Count > 0)
						{
							var backTranslations = JsonConvert.SerializeObject(updatedSegment.BackTranslationTokens);
							segmentPair.Target.Properties.TranslationOrigin.SetMetaData("back-translation", backTranslations);
						}

						if (!canOverwriteTranslation)
						{
							if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
							{
								var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
								var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

								segmentPair.Target.Properties.ConfirmationLevel = statusTranslationNotUpdated;
								segmentPair.Properties.ConfirmationLevel = statusTranslationNotUpdated;

								status = segmentPair.Properties.ConfirmationLevel.ToString();
								match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);
							}

							AddWordExcludedCounts(status, segmentPairInfo, match);
							continue;
						}
					}
					catch (Exception ex)
					{
						throw new Exception("Problem when merging content of segment " + segmentPair.Properties.Id.Id, ex);
					}

					tokenVisitor.Process(targetSegment);
					var updatedText = tokenVisitor.PlainText.ToString();

					if (string.Compare(originalText, updatedText, StringComparison.Ordinal) != 0)
					{
						if (!string.IsNullOrEmpty(_importOptions.StatusTranslationUpdatedId))
						{
							var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationUpdatedId, true,
								out var result);
							var statusTranslationUpdated = success ? result : ConfirmationLevel.Unspecified;

							targetSegment.Properties.ConfirmationLevel = statusTranslationUpdated;
							segmentPair.Properties.ConfirmationLevel = statusTranslationUpdated;

							status = segmentPair.Properties.ConfirmationLevel.ToString();
							match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);
						}

						AddWordProcessedCounts(status, segmentPairInfo, match);
					}
					else
					{
						if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
						{
							var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
							var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

							targetSegment.Properties.ConfirmationLevel = statusTranslationNotUpdated;
						}

						status = targetSegment.Properties.ConfirmationLevel.ToString();
						match = Enumerators.GetTranslationOriginType(targetSegment.Properties.TranslationOrigin, _analysisBands);

						AddWordExcludedCounts(status, segmentPairInfo, match);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(_importOptions.StatusSegmentNotImportedId))
					{
						var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
						var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

						segmentPair.Target.Properties.ConfirmationLevel = statusTranslationNotUpdated;
						segmentPair.Properties.ConfirmationLevel = statusTranslationNotUpdated;

						status = segmentPair.Properties.ConfirmationLevel.ToString();
						match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);
					}

					AddWordNotProcessedCounts(status, segmentPairInfo, match);
				}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private void SetTranslationOrigin(ISegment targetSegment)
		{
			targetSegment.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
			targetSegment.Properties.TranslationOrigin.OriginSystem = _importOptions.OriginSystem;
			targetSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			targetSegment.Properties.TranslationOrigin.IsStructureContextMatch = false;
			targetSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;

			targetSegment.Properties.TranslationOrigin.SetMetaData("last_modified_by", _importOptions.OriginSystem);
			targetSegment.Properties.TranslationOrigin.SetMetaData("modified_on", FormatAsInvariantDateTime(DateTime.UtcNow));
		}

		private string FormatAsInvariantDateTime(DateTime date)
		{
			return date.ToString(DateTimeFormatInfo.InvariantInfo);
		}

		private void AddWordProcessedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Processed, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Processed, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		private void AddWordNotProcessedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.NotProcessed, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.NotProcessed, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		private void AddWordExcludedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		/// <summary>
		/// Need to find out the segment identifier, there is a possibility that the old files 
		/// are processed and the paragraph unit ID is not entered
		/// </summary>
		/// <param name="paragrahpUnitId"></param>
		/// <param name="segmentId"></param>
		/// <param name="segmentIdentifier"></param>
		/// <returns></returns>
		private UpdatedSegmentContent GetUpdatedSegmentContent(string paragrahpUnitId, string segmentId, ref string segmentIdentifier)
		{
			if (_updatedSegments.ContainsKey(paragrahpUnitId + "_" + segmentId))
			{
				segmentIdentifier = paragrahpUnitId + "_" + segmentId;
				return _updatedSegments[segmentIdentifier];
			}
			if (_updatedSegments.ContainsKey("_" + segmentId))
			{
				segmentIdentifier = "_" + segmentId;
				return _updatedSegments[segmentIdentifier];
			}

			return null;
		}

		/// <summary>
		/// Returns the segment confirmation status - based on setting
		/// </summary>
		/// <param name="segmentHasChanges"></param>
		/// <param name="originalStatus"></param>
		/// <returns></returns>
		private ConfirmationLevel UpdateSegmentStatus(bool segmentHasChanges, ConfirmationLevel originalStatus)
		{
			//TODO get status

			//update segments with tracked changes
			//if (segmentHasChanges && _convertSettings.ImportUpdateSegmentMode == GeneratorSettings.UpdateSegmentMode.TrackedOnly &&
			//	_convertSettings.UpdateSegmentStatusTracked)
			//{
			//	return _convertSettings.NewSegmentStatusTrackedChanges;
			//}

			//Update all segments - distinguish between segments with changes and without 
			//if (_convertSettings.ImportUpdateSegmentMode == GeneratorSettings.UpdateSegmentMode.All)
			//{
			//if (_convertSettings.UpdateSegmentStatusTracked && segmentHasChanges)
			//{
			//	return _convertSettings.NewSegmentStatusTrackedChanges;
			//}

			//if (_convertSettings.UpdateSegmentStatusNoTracked && !segmentHasChanges)
			//{
			//	return _convertSettings.NewSegmentStatusAll;
			//}
			//}

			return originalStatus;
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

	}
}
