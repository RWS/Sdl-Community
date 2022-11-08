using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace TMX_TranslationProvider.Search.Result
{
	internal class SimpleResult
	{
		public List<TextPart> Source = new List<TextPart>();
		public List<TextPart> Target = new List<TextPart>();

		public string SourceText() => string.Join("", Source.Where(part => part.Text != "").Select(part => part.Text));
		public string TargetText() => string.Join("", Target.Where(part => part.Text != "").Select(part => part.Text));

		public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;
		public TranslationUnitOrigin Origin = TranslationUnitOrigin.TM;
		public int Score = 100;

		// FIXME tokenize it!
		public SearchResult ToSearchResult(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var source = new Segment(sourceLanguage);
			var target = new Segment(targetLanguage);
			source.Add(SourceText());
			target.Add(TargetText());
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel,
			};
			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			tu.Origin = Origin;

			var sr = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = Score,
				},
				TranslationProposal = tu,
			};

			return sr;
		}
	}
}
