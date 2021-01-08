using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace StudioViews.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		
		public ContentReader()
		{
			ParagraphUnits = new List<IParagraphUnit>();
		}

		public List<IParagraphUnit> ParagraphUnits { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;
			base.SetFileProperties(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			ParagraphUnits.Add(paragraphUnit);
		}
	}
}
