using System;
using System.Globalization;
using System.Text;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace TMX_Lib.Utils
{
	// for testing/debugging
	internal class DummyTranslate
	{
		private static string DummyTranslateText(string text)
		{
			var dummy = new StringBuilder();
			foreach (var ch in text)
				dummy.Append($"{ch}-");
			return dummy.ToString();
		}
		public static SearchResults SearchSegment(Segment segment, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var result = new SearchResults();
			var source = new Segment(sourceLanguage);
			var target = new Segment(targetLanguage);
			source.Add(segment.ToPlain());
			target.Add(DummyTranslateText(segment.ToPlain()));
			result.SourceSegment = source;
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel.Draft,
			};
			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			tu.Origin = TranslationUnitOrigin.Nmt;

			var sr = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = 70,
				},
				TranslationProposal = tu,
			};
			result.Add(sr);
			return result;
		}
	}
}
