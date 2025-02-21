using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Model;
using File = Trados.Transcreate.FileTypeSupport.XLIFF.Model.File;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
{
	internal class XliffContentReader : AbstractBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ExportOptions _exportOptions;
		private readonly bool _ignoreTags;
		private readonly string _inputPath;
		private readonly string _projectId;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly string _documentId;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;
		private int _contextIndex;

		internal XliffContentReader(string projectId, string documentId, string inputPath, string targetLanguage, bool ignoreTags, SegmentBuilder segmentBuilder,
			ExportOptions exportOptions, List<AnalysisBand> analysisBands)
		{
			_projectId = projectId;
			_documentId = documentId;
			_inputPath = inputPath;
			_ignoreTags = ignoreTags;
			_segmentBuilder = segmentBuilder;
			_contextIndex = 0;

			_exportOptions = exportOptions;
			_analysisBands = analysisBands;


			TargetLanguage = new CultureInfo(targetLanguage);

			Xliff = new Xliff();
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; }

		public Xliff Xliff { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		private SegmentVisitor SegmentVisitor => _segmentVisitor ?? (_segmentVisitor = new SegmentVisitor(_ignoreTags));

		internal List<string> DummyOutputFiles = new List<string>();

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? TargetLanguage;

			Xliff.DocInfo.Created = DateTime.UtcNow;
			Xliff.DocInfo.Source = _inputPath;
			Xliff.DocInfo.ProjectId = _projectId;
			Xliff.DocInfo.DocumentId = _documentId;
			Xliff.DocInfo.SourceLanguage = SourceLanguage.Name;
			Xliff.DocInfo.TargetLanguage = TargetLanguage.Name;

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;

			var file = new File
			{
				SourceLanguage = SourceLanguage.Name,
				TargetLanguage = TargetLanguage.Name
			};

			file.Original = fileInfo.FileConversionProperties.OriginalFilePath;
			file.DataType = fileInfo.FileConversionProperties.FileTypeDefinitionId.Id;

			Xliff.Files.Add(file);

			CreateDummyOutputFiles(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			var transUnit = new TransUnit
			{
				Id = paragraphUnit.Properties.ParagraphUnitId.Id
			};

			var file = GetFileContainer(paragraphUnit);

			UpdateContexts(file, paragraphUnit, transUnit);

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

				if (_exportOptions.ExcludeFilterIds != null)
				{
					if ((segmentPair.Properties.IsLocked && _exportOptions.ExcludeFilterIds.Exists(a => a == "Locked"))
						|| _exportOptions.ExcludeFilterIds.Exists(a => a == status)
						|| _exportOptions.ExcludeFilterIds.Exists(a => a == match))
					{
						AddWordExcludedCounts(status, segmentPairInfo, match);
						continue;
					}
				}

				SegmentVisitor.VisitSegment(segmentPair.Source);
				if (SegmentVisitor.HasRevisions)
				{
					//throw new Exception(PluginResources.Message_UnableToProcessFileWithTrackChanges +
										//Environment.NewLine +
										//PluginResources.Message_AccecptRejectTrackChangesBeforeExporting);
				}

				var sourceElements = SegmentVisitor.Elements;
				if (SegmentVisitor.Comments.Count > 0)
				{
					foreach (var comment in SegmentVisitor.Comments)
					{
						if (!Xliff.DocInfo.Comments.ContainsKey(comment.Key))
						{
							Xliff.DocInfo.Comments.Add(comment.Key, comment.Value);
						}
					}
				}
				UpdateTagIds();

				if (segmentPair.Target?.Count == 0 && _exportOptions.CopySourceToTarget)
				{
					segmentPair.Target.Clear();

					foreach (var sourceElement in segmentPair.Source)
					{
						segmentPair.Target.Add(sourceElement.Clone() as IAbstractMarkupData);
					}
				}

				SegmentVisitor.VisitSegment(segmentPair.Target);
				if (SegmentVisitor.HasRevisions)
				{
					//throw new Exception(PluginResources.Message_UnableToProcessFileWithTrackChanges +
										//Environment.NewLine +
										//PluginResources.Message_AccecptRejectTrackChangesBeforeExporting);
				}
				var targetElements = SegmentVisitor.Elements;
				if (SegmentVisitor.Comments.Count > 0)
				{
					foreach (var comment in SegmentVisitor.Comments)
					{
						if (!Xliff.DocInfo.Comments.ContainsKey(comment.Key))
						{
							Xliff.DocInfo.Comments.Add(comment.Key, comment.Value);
						}
					}
				}
				UpdateTagIds();

				var newSegmentPair = new SegmentPair(_segmentBuilder)
				{
					Id = segmentPair.Properties.Id.Id,
					Source = new Source { Elements = sourceElements },
					Target = new Target { Elements = targetElements },
					IsLocked = segmentPair.Properties.IsLocked,
					ConfirmationLevel = segmentPair.Properties.ConfirmationLevel,
					TranslationOrigin = segmentPair.Properties.TranslationOrigin
				};
				transUnit.SegmentPairs.Add(newSegmentPair);

				AddWordProcessedCounts(status, segmentPairInfo, match);
			}

			if (transUnit.SegmentPairs.Count > 0)
			{
				file.Body.TransUnits.Add(transUnit);
			}
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

		public override void FileComplete()
		{
			//One file complete, but we may have more
			base.FileComplete();
		}

		/// <summary>
		/// Using Complete instead of FileComplete for merged files.
		/// </summary>
		public override void Complete()
		{
			base.Complete();
		}

		private void UpdateContexts(File file, IParagraphUnit paragraphUnit, TransUnit transUnit)
		{
			if (paragraphUnit.Properties.Contexts?.Contexts != null)
			{
				foreach (var context in paragraphUnit.Properties.Contexts.Contexts)
				{
					var existingContext = file.Header.Contexts.FirstOrDefault(a =>
						string.Compare(a.ContextType, context.ContextType, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.DisplayCode, context.DisplayCode, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.DisplayName, context.DisplayName, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.Description, context.Description, StringComparison.CurrentCultureIgnoreCase) == 0);

					if (existingContext != null)
					{
						transUnit.Contexts.Add(existingContext);
					}
					else
					{
						_contextIndex++;

						var newContext = new Context
						{
							Id = _contextIndex.ToString(),
							ContextType = context.ContextType,
							Description = context.Description,
							DisplayCode = context.DisplayCode,
							DisplayName = context.DisplayName
						};

						if (context.MetaData != null)
						{
							foreach (var metaData in context.MetaData)
							{
								newContext.MetaData.Add(metaData.Key, metaData.Value);
							}
						}

						transUnit.Contexts.Add(newContext);
						file.Header.Contexts.Add(newContext);
					}
				}
			}
		}

		private File GetFileContainer(IParagraphUnit paragraphUnit)
		{
			var contextType = GetContextType(paragraphUnit);
			var file = Xliff.Files[Xliff.Files.Count - 1];
			if (contextType != null)
			{
				if (contextType != "Recommended")
				{
					var alternativeFile = Xliff.Files.FirstOrDefault(a => a.Original == contextType);
					if (alternativeFile != null)
					{
						file = alternativeFile;
					}
					else
					{
						file = new File
						{
							Original = contextType,
							SourceLanguage = SourceLanguage.Name,
							TargetLanguage = TargetLanguage.Name,
							DataType = "xml"
						};
						Xliff.Files.Add(file);
					}
				}
				else
				{
					file.Original = "Recommended";
				}
			}

			return file;
		}

		private string GetContextType(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.Properties.Contexts?.Contexts != null)
			{
				var contextType = "Recommended";
				foreach (var context in paragraphUnit.Properties.Contexts.Contexts)
				{
					if (context.ContextType.StartsWith("Alternative "))
					{
						return context.ContextType;
					}
				}

				return contextType;
			}

			return null;
		}

		private void UpdateTagIds()
		{
			foreach (var id in SegmentVisitor.TagPairIds)
			{
				if (!Xliff.TagPairIds.Contains(id))
				{
					Xliff.TagPairIds.Add(id);
				}
			}
			foreach (var id in SegmentVisitor.PlaceholderIds)
			{
				if (!Xliff.PlaceholderIds.Contains(id))
				{
					Xliff.PlaceholderIds.Add(id);
				}
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

		private void CreateDummyOutputFiles(IFileProperties fileInfo)
		{
			foreach (var fileProperties in fileInfo.FileConversionProperties.DependencyFiles)
			{
				var iDependencyFileProperties = fileProperties;
				if (iDependencyFileProperties.PreferredLinkage == DependencyFileLinkOption.Embed || iDependencyFileProperties.FileExists)
				{
					continue;
				}

				try
				{
					fileProperties.PreferredLinkage = DependencyFileLinkOption.ReferenceRelative;
					if (!System.IO.File.Exists(iDependencyFileProperties.CurrentFilePath) &&
						iDependencyFileProperties.CurrentFilePath != null)
					{
						var dummyOutputFullPath = Path.Combine(Path.GetDirectoryName(_inputPath),
							Path.GetFileName(iDependencyFileProperties.CurrentFilePath));

						DummyOutputFiles.Add(dummyOutputFullPath);
						using (var sw = new StreamWriter(dummyOutputFullPath))
						{
							sw.WriteLine(string.Empty);
							sw.Flush();
							sw.Close();
						}
					}
				}
				catch
				{
					// catch all; ignore
				}
			}
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
					throw new Exception(string.Format("Unable to parse the file; {0} langauge cannot be null!", SourceLanguage == null ? "Source" : "Target"));
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
