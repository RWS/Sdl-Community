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
		// if !minvalue -> it's the time when this was translated
		public DateTime TranslateTime = DateTime.MinValue;

		public List<TextPart> Source = new List<TextPart>();
		public List<TextPart> Target = new List<TextPart>();

		public string SourceText() => string.Join("", Source.Where(part => part.Text != "").Select(part => part.Text));
		public string TargetText() => string.Join("", Target.Where(part => part.Text != "").Select(part => part.Text));

		public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;
		public TranslationUnitOrigin Origin = TranslationUnitOrigin.TM;
		public int Score = 100;

		public List<PenaltyType> Penalties = new List<PenaltyType>();

		// more about Penalties: TranslationMemorySettings.cs:498
		private void AddPenalty(SearchResult result, PenaltyType type, SearchSettings settings)
		{
			var penalty = settings.FindPenalty(type);
			if (penalty != null)
				result.ScoringResult.ApplyPenalty(penalty);
		}

		// FIXME tokenize it!
		public SearchResult ToSearchResult(SearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
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

			foreach (var penalty in Penalties)
				AddPenalty(sr, penalty, settings);

			return sr;
		}
	}
}
