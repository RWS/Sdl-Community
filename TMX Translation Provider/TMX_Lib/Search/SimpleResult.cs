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
using TMX_Lib.TokenizeUtil;

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

		// what DB is this result coming from? (we can pull results from several databases at the same time)
		public string DatabaseName => Segment.DatabaseName;

		public bool IsExactMatch => Score >= 100;

		public List<PenaltyType> Penalties = new List<PenaltyType>();

		private TokenizeText _tokenizeText = new TokenizeText();
		private ComputeEditDistance _computeEditDistance = new ComputeEditDistance();

		private Segment CreateTokenizedSegment(string text, CultureInfo language) => _tokenizeText.CreateTokenizedSegment(text, language);
		private Segment CreateSimpleSegment(string text, CultureInfo language)
		{
			var segment = new Segment(language);
			segment.Add(text);
			return segment;
		}

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
			// this is useful when I want to update the translation unit
			var uniqueId = (int)Segment.DbTU.TranslationUnitID;
			tu.ResourceId = new PersistentObjectToken(uniqueId, Guid.Empty);
			tu.Origin = Origin;

			var createddAt = CreateTime;
			var modifiedAt = TranslateTime;

			tu.SystemFields.CreationDate = createddAt;
			tu.SystemFields.CreationUser = Segment.DbTU.CreationAuthor;

			if (modifiedAt != DateTime.MinValue)
			{
				var fieldModifiedAt = new SingleStringFieldValue("Modified At", modifiedAt.ToString(CultureInfo.InvariantCulture));
				tu.FieldValues.Add(fieldModifiedAt);
			}

			tu.SystemFields.ChangeDate = modifiedAt;
			tu.SystemFields.ChangeUser = Segment.DbTU.ChangeAuthor;

			var field = new SingleStringFieldValue("Database", DatabaseName);
			tu.FieldValues.Add(field);
		}

		public SearchResult ToSearchResult(string originalText, TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var isTargetConcordance = settings.Mode == SearchMode.TargetConcordanceSearch;
			if (!isTargetConcordance)
				return ToNormalSearchResult(originalText, settings, sourceLanguage, targetLanguage);
			else
				return ToReverseSearchResult(originalText, settings, sourceLanguage, targetLanguage);
		}

		private SearchResult ToNormalSearchResult(string originalText, TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (Segment.DbSourceText == null || Segment.DbTargetText == null)
				throw new TmxException("Invalid simple result, bad source/target text");

			var isConcordance = settings.Mode == SearchMode.ConcordanceSearch || settings.Mode == SearchMode.TargetConcordanceSearch;
			var source = isConcordance ? CreateSimpleSegment(Segment.SourceText, sourceLanguage) :  CreateTokenizedSegment(Segment.SourceText, sourceLanguage);
			var target = CreateSimpleSegment(Segment.TargetText, targetLanguage);
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
			if (!isConcordance)
				sr.ScoringResult.EditDistance = _computeEditDistance.Compute(originalText, Segment.SourceText);

			foreach (var penalty in Penalties)
				AddPenalty(sr, penalty, settings);

			return sr;
		}

		private SearchResult ToReverseSearchResult(string originalText, TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (Segment.DbSourceText == null || Segment.DbTargetText == null)
				throw new TmxException("Invalid simple result, bad source/target text");

			var source = CreateTokenizedSegment(Segment.TargetText, targetLanguage);
			var target = CreateSimpleSegment(Segment.SourceText, sourceLanguage);
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
				ConfirmationLevel = ConfirmationLevel,
			};
			UpdateTU(tu);
			var distance = _computeEditDistance.Compute(originalText, Segment.TargetText);
			var sr = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = Score,
					EditDistance = distance,
				},
				TranslationProposal = tu,
			};

			foreach (var penalty in Penalties)
				AddPenalty(sr, penalty, settings);

			return sr;

		}
	}
}
