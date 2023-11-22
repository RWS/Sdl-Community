using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using File = Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model.File;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	internal class ContentReader : IBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ExportOptions _exportOptions;
		private readonly bool _ignoreTags;
		private readonly string _inputPath;
		private readonly string _projectId;
		private readonly List<AnalysisBand> _analysisBands;
		private IBilingualContentHandler _output;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;

		internal ContentReader(string projectId, string inputPath, bool ignoreTags, SegmentBuilder segmentBuilder,
			ExportOptions exportOptions, List<AnalysisBand> analysisBands)
		{
			_projectId = projectId;
			_inputPath = inputPath;
			_ignoreTags = ignoreTags;
			_segmentBuilder = segmentBuilder;

			_exportOptions = exportOptions;
			_analysisBands = analysisBands;

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

		public void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			Xliff.DocInfo.Created = DateTime.UtcNow;
			Xliff.DocInfo.Source = _inputPath;
			Xliff.DocInfo.ProjectId = _projectId;
			Xliff.DocInfo.SourceLanguage = SourceLanguage.Name;
			Xliff.DocInfo.TargetLanguage = TargetLanguage.Name;
		}

		public void Complete()
		{
			// not used for this implementation
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;

			var file = new File
			{
				SourceLanguage = SourceLanguage.Name,
				TargetLanguage = TargetLanguage.Name
			};

			var originalFilePath = fileInfo.FileConversionProperties.OriginalFilePath ??
								   fileInfo.FileConversionProperties.InputFilePath;

			var systemFileInfo = new FileInfo(originalFilePath);
			file.Original = fileInfo.FileConversionProperties.OriginalFilePath;
			//file.DataType = systemFileInfo.Extension;
			file.DataType = fileInfo.FileConversionProperties.FileTypeDefinitionId.Id;

			//var addedExternalDependency = false;
			//foreach (var dependencyFile in fileInfo.FileConversionProperties.DependencyFiles)
			//{
			//	if (!addedExternalDependency &&
			//		dependencyFile.PreferredLinkage != DependencyFileLinkOption.Embed)
			//	{
			//		addedExternalDependency = true;
			//		var externalFile = new ExternalFile
			//		{
			//			Uid = dependencyFile.Id,
			//			Href = dependencyFile.CurrentFilePath ??
			//				   dependencyFile.OriginalFilePath ?? dependencyFile.PathRelativeToConverted
			//		};
			//		file.Header.Skl.ExternalFile = externalFile;
			//	}
			//}

			Xliff.Files.Add(file);

			CreateDummyOutputFiles(fileInfo);
		}

		public void FileComplete()
		{
			// not used for this implementation
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			var transUnit = new TransUnit
			{
				Id = paragraphUnit.Properties.ParagraphUnitId.Id
			};

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);
				var status = segmentPair.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

				if (_exportOptions.ExcludeFilterIds != null)
				{
					if ((segmentPair.Properties.IsLocked && _exportOptions.ExcludeFilterIds.Exists(a => a == "Locked"))
						|| _exportOptions.ExcludeFilterIds.Exists(a => a == status)
						|| _exportOptions.ExcludeFilterIds.Exists(a => a == match))
					{
						AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
						AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);
						continue;
					}
				}

				SegmentVisitor.VisitSegment(segmentPair.Source);
				if (SegmentVisitor.HasRevisions)
				{
					throw new Exception(PluginResources.Message_UnableToProcessFileWithTrackChanges +
										Environment.NewLine +
										PluginResources.Message_AccecptRejectTrackChangesBeforeExporting);
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
					throw new Exception(PluginResources.Message_UnableToProcessFileWithTrackChanges +
										Environment.NewLine +
										PluginResources.Message_AccecptRejectTrackChangesBeforeExporting);
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

				AddWordCounts(status, ConfirmationStatistics.WordCounts.Processed, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Processed, segmentPairInfo);
			}

			if (transUnit.SegmentPairs.Count > 0)
			{
				Xliff.Files[Xliff.Files.Count - 1].Body.TransUnits.Add(transUnit);
			}
		}

		private static void AddWordCounts(string category, ICollection<WordCount> wordCounts, SegmentPairInfo segmentPairInfo)
		{
			var count = wordCounts.FirstOrDefault(a => a.Category == category);
			if (count != null)
			{
				count.Segments++;
				count.Words += segmentPairInfo.SourceWordCounts.Words;
				count.Characters += segmentPairInfo.SourceWordCounts.Characters;
				count.Placeables += segmentPairInfo.SourceWordCounts.Placeables;
				count.Tags += segmentPairInfo.SourceWordCounts.Tags;
			}
			else
			{
				var wordCount = new WordCount
				{
					Category = category,
					Segments = 1,
					Words = segmentPairInfo.SourceWordCounts.Words,
					Characters = segmentPairInfo.SourceWordCounts.Characters,
					Placeables = segmentPairInfo.SourceWordCounts.Placeables,
					Tags = segmentPairInfo.SourceWordCounts.Tags
				};

				wordCounts.Add(wordCount);
			}
		}

		public IBilingualContentHandler Output
		{
			get => _output;
			set => _output = value;
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
