// ---------------------------------
// <copyright file="AbstractTrados2007LanguageDirection.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-08</date>
// ---------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sdl.Community.Trados2007
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemory;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    using Trados.Interop.TMAccess;

    using Action = Sdl.LanguagePlatform.TranslationMemory.Action;
    using SearchResult = Sdl.LanguagePlatform.TranslationMemory.SearchResult;
    
    using Sdl.LanguagePlatform.Lingua.TermRecognition;

    /// <summary>
    /// Abstract base class for file- and server-based Trados 2007 language directions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors",
        Justification = "By original SDL API design.")]
    public abstract class AbstractTrados2007LanguageDirection : ITranslationProviderLanguageDirection
    {
        #region Fields

        protected readonly object locker = new object();
        /// <summary>
        /// Stores Trados 2007 Translation Provider that owns this particular Language Direction.
        /// </summary>
        private readonly AbstractTrados2007TranslationProvider translationProvider;

        /// <summary>
        /// Stores languages direction.
        /// </summary>
        private readonly LanguagePair languageDirection;

        #endregion // Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTrados2007LanguageDirection"/> class.
        /// </summary>
        /// <param name="translationProvider">The Trados 2007 translation provider.</param>
        protected AbstractTrados2007LanguageDirection(AbstractTrados2007TranslationProvider translationProvider)
        {
            if (translationProvider == null)
            {
                throw new ArgumentNullException("translationProvider");
            }

            // Trados 2007 TP supports only one language direction, regardless file- or -server based
            this.translationProvider = translationProvider;
            this.languageDirection = translationProvider.LanguageDirection;
        }

        #region Properties

        /// <summary>
        /// The translation provider to which this language direction belongs.
        /// </summary>
        ITranslationProvider ITranslationProviderLanguageDirection.TranslationProvider
        {
            get
            {
                return this.translationProvider;
            }
        }

        /// <summary>
        /// Gets the source language.
        /// </summary>
        public CultureInfo SourceLanguage
        {
            get
            {
                return this.languageDirection.SourceCulture;
            }
        }

        /// <summary>
        /// Gets the target language.
        /// </summary>
        public CultureInfo TargetLanguage
        {
            get
            {
                return this.languageDirection.TargetCulture;
            }
        }

        /// <summary>
        /// Gets a flag which indicates whether the translation provider supports
        /// searches in the reversed language direction.
        /// </summary>
        public bool CanReverseLanguageDirection
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The translation provider to which this language direction belongs.
        /// </summary>
        protected AbstractTrados2007TranslationProvider TranslationProvider
        {
            get
            {
                return this.translationProvider;
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Adds a translation unit to the database. If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> but should not throw an exception.
        /// </summary>
        /// <param name="translationUnit">The translation unit.</param><param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> which represents the status of the operation (succeeded, ignored, etc).
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            return new ImportResult { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation };
        }

        /// <summary>
        /// Adds an array of translation units to the database. If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> but should not throw an exception.
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param><param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> objects, which mirrors the translation unit array. It has the exact same size and contains the
        ///             status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            return new[] { new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation } };
        }

        /// <summary>
        /// Adds an array of translation units to the database. If hash codes of the previous translations are provided, 
        ///             a found translation will be overwritten. If none is found, or the hash is 0 or the collection is <c>null</c>, 
        ///             the operation behaves identical to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.AddTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],Sdl.LanguagePlatform.TranslationMemory.ImportSettings)"/>.
        /// <para>
        /// If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> but should not throw an exception.
        /// </para>
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param><param name="previousTranslationHashes">If provided, a corresponding array of a the hash code of a previous translation.</param><param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.ImportResult"/> objects, which mirrors the translation unit array. It has the exact same size and contains the
        ///             status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            int count = translationUnits.Length;
            var result = new ImportResult[count];
            var err = new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation };

            for (int i = 0; i < count; i++)
            {
                result[i] = err;
            }

            return result;
        }

        /// <summary>
        /// Adds an array of translation units to the database, but will only add those
        ///             for which the corresponding mask field is <c>true</c>. If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable ImportResult but should not throw an exception.
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param><param name="settings">The settings used for this operation.</param><param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        ///             status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            return new[] { new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation } };
        }

        /// <summary>
        /// Adds an array of translation units to the database, but will only add those
        ///             for which the corresponding mask field is true. If the previous translation hashes are provided,
        ///             existing translations will be updated if the target segment hash changed.
        /// <para>
        /// If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable ImportResult but should not throw an exception.
        /// </para>
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param><param name="previousTranslationHashes">Corresponding hash codes of a previous translation (0 if unknown). The parameter may be null.</param><param name="settings">The settings used for this operation.</param><param name="mask">A boolean array with the same cardinality as the TU array, specifying which TUs to add.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        ///             status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            return new[] { new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation } };
        }

        /// <summary>
        /// Performs a search for an array of segments.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param><param name="segments">The array containing the segments to search for.</param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> objects, which mirrors the segments array. It has the exact same size and contains the
        ///             search results for each segment with the same index within the segments array.
        /// </returns>
        public virtual SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            var searchResultsArray = new SearchResults[segments.Length];

            for (int segmentCount = 0; segmentCount < segments.Length; segmentCount++)
            {
                searchResultsArray[segmentCount] = this.SearchSegment(settings, segments[segmentCount]);
            }

            return searchResultsArray;
        }

        /// <summary>
        /// Performs a search for an array of segments, specifying a mask which specifies which segments should actually be
        ///             searched (only those for which the corresponding mask bit is <c>true</c> are searched). If the mask is <c>null</c>, the method
        ///             behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])"/>. Passing a mask only makes sense in document search contexts (<see cref="P:Sdl.LanguagePlatform.TranslationMemory.SearchSettings.IsDocumentSearch"/>
        ///             set to <c>true</c>).
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param>
        /// <param name="segments">The array containing the segments to search for.</param>
        /// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). 
        /// If <c>null</c>, the method
        /// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])"/>.
        /// </param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> objects, which mirrors the segments array. It has the exact same size and contains the
        ///             search results for each segment with the same index within the segments array.
        /// </returns>
        public virtual SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            var searchResultsArray = new SearchResults[segments.Length];

            for (int segmentCount = 0; segmentCount < segments.Length; segmentCount++)
            {
                if (mask[segmentCount])
                {
                    searchResultsArray[segmentCount] = this.SearchSegment(settings, segments[segmentCount]);
                }
            }

            return searchResultsArray;
        }

        /// <summary>
        /// Performs a translation unit search for an array of translation units.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param><param name="translationUnits">The array containing the translation units to search for.</param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> objects, which mirrors the translation unit array. It has the exact same size and contains
        ///             the search results for each translation unit with the same index within the translation unit array.
        /// </returns>
        public virtual SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            var segments = new Segment[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                segments[unitCount] = translationUnits[unitCount].SourceSegment;
            }

            return this.SearchSegments(settings, segments);
        }

        /// <summary>
        /// Similar to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])"/>, but allows passing a mask which specifies which TUs are actually searched. This is useful
        ///             in document search contexts where some TUs are passed which should be used to establish a (text) context, but which should not be
        ///             processed.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param>
        /// <param name="translationUnits">The array containing the translation units to search for.</param>
        /// <param name="mask">A <c>bool</c> array which specifies which TUs are actually searched (mask[i] = <c>true</c>). 
        /// If <c>null</c>, the method
        /// behaves identically to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])"/>.
        /// </param>
        /// <returns>
        /// An array of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> objects, which mirrors the translation unit array. It has the exact same size and contains
        ///             the search results for each translation unit with the same index within the translation unit array.
        /// </returns>
        public virtual SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            var segments = new Segment[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                segments[unitCount] = translationUnits[unitCount].SourceSegment;
            }

            return this.SearchSegmentsMasked(settings, segments, mask);
        }

        /// <summary>
        /// Updates the properties and fields of an existing translation unit if the source and target segments are unchanged or
        ///             adds a new translation unit otherwise. If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable ImportResult but should not throw an exception.
        /// <para>
        /// The translation unit should be initialized in a previous call to the translation memory, so that the ID property is set to a 
        ///             valid value.
        /// </para>
        /// </summary>
        /// <param name="translationUnit">The translation unit to be updated.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            return new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation };
        }

        /// <summary>
        /// Updates the properties and fields of an array of existing translation units if the source and target segments are unchanged or
        ///             adds new translation units otherwise. If the provider doesn't support adding/updating, the 
        ///             implementation should return a reasonable ImportResult but should not throw an exception.
        /// <para>
        /// The translation units should be initialized in previous calls to the translation memory, so that their ID properties
        ///             are set to valid values.
        /// </para>
        /// </summary>
        /// <param name="translationUnits">The translation unit array to be updated.</param>
        /// <returns>
        /// An array of results which mirrors the translation unit array. It has the exact same size and contains the 
        ///             results for each translation unit with the same index within the translation unit array.
        /// </returns>
        [Obsolete(@"Trados 2007 Translation Provider does not support adding\editing.")]
        public virtual ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            return new[] { new ImportResult() { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation } };
        }

        #endregion // Methods (Obsolete)

        //#region Methods abstract

        ///// <summary>
        ///// Performs a segment search.
        ///// </summary>
        ///// <param name="settings">The settings that define the search parameters.</param>
        ///// <param name="segment">The segment to search for.</param>
        ///// <returns>
        ///// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        ///// </returns>
        //public abstract SearchResults SearchSegment(SearchSettings settings, Segment segment);

        ///// <summary>
        ///// Performs a text search.
        ///// </summary>
        ///// <param name="settings">The settings that define the search parameters.</param>
        ///// <param name="segment">The text to search for.</param>
        ///// <returns>
        ///// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        ///// </returns>
        //public abstract SearchResults SearchText(SearchSettings settings, string segment);

        ///// <summary>
        ///// Performs a translation unit search.
        ///// </summary>
        ///// <param name="settings">The settings that define the search parameters.</param>
        ///// <param name="translationUnit">The translation unit to search for.</param>
        ///// <returns>
        ///// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        ///// </returns>
        //public abstract SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit);

        //#endregion // Methods abstract

        #region Virtual methods

        #region Methods

        /// <summary>
        /// Performs a segment search.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param>
        /// <param name="segment">The segment to search for.</param>
        /// <returns>
        /// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        /// </returns>
        public virtual SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            lock (this.locker)
            {
                string segmentText = SegmentParser.ExtractSegmentText(segment);
                
                Trados.Interop.TMAccess.SearchResult tradosSegments = GetMatchSegments(settings, segmentText);

                if (tradosSegments == null)
                    return null;

                var searchResults = new SearchResults
                {
                    SourceSegment = segment.Duplicate(),
                    SourceHash = segment.GetHashCode(),
                };

                int id = 0;

                foreach (TmMatch match in tradosSegments.TmMatches)
                {
                    // form ScoringResult via applying penalties
                    var scoringResult = new ScoringResult
                    {
                        AppliedPenalties = new List<AppliedPenalty>(),
                        BaseScore = match.ScoreValues.GetMatchPercent(),
                        IsStructureContextMatch = false,
                    };
                    this.ApplyPenalties(match, scoringResult, settings);

                    // convert trados 2007 format segments into Studio 2011 ones
                    string translationSource = match.GetSource(settings.IsConcordanceSearch);
                    string translationTarget = match.GetTarget(settings.IsConcordanceSearch);

                    var sourceSegment = SegmentParser.CreateSegment(
                        scoringResult, settings, translationSource, this.SourceLanguage);
                    var targetSegment = SegmentParser.CreateSegment(
                        scoringResult, settings, translationTarget, this.TargetLanguage);

                    // tokenize segments
                    SegmentParser.TokenizeSegment(ref sourceSegment);
                    SegmentParser.TokenizeSegment(ref targetSegment);

                    // create Translation Unit
                    var unit = new TranslationUnit(sourceSegment, targetSegment)
                    {
                        Origin = TranslationUnitOrigin.TM,
                        Format = TranslationUnitFormat.TradosTranslatorsWorkbench,
                        SystemFields = new SystemFields(),
                        ResourceId = new PersistentObjectToken(id++, Guid.NewGuid()),
                    };

                    // set custom attributes
                    this.SetMetaData(match, unit, settings);

                    // form SearchResult
                    var searchResult = new SearchResult(unit)
                    {
                        ScoringResult = scoringResult,
                        TranslationProposal = unit.Duplicate()
                    };

                    scoringResult.EditDistance = ComputeEditDistance(segment, searchResult, settings);

                    // set "yellow" fileds for concordance
                    if (settings.Mode == SearchMode.ConcordanceSearch || settings.Mode == SearchMode.TargetConcordanceSearch)
                    {
                        this.AnnotateConcordanceHit(segment, searchResult, settings.Mode);
                    }

                    // finally...
                    searchResults.Add(searchResult);
                }


                searchResults.CheckForMultipleTranslations(settings);
                searchResults.RemoveDuplicates();

                //if "Search fo fuzzy matches even if exact match found" is OFF 
                //and exact matches are present then also remove fuzzy matches
                //According to LanguagePlatform description for enum SearchMode, exact match should return only exact matches,
                //normal search should return exact matches OR fuzzy matches if no exact matches found
                if (settings.Mode == SearchMode.ExactSearch ||
                    (settings.Mode == SearchMode.NormalSearch && searchResults.Results.Any(i => i.ScoringResult.IsExactMatch)))
                {
                    searchResults.Results.RemoveAll(r => !r.ScoringResult.IsExactMatch);
                }

                //Determine placebles in the searched segment
                if (settings.Mode != SearchMode.TargetConcordanceSearch && !settings.IsConcordanceSearch)
                    searchResults.DocumentPlaceables = PlaceableComputer.ComputePlaceables(searchResults.SourceSegment, null);

                return searchResults;
            }
        }

        private Trados.Interop.TMAccess.SearchResult GetMatchSegments(SearchSettings settings, string segmentText)
        {
            switch (settings.Mode)
            {
                case SearchMode.TargetConcordanceSearch:
                case SearchMode.ConcordanceSearch:
                    {
                        return this.TranslationProvider.TradosProvider.ConcordanceSearch(
                            segmentText, settings.MinScore, settings.MaxResults);
                    }

                case SearchMode.ExactSearch:
                case SearchMode.FullSearch:
                case SearchMode.NormalSearch:
                    {
                        return this.TranslationProvider.TradosProvider.SearchTxtOnly(
                            segmentText, settings.MinScore, settings.MaxResults);
                    }
            }
            return null;
        }

        private void SetMetaData(TmMatch match, TranslationUnit unit, SearchSettings settings)
        {
            // Only concordance search method returns segments with attributes.
            // That's why concordance search for already found segment is used here.
            TmMatch matchToCopyFrom = settings.Mode == SearchMode.ConcordanceSearch ?
                                match :
                                RetrieveSimilarMatchWithConcordanceSearch(match, settings);

            foreach (TuAttributeField attr in matchToCopyFrom.TranslationUnitFields.Attributes)
            {
                if (attr.Values.Count == 0)
                    continue;

                FieldValue field; 
                
                if (attr.Values.Count == 1)
                {
                    field =  new SingleStringFieldValue(attr.Name, attr.Values[1]);
                }
                else
                {
                    field = new MultipleStringFieldValue(attr.Name, TuAttributeValuesToList(attr.Values));
                }

                unit.FieldValues.Add(field);
            }

            SetSystemFieldsValues(matchToCopyFrom, unit);
        }

        private TmMatch RetrieveSimilarMatchWithConcordanceSearch(TmMatch match, SearchSettings settings)
        {
            var segmentText = match.GetSource(settings.IsConcordanceSearch);

            // concordance search don't return any results with too little value of MaxResults
            // That's why it isn't set up to use it's default value 20.
            Trados.Interop.TMAccess.SearchResult tradosSegments = this.TranslationProvider.TradosProvider.ConcordanceSearch(
                            segmentText, settings.MinScore);

            return tradosSegments.TmMatches.Count > 0 ? tradosSegments.TmMatches[1] : match;
        }

        private ICollection<string> TuAttributeValuesToList(TuAttributeValues tuAttributeValues)
        {
            List<string> list = new List<string>();

            foreach (string el in tuAttributeValues)
            {
                list.Add(el);
            }

            return list;
        }

        private Sdl.LanguagePlatform.Core.EditDistance.EditDistance ComputeEditDistance(Segment sourceSegment, SearchResult searchResult, SearchSettings settings)
        {
            Sdl.LanguagePlatform.Lingua.SegmentEditDistanceComputer edc
                = new Sdl.LanguagePlatform.Lingua.SegmentEditDistanceComputer();

            Sdl.LanguagePlatform.Lingua.TagAssociations alignedTags = null;

            Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions = (settings.AutoLocalizationSettings == null)
                    ? Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone
                    : settings.AutoLocalizationSettings.DisableAutoSubstitution;

            return edc.ComputeEditDistance(sourceSegment.Tokens,
                                searchResult.MemoryTranslationUnit.SourceSegment.Tokens,
                                settings.Mode == SearchMode.DuplicateSearch, disabledAutoSubstitutions, out alignedTags);
        }
 
        /// <summary>
        /// Performs a text search.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param>
        /// <param name="segment">The text to search for.</param>
        /// <returns>
        /// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        /// </returns>
        public virtual SearchResults SearchText(SearchSettings settings, string segment)
        {
            Segment newSegment = settings.Mode == SearchMode.TargetConcordanceSearch
                ? new Segment(this.TargetLanguage)
                : new Segment(this.SourceLanguage);

            newSegment.Add(segment);

            return this.SearchSegment(settings, newSegment);
        }

        /// <summary>
        /// Performs a translation unit search.
        /// </summary>
        /// <param name="settings">The settings that define the search parameters.</param>
        /// <param name="translationUnit">The translation unit to search for.</param>
        /// <returns>
        /// A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.SearchResults"/> object containing the results or an empty object if no results were found.
        /// </returns>
        public virtual SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            return this.SearchSegment(settings, translationUnit.SourceSegment);
        }

        #endregion // Methods

        #endregion //End Virtual methods

        #region Methods protected

        /// <summary>
        /// Applies the penalties from the match. 
        /// </summary>
        /// <param name="translationMemoryMatch">The translation memory match.</param>
        /// <param name="scoringResult">The scoring result.</param>
        /// <param name="settings">The settings.</param>
        protected void ApplyPenalties(ITmMatch translationMemoryMatch, ScoringResult scoringResult, SearchSettings settings)
        {
            scoringResult.CheckForZeroAndApply(new Penalty(PenaltyType.FilterPenalty, 
                translationMemoryMatch.ScoreValues.TextFieldMismatchPenalty + translationMemoryMatch.ScoreValues.AttributeMismatchPenalty));

            // this can be applied twice, see Penalty.CanApplyMultipleTimes method
            scoringResult.CheckForZeroAndApply(new Penalty(PenaltyType.TagMismatch, translationMemoryMatch.ScoreValues.FormattingMismatchPenalty));
            scoringResult.CheckForZeroAndApply(new Penalty(PenaltyType.TagMismatch, translationMemoryMatch.ScoreValues.PlaceableMismatchPenalty));
            
            scoringResult.CheckForZeroAndApply(new Penalty(PenaltyType.MultipleTranslations, translationMemoryMatch.ScoreValues.MultipleTranslationsPenalty));
        }

        /// <summary>
        /// Sets the field value in the translation unit.
        /// </summary>
        /// <param name="translationMemoryMatch">The translation memory match.</param>
        /// <param name="translationUnit">The translation unit to applied the value to.</param>
        protected void SetSystemFieldsValues(ITmMatch translationMemoryMatch, TranslationUnit translationUnit)
        {
            translationUnit.SystemFields.ChangeDate = translationMemoryMatch.TranslationUnitFields.ChangeDate;
            translationUnit.SystemFields.ChangeUser = translationMemoryMatch.TranslationUnitFields.ChangeUser;
            translationUnit.SystemFields.CreationDate = translationMemoryMatch.TranslationUnitFields.CreationDate;
            translationUnit.SystemFields.CreationUser = translationMemoryMatch.TranslationUnitFields.CreationUser;
            translationUnit.SystemFields.UseCount = translationMemoryMatch.TranslationUnitFields.UsageCounter;
            translationUnit.SystemFields.UseDate = translationMemoryMatch.TranslationUnitFields.UsedDate;
            //// translationUnit.SystemFields.UseUser = translationMemoryMatch.TranslationUnitFields 
            //// Note: last used user is not stored
        }

        /// <summary>
        /// Annotates concordance search results so that the matching range is highlighted in Studio
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="result">The result.</param>
        /// <param name="searchMode">The search mode.</param>
        protected void AnnotateConcordanceHit(Segment segment, SearchResult result, SearchMode searchMode)
        {
            try
            {
                // we do not support target concordance
                Segment resultSegment = result.MemoryTranslationUnit.SourceSegment;

                if (resultSegment != null && resultSegment.Tokens != null)
                {
                    TermFinderResult finderResult = TermFinder.FindTerms(segment, resultSegment, true);

                    if (finderResult != null
                        && finderResult.MatchingRanges != null
                        && finderResult.MatchingRanges.Count > 0)
                    {
                        result.ScoringResult.MatchingConcordanceRanges = finderResult.MatchingRanges;
                    }
                }
            }
            catch
            {
                // catch all and ignore - don't want the term finder to break anything
            }
        }

        #endregion // Methods private

    }

    static class ScoringResultExtensions
    {
        public static void CheckForZeroAndApply(this ScoringResult scoringResult, Penalty penalty)
        {
            if (penalty.Malus != 0)
            {
                scoringResult.ApplyPenalty(penalty);
            }
        }
    }

    static class TmTradosExtenstions
    {
        public static string GetTarget(this TmMatch tm, bool isConcordance)
        {
            return !isConcordance
                       ? tm.GetTarget()
                       : tm.TtxTranslationUnit.Children[0].Children[1].Children[0].Children[0].Children[1].Text;

        }

        public static string GetSource(this TmMatch tm, bool isConcordance)
        {
             return !isConcordance
                       ? tm.GetSource()
                       : tm.TtxTranslationUnit.Children[0].Children[1].Children[0].Children[0].Children[0].Text;
        }
    }
}