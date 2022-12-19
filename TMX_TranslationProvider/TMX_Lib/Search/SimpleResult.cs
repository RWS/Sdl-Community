using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public readonly TmxSegment Segment;

		// if !minvalue -> it's the time when this was translated
		public DateTime TranslateTime
		{
			get
			{
				if (Segment.DbTU == null)
					return DateTime.MinValue;
				return Segment.DbTU.ChangeDate ?? Segment.DbTU.CreationDate ?? DateTime.MinValue;
			}
		}
		public DateTime CreateTime
		{
			get
			{
				if (Segment.DbTU == null)
					return DateTime.MinValue;
				return Segment.DbTU.CreationDate ?? DateTime.MinValue;
			}
		}

		public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;
		public TranslationUnitOrigin Origin = TranslationUnitOrigin.TM;
		public int Score = 100;

		public bool IsExactMatch => Score >= 100;

		public List<PenaltyType> Penalties = new List<PenaltyType>();

		public SimpleResult(TmxSegment segment)
		{
			Debug.Assert(segment != null);
			Segment = segment;
		}

		// more about Penalties: TranslationMemorySettings.cs:498
		private void AddPenalty(SearchResult result, PenaltyType type, TmxSearchSettings settings)
		{
			var penalty = settings.FindPenalty(type);
			if (penalty != null)
				result.ScoringResult.ApplyPenalty(penalty);
		}

		private void UpdateTU(TranslationUnit tu)
		{
			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
			tu.Origin = Origin;

			var createddAt = CreateTime;
			var modifiedAt = TranslateTime;

			tu.SystemFields.CreationDate = createddAt;
			tu.SystemFields.CreationUser = Segment.DbTU.CreationAuthor;

			if (modifiedAt != DateTime.MinValue)
			{
				var fieldValue = new MultiplePicklistFieldValue("modifiedAt");
				fieldValue.Add(modifiedAt.ToString(CultureInfo.InvariantCulture));
				tu.FieldValues.Add(fieldValue);
			}

			tu.SystemFields.ChangeDate = modifiedAt;
			tu.SystemFields.ChangeUser = Segment.DbTU.ChangeAuthor;
		}

		// FIXME tokenize it!
		public SearchResult ToSearchResult(TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var isTargetConcordance = settings.Mode == SearchMode.TargetConcordanceSearch;
			if (!isTargetConcordance)
				return ToNormalSearchResult(settings, sourceLanguage, targetLanguage);
			else
				return ToReverseSearchResult(settings, sourceLanguage, targetLanguage);
		}

		private SearchResult ToNormalSearchResult(TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (Segment.DbSourceText == null || Segment.DbTargetText == null)
				throw new TmxException("Invalid simple result, bad source/target text");

			var source = new Segment(sourceLanguage);
			var target = new Segment(targetLanguage);
			source.Add(Segment.SourceText);
			target.Add(Segment.TargetText);
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel,
			};
			UpdateTU(tu);
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

		private SearchResult ToReverseSearchResult(TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (Segment.DbSourceText == null || Segment.DbTargetText == null)
				throw new TmxException("Invalid simple result, bad source/target text");

			var source = new Segment(targetLanguage);
			var target = new Segment(sourceLanguage);
			source.Add(Segment.TargetText);
			target.Add(Segment.SourceText);
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel,
			};
			UpdateTU(tu);
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
