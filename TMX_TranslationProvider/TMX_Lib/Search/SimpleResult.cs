using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;

namespace TMX_Lib.Search
{
	internal class SimpleResult
	{
		public Db.TmxTranslationUnit TU;
		public Db.TmxText SourceText, TargetText;

		// if !minvalue -> it's the time when this was translated
		public DateTime TranslateTime
		{
			get
			{
				if (TU == null)
					return DateTime.MinValue;
				return TU.ChangeDate ?? TU.CreationDate ?? DateTime.MinValue;
			}
		}

		public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;
		public TranslationUnitOrigin Origin = TranslationUnitOrigin.Nmt;
		public int Score = 100;

		public bool IsExactMatch => Score >= 100;

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
			if (SourceText == null || TargetText == null)
				throw new TmxException("Invalid simple result, bad source/target text");

			var source = new Segment(sourceLanguage);
			var target = new Segment(targetLanguage);
			source.Add(SourceText.Text);
			target.Add(TargetText.Text);
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
