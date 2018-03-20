using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.Amgen.Core.Tokenization;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Amgen.Core.SDLXLIFF
{
	public class ParagraphProcessor : IBilingualContentProcessor
	{
		private Tokenizer _tokenizer { get; set; }

		public Tokenizer Tokenizer
		{
			get
			{
				if (_tokenizer != null)
				{
					return _tokenizer;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(string.Format("Unable to parse the file; {0} langauge cannot be null!", SourceLanguage == null ? "Source" : "Target"));
				}

				_tokenizer = new Tokenizer(SourceLanguage, TargetLanguage);
				_tokenizer.CreateTranslationMemory();

				return _tokenizer;
			}
		}

		internal List<SegmentInfo> Segments { get; set; }

		internal CultureInfo SourceLanguage { get; private set; }

		internal CultureInfo TargetLanguage { get; private set; }

		public IBilingualContentHandler Output
		{
			get;
			set;
		}

		public void Complete()
		{
			//not needed for this implementation
		}

		public void FileComplete()
		{
			// Not required for this implementation.
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			SourceLanguage = documentInfo.SourceLanguage?.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo;
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				Tokenizer.SearchTranslationMemory(segmentPair);

				Segments.Add(new SegmentInfo
				{
					ParagraphId = paragraphUnit.Properties.ParagraphUnitId.Id,
					SegmentId = segmentPair.Properties.Id.Id,
					SegmentPair = segmentPair,
					SourceSegment = _tokenizer.SourceSegment,
					TargetSegment = _tokenizer.TargetSegment
				});
			}
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			// Not required for this implementation.
		}
	}
}