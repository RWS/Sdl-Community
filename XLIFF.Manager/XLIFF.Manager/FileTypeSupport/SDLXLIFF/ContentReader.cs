using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using File = Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model.File;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	internal class ContentReader : IBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly bool _copySourceToTarget;
		private readonly bool _ignoreTags;
		private readonly string _inputPath;
		private readonly string _projectId;
		private IBilingualContentHandler _output;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;		

		internal ContentReader(string projectId, string inputPath, bool ignoreTags, SegmentBuilder segmentBuilder, bool copySourceToTarget)
		{
			_projectId = projectId;
			_inputPath = inputPath;
			_ignoreTags = ignoreTags;
			_segmentBuilder = segmentBuilder;
			_copySourceToTarget = copySourceToTarget;

			Xliff = new Xliff();
		}

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

			Xliff.DocInfo.Created = DateTime.Now;
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
				SegmentVisitor.VisitSegment(segmentPair.Source);
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

				if (segmentPair.Target?.Count == 0 && _copySourceToTarget)
				{
					segmentPair.Target.Clear();

					foreach (var sourceElement in segmentPair.Source)
					{
						segmentPair.Target.Add(sourceElement.Clone() as IAbstractMarkupData);
					}
				}

				SegmentVisitor.VisitSegment(segmentPair.Target);
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

				transUnit.SegmentPairs.Add(new SegmentPair(_segmentBuilder)
				{
					Id = segmentPair.Properties.Id.Id,
					Source = new Source { Elements = sourceElements },
					Target = new Target { Elements = targetElements },
					IsLocked = segmentPair.Properties.IsLocked,
					ConfirmationLevel = segmentPair.Properties.ConfirmationLevel,
					TranslationOrigin = segmentPair.Properties.TranslationOrigin
				});
			}

			Xliff.Files[Xliff.Files.Count - 1].Body.TransUnits.Add(transUnit);
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
	}
}
