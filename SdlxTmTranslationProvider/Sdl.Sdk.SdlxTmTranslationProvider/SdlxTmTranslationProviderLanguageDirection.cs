//-----------------------------------------------------------------------
// <copyright file="SdlxTmTranslationProviderLanguageDirection.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.Core.EditDistance;
    using Sdl.LanguagePlatform.Core.Tokenization;
    using Sdl.LanguagePlatform.TranslationMemory;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using Sdl.Sdk.SdlxTmTranslationProvider.Lingua;
    using Sdl.Sdk.SdlxTmTranslationProvider.SdlxTm;

    /// <summary>
    /// A language direction for an SDLX translation memory
    /// </summary>
    public class SdlxTmTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        /// <summary>
        /// The SDLX language code for the source language
        /// </summary>
        private string sourceLanguage;

        /// <summary>
        /// The SDLX language code for the target language
        /// </summary>
        private string targetLanguage;

        /// <summary>
        /// The tm controller reference number for the TM
        /// </summary>
        private int translationMemoryReference;

        /// <summary>
        /// The parent (owner) object of this language direction
        /// </summary>
        private SdlxTmTranslationProviderPlugin sdlxTmTranslationProvider;

        /// <summary>
        /// Fuzzy match threshold
        /// </summary>
        private int fuzzyThreshold = 75;

        /// <summary>
        /// The maximum number of matches to be returned
        /// </summary>
        private int maximumMatches = 10;

        /// <summary>
        /// Determines whether to use Trados style scoring
        /// </summary>
        private bool useTradosScoring = false;

        /// <summary>
        /// The context mismatch penalty
        /// </summary>
        private int contextPenalty = 0;

        /// <summary>
        /// Determines whether we will use AutoMatch when doing TM lookups
        /// </summary>
        private bool useAutoMatch = false;

        /// <summary>
        /// A map of tags to text (and vice-versa).
        /// </summary>
        private List<KeyValuePair<string, SegmentElement>> tagMap = new List<KeyValuePair<string, SegmentElement>>();

        /// <summary>
        /// A stack of tags to ensure that the results are well-formed.
        /// </summary>
        private Stack<string> tagStack = new Stack<string>();

        /// <summary>
        /// A list of segment ranges to highlight the concordance matches
        /// </summary>
        private List<SegmentRange> concordanceRanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="SdlxTmTranslationProviderLanguageDirection"/> class.
        /// </summary>
        /// <param name="sdlxTmTranslationProvider">The SDLX translation memory translation provider.</param>
        /// <param name="translationMemoryReference">The tm reference.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        public SdlxTmTranslationProviderLanguageDirection(SdlxTmTranslationProviderPlugin sdlxTmTranslationProvider, int translationMemoryReference, string sourceLanguage, string targetLanguage)
        {
            this.sdlxTmTranslationProvider = sdlxTmTranslationProvider;
            this.translationMemoryReference = translationMemoryReference;
            this.sourceLanguage = sourceLanguage;
            this.targetLanguage = targetLanguage;
        }

        #region ITranslationProviderLanguageDirection Members

        /// <summary>
        /// Gets a value indicating whether this instance can reverse language direction.
        /// </summary>
        /// <value><c>true</c> if this instance can reverse language direction; otherwise, <c>false</c>.</value>
        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the source language.
        /// </summary>
        /// <value>The source language.</value>
        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return SdlxTmLanguageMap.GetCultureInfoFromIsoCode(this.sourceLanguage); }
        }

        /// <summary>
        /// Gets the target language.
        /// </summary>
        /// <value>The target language.</value>
        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return SdlxTmLanguageMap.GetCultureInfoFromIsoCode(this.targetLanguage); }
        }

        /// <summary>
        /// Gets the translation provider.
        /// </summary>
        /// <value>The translation provider.</value>
        public ITranslationProvider TranslationProvider
        {
            get { return this.sdlxTmTranslationProvider; }
        }

        /// <summary>
        /// Adds an array of translation units to the database. If hash codes of the previous translations are provided,
        /// a found translation will be overwritten. If none is found, or the hash is 0 or the collection is null,
        /// the operation behaves identical to AddTranslationUnits().
        /// <para>
        /// If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// </para>
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param>
        /// <param name="previousTranslationHashes">If provided, a corresponding array of a the hash code of a previous translation.</param>
        /// <param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        /// status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            return this.UpdateTranslationUnits(translationUnits);
        }

        /// <summary>
        /// Adds an array of translation units to the database, but will only add those
        /// for which the corresponding mask field is true. If the previous translation hashes are provided,
        /// existing translations will be updated if the target segment hash changed.
        /// <para>
        /// If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// </para>
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param>
        /// <param name="previousTranslationHashes">Corresponding hash codes of a previous translation (0 if unknown). The parameter may be null.</param>
        /// <param name="settings">The settings used for this operation.</param>
        /// <param name="mask">A bool array with the same cardinality as the TU array, specifying which TUs to add.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        /// status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            List<TranslationUnit> updateUnits = new List<TranslationUnit>();
            for (int i = 0; i < translationUnits.Length; i++)
            {
                if (mask[i])
                {
                    updateUnits.Add(translationUnits[i]);
                }
            }

            return this.UpdateTranslationUnits(updateUnits.ToArray());
        }

        /// <summary>
        /// Adds a translation unit to the database. If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// </summary>
        /// <param name="translationUnit">The translation unit.</param>
        /// <param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An ImportResult which represents the status of the operation (succeeded, ignored, etc).
        /// </returns>
        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            return this.AddOrUpdateTranslationUnit(translationUnit, false);
        }

        /// <summary>
        /// Adds an array of translation units to the database. If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param>
        /// <param name="settings">The settings used for this operation.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        /// status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            ImportResult[] importResultArray = new ImportResult[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                importResultArray[unitCount] = this.AddTranslationUnit(translationUnits[unitCount], settings);
            }

            return importResultArray;
        }

        /// <summary>
        /// Adds an array of translation units to the database, but will only add those
        /// for which the corresponding mask field is true. If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// </summary>
        /// <param name="translationUnits">An arrays of translation units to be added.</param>
        /// <param name="settings">The settings used for this operation.</param>
        /// <param name="mask">A bool array with the same cardinality as the TU array, specifying which TUs to add.</param>
        /// <returns>
        /// An array of ImportResult objects, which mirrors the translation unit array. It has the exact same size and contains the
        /// status of each add operation for each particular translation unit with the same index within the array.
        /// </returns>
        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            ImportResult[] importResultArray = new ImportResult[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                if (mask[unitCount])
                {
                    importResultArray[unitCount] = this.AddTranslationUnit(translationUnits[unitCount], settings);
                }
            }

            return importResultArray;
        }

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegment(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment)"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="segment">The segment to search for.</param>
        /// <returns>A <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> object</returns>
        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            var searchResults = this.SearchSegmentsInBulk(settings, new[] { segment });
            if ((searchResults != null) && searchResults.Any())
            {
                return searchResults[0];
            }

            return new SearchResults();
        }        

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegments(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[])"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="segments">The segment array.</param>
        /// <returns>An array of <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> objects.</returns>
        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            var searchResultsArray = this.SearchSegmentsInBulk(settings, segments);
            return searchResultsArray;
        }     

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchSegmentsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.Core.Segment[],System.Boolean[])"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="segments">The segment array.</param>
        /// <param name="mask">The segment array mask.</param>
        /// <returns>An array of <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> objects.</returns>
        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            var searchResultsArray = new SearchResults[segments.Length];
            var maskedSegments = segments.Where((t, segmentCount) => mask[segmentCount]).ToArray();

            var searchResults = this.SearchSegments(settings, maskedSegments);
            var index = 0;
            for (var i = 0; i < segments.Count(); i++)
            {
                if (!mask[i] || (searchResults.Count() <= index))
                {
                    continue;
                }

                searchResultsArray[i] = searchResults[index];
                index++;
            }

            return searchResultsArray;
        }

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchText(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,System.String)"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="segment">The segment to search for.</param>
        /// <returns>A <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> object.</returns>
        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            Segment newSegment = new Segment(this.SourceLanguage);
            newSegment.Add(segment);

            return this.SearchSegment(settings, newSegment);
        }

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit)"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="translationUnit">The translation unit to search for.</param>
        /// <returns>A <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> object.</returns>
        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            return this.SearchSegment(settings, translationUnit.SourceSegment);
        }

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnits(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[])"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="translationUnits">The translation units to search.</param>
        /// <returns>An array of <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> objects.</returns>
        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            Segment[] segments = new Segment[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                segments[unitCount] = translationUnits[unitCount].SourceSegment;
            }

            return this.SearchSegments(settings, segments);
        }

        /// <summary>
        /// Performs the equivalent of <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderLanguageDirection.SearchTranslationUnitsMasked(Sdl.LanguagePlatform.TranslationMemory.SearchSettings,Sdl.LanguagePlatform.TranslationMemory.TranslationUnit[],System.Boolean[])"/>,
        /// using the current search scope or search provider.
        /// </summary>
        /// <param name="settings">The search settings.</param>
        /// <param name="translationUnits">The translation units to search for.</param>
        /// <param name="mask">The translation units mask.</param>
        /// <returns>An array of <see cref="Sdl.LanguagePlatform.TranslationMemory.SearchResults">Search Results</see> objects.</returns>
        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            Segment[] segments = new Segment[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                segments[unitCount] = translationUnits[unitCount].SourceSegment;
            }

            return this.SearchSegmentsMasked(settings, segments, mask);
        }

        /// <summary>
        /// Updates the properties and fields of an existing translation unit if the source and target segments are unchanged or
        /// adds a new translation unit otherwise. If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// <para>The translation unit should be initialized in a previous call to the translation memory, so that the ID property is set to a
        /// valid value.</para>
        /// </summary>
        /// <param name="translationUnit">The translation unit to be updated.</param>
        /// <returns>The result of the operation.</returns>
        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            return this.AddOrUpdateTranslationUnit(translationUnit, true);
        }

        /// <summary>
        /// Updates the properties and fields of an array of existing translation units if the source and target segments are unchanged or
        /// adds new translation units otherwise. If the provider doesn't support adding/updating, the
        /// implementation should return a reasonable ImportResult but should not throw an exception.
        /// <para>The translation units should be initialized in previous calls to the translation memory, so that their ID properties
        /// are set to valid values.</para>
        /// </summary>
        /// <param name="translationUnits">The translation unit array to be updated.</param>
        /// <returns>
        /// An array of results which mirrors the translation unit array. It has the exact same size and contains the
        /// results for each translation unit with the same index within the translation unit array.
        /// </returns>
        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            ImportResult[] importResultArray = new ImportResult[translationUnits.Length];

            for (int unitCount = 0; unitCount < translationUnits.Length; unitCount++)
            {
                importResultArray[unitCount] = this.UpdateTranslationUnit(translationUnits[unitCount]);
            }

            return importResultArray;
        }

        #endregion

        /// <summary>
        /// A delegate function to compare two TM matches.
        /// </summary>
        /// <param name="x">The first match to compare.</param>
        /// <param name="y">The second match to compare.</param>
        /// <returns>An integer that indicates whether the first item is less than, equal to or greater than the second item.</returns>
        private static int CompareTmMatches(SdlxTm.TmMatch x, SdlxTm.TmMatch y)
        {
            return -x.MatchScore.CompareTo(y.MatchScore);
        }

        /// <summary>
        /// Searches the segments in bulk.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="segments">The segments.</param>
        /// <returns>the search results</returns>
        private SearchResults[] SearchSegmentsInBulk(SearchSettings settings, Segment[] segments)
        {
            // Get the string representation of the segment and the structure context.
            string searchContext = settings.CurrentStructureContext ?? "TEXT";

            // Transfer some settings across
            this.fuzzyThreshold = settings.MinScore;
            this.maximumMatches = settings.MaxResults;

            // Check whether this is a concordance search or not.
            return this.TmLookup(segments, searchContext, settings.IsConcordanceSearch);
        }

        /// <summary>
        /// Adds the or update translation unit in the TM.
        /// </summary>
        /// <param name="translationUnit">The translation unit.</param>
        /// <param name="update">If set to <c>true</c> any existing units will be updated.</param>
        /// <returns>An import result.</returns>
        private ImportResult AddOrUpdateTranslationUnit(TranslationUnit translationUnit, bool update)
        {
            string sourceSegment = this.EmitSegment(translationUnit.SourceSegment);
            string targetSegment = this.EmitSegment(translationUnit.TargetSegment);
            string context = "TEXT";

            ErrorCode result = this.TmUpdate(sourceSegment, targetSegment, context, update) ? ErrorCode.OK : ErrorCode.Other;
            ImportResult importResult = new ImportResult(update ? Sdl.LanguagePlatform.TranslationMemory.Action.Merge : Sdl.LanguagePlatform.TranslationMemory.Action.Add, result);

            return importResult;
        }

        /// <summary>
        /// Emits a string representation of a segment.
        /// </summary>
        /// <param name="searchSegment">The search segment.</param>
        /// <returns>The string representation of the segment.</returns>
        private string EmitSegment(Segment searchSegment)
        {
            string returnString = string.Empty;
            this.tagMap.Clear();
            string tagFormat = string.Empty;

            foreach (SegmentElement segmentElement in searchSegment.Elements)
            {
                if (segmentElement is Text)
                {
                    returnString += this.EmitText(((Text)segmentElement).Value);
                }
                else if (segmentElement is Tag)
                {
                    tagFormat = this.EmitTag((Tag)segmentElement);
                    this.tagMap.Add(new KeyValuePair<string, SegmentElement>(tagFormat, segmentElement));
                    returnString += tagFormat;
                }
                else if (segmentElement is TagToken)
                {
                    tagFormat = this.EmitTag(((TagToken)segmentElement).Tag);
                    this.tagMap.Add(new KeyValuePair<string, SegmentElement>(tagFormat, segmentElement));
                    returnString += tagFormat;
                }
                else if (segmentElement is Token)
                {
                    returnString += this.EmitText((segmentElement as Token).Text);
                }
            }

            return returnString;
        }

        /// <summary>
        /// Emits the text for a segmenet element.
        /// </summary>
        /// <param name="text">The text to emit.</param>
        /// <returns>An XML friendly representation of the text.</returns>
        private string EmitText(string text)
        {
            string returnString = text.Replace("&", "&amp;");
            returnString = returnString.Replace("<", "&lt;");
            returnString = returnString.Replace(">", "&gt;");

            return returnString;
        }

        /// <summary>
        /// Emits the tag for a segment element.
        /// </summary>
        /// <param name="tag">The tag to convert to text.</param>
        /// <returns>A string representation of the tag.</returns>
        private string EmitTag(Tag tag)
        {
            string returnString = string.Empty;

            switch (tag.Type)
            {
                case TagType.Start:
                    returnString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", (tag.AlignmentAnchor > 0) ? tag.AlignmentAnchor : tag.Anchor);
                    break;
                case TagType.End:
                    returnString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", (tag.AlignmentAnchor > 0) ? tag.AlignmentAnchor : tag.Anchor);
                    break;
                case TagType.Standalone:
                    returnString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}/>", (tag.AlignmentAnchor > 0) ? tag.AlignmentAnchor : tag.Anchor);
                    break;
                case TagType.TextPlaceholder:
                case TagType.LockedContent:
                    returnString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", (tag.AlignmentAnchor > 0) ? tag.AlignmentAnchor : tag.Anchor);
                    returnString += this.EmitText(tag.TextEquivalent ?? string.Empty);
                    returnString += string.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", (tag.AlignmentAnchor > 0) ? tag.AlignmentAnchor : tag.Anchor);
                    break;
                default:
                    break;
            }

            return returnString;
        }

        /// <summary>
        /// Gets a segment representation of a TM match string.
        /// </summary>
        /// <param name="matchResult">The match result.</param>
        /// <param name="createConcordanceRanges">If set to <c>true</c> concordance ranges will be created.</param>
        /// <returns>
        /// The segment representation of the match result.
        /// </returns>
        private Segment GetSegment(string matchResult, bool createConcordanceRanges)
        {
            Segment returnSegment = new Segment();
            string textSegment = string.Empty;
            this.tagStack.Clear();

            if (createConcordanceRanges)
            {
                this.concordanceRanges = new List<SegmentRange>();
            }

            int elementCount = 0;

            for (int i = 0; i < matchResult.Length; i++)
            {
                // Look for formatting
                if (matchResult[i] == '<')
                {
                    // Add in any existing text segments
                    if (!string.IsNullOrEmpty(textSegment))
                    {
                        elementCount++;
                        returnSegment.Elements.Add(new Text(this.GetText(textSegment)));
                        textSegment = string.Empty;
                    }

                    // Extract the tag
                    int j = matchResult.IndexOf('>', i);
                    string tag = matchResult.Substring(i, j - i + 1);

                    // Check it's not a hard-format tag
                    if (tag.IndexOfAny(new char[] { 'b', 'B', 'i', 'I', 'u', 'U' }) >= 0)
                    {
                        tag = string.Empty;
                    }
                    else
                    {
                        // Manage the tag stack
                        if (tag[1] == '/')
                        {
                            // End tag
                            if (this.tagStack.Count > 0)
                            {
                                // Are we expecting this tag?
                                if (this.tagStack.Peek() != tag.Substring(2, tag.Length - 3))
                                {
                                    // Not well-formed so ignore this tag
                                    tag = string.Empty;
                                }
                                else
                                {
                                    // Pop the start tag from the stack
                                    this.tagStack.Pop();

                                    // Create the segment range for concordance if necessary
                                    if (createConcordanceRanges)
                                    {
                                        Text lastSegment = returnSegment.Elements[elementCount - 1] as Text;
                                        SegmentPosition segmentPositionFrom = new SegmentPosition(elementCount - 1, 0);
                                        SegmentPosition segmentPositionInto = new SegmentPosition(elementCount - 1, lastSegment.Value.Length - 1);
                                        SegmentRange segmentRange = new SegmentRange(segmentPositionFrom, segmentPositionInto);
                                        this.concordanceRanges.Add(segmentRange);
                                    }
                                }
                            }
                            else
                            {
                                // Nothing on the stack so this is not valid
                                tag = string.Empty;
                            }
                        }
                        else if (tag[tag.Length - 2] != '/')
                        {
                            // Push the id of the start tag on to the stack
                            this.tagStack.Push(tag.Substring(1, tag.Length - 2));
                        }
                    }

                    // Look for the segment element in the map
                    foreach (KeyValuePair<string, SegmentElement> tagMapItem in this.tagMap)
                    {
                        if (tag == tagMapItem.Key)
                        {
                            returnSegment.Elements.Add(tagMapItem.Value);
                            break;
                        }
                    }

                    // Move the search to the end of the tag
                    i = j;
                }
                else
                {
                    // Add on to the existing text segment
                    textSegment += matchResult[i];
                }
            }

            // Append any outstanding text segments
            if (!string.IsNullOrEmpty(textSegment))
            {
                returnSegment.Elements.Add(new Text(this.GetText(textSegment)));
                textSegment = string.Empty;
            }

            // Deal with any hanging elements on the tag stack
            while (this.tagStack.Count > 0)
            {
                // Get the end tag format
                string tag = string.Format("</{0}>", this.tagStack.Pop());

                // Look for the segment element in the map
                foreach (KeyValuePair<string, SegmentElement> tagMapItem in this.tagMap)
                {
                    if (tag == tagMapItem.Key)
                    {
                        returnSegment.Elements.Add(tagMapItem.Value);
                        break;
                    }
                }
            }

            return returnSegment;
        }

        /// <summary>
        /// Converts from XML friendly format to plain text.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>Non-XML friendlt version of the text.</returns>
        private string GetText(string text)
        {
            string returnString = text.Replace("&gt;", ">");
            returnString = returnString.Replace("&lt;", "<");
            returnString = returnString.Replace("&nbsp;", "\x00a0");
            returnString = returnString.Replace("&amp;", "&");

            return returnString;
        }

        /// <summary>
        /// Sets the field value in the translation unit.
        /// </summary>
        /// <param name="translationMemoryMatch">The translation memory match.</param>
        /// <param name="translationUnit">The translation unit to applied the value to.</param>
        private void SetFieldValues(SdlxTm.TmMatch translationMemoryMatch, TranslationUnit translationUnit)
        {
            foreach (SdlxTm.TmField translationMemoryField in translationMemoryMatch.TargetMatch.Properties)
            {
                switch (translationMemoryField.Name)
                {
                    case "Created":
                        translationUnit.SystemFields.CreationDate = (DateTime)translationMemoryField.Value;
                        break;
                    case "Created By":
                        translationUnit.SystemFields.CreationUser = translationMemoryField.Value.ToString();
                        break;
                    case "Last Used":
                        translationUnit.SystemFields.UseDate = (DateTime)translationMemoryField.Value;
                        break;
                    case "Last Modified":
                        translationUnit.SystemFields.ChangeDate = (DateTime)translationMemoryField.Value;
                        break;
                    case "Modified By":
                        translationUnit.SystemFields.ChangeUser = translationMemoryField.Value.ToString();
                        break;
                    case "Used Count":
                        translationUnit.SystemFields.UseCount = Convert.ToInt32(translationMemoryField.Value.ToString());
                        break;
                    default:
                        if (!translationMemoryField.UserDefined)
                        {
                            switch (translationMemoryField.Name)
                            {
                                case "Source File":
                                    translationMemoryField.Name = PluginResources.FieldName_SourceFile;
                                    break;
                                case "Context":
                                    translationMemoryField.Name = PluginResources.FieldName_Context;
                                    break;
                            }
                        }

                        switch (translationMemoryField.Type)
                        {
                            case SdlxTm.TmFieldType.TmFieldTypeAttribute:
                                if (translationMemoryField.AllowMultiple)
                                {
                                    MultiplePicklistFieldValue mpfValue = new MultiplePicklistFieldValue(translationMemoryField.Name);
                                    string[] mpfValues = translationMemoryField.Value.ToString().Split(new char[] { ';' });
                                    foreach (string singleValue in mpfValues)
                                    {
                                        mpfValue.Add(singleValue);
                                    }

                                    translationUnit.FieldValues.Add(mpfValue);
                                }
                                else
                                {
                                    SinglePicklistFieldValue spfValue = new SinglePicklistFieldValue(translationMemoryField.Name, new PicklistItem(translationMemoryField.Value.ToString()));
                                    translationUnit.FieldValues.Add(spfValue);
                                }

                                break;
                            case SdlxTm.TmFieldType.TmFieldTypeDate:
                                DateTimeFieldValue dtfValue = new DateTimeFieldValue(translationMemoryField.Name, (DateTime)translationMemoryField.Value);
                                translationUnit.FieldValues.Add(dtfValue);
                                break;
                            case SdlxTm.TmFieldType.TmFieldTypeNumber:
                                IntFieldValue ifValue = new IntFieldValue(translationMemoryField.Name, Convert.ToInt32(translationMemoryField.Value.ToString()));
                                translationUnit.FieldValues.Add(ifValue);
                                break;
                            case SdlxTm.TmFieldType.TmFieldTypeText:
                                SingleStringFieldValue ssfValue = new SingleStringFieldValue(translationMemoryField.Name, translationMemoryField.Value.ToString());
                                translationUnit.FieldValues.Add(ssfValue);
                                break;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Applies the penalties from the match.
        /// </summary>
        /// <param name="appliedPenalties">The applied penalties.</param>
        /// <param name="translationMemoryMatch">The TM match from TMS.</param>
        /// <returns>The malus of the applied penalties</returns>
        private int ApplyPenalties(List<AppliedPenalty> appliedPenalties, SdlxTm.TmMatch translationMemoryMatch)
        {
            AppliedPenalty appliedPenalty = null;
            int malus = 0;

            // Apply penalty
            if (translationMemoryMatch.Penalties.ApplyPenalty > 0)
            {
                appliedPenalty = new AppliedPenalty(PenaltyType.ProviderPenalty, Convert.ToInt32(translationMemoryMatch.Penalties.ApplyPenalty));
                appliedPenalties.Add(appliedPenalty);
                malus += appliedPenalty.Malus;
            }

            // Automatch penalty
            if (translationMemoryMatch.Penalties.AutomatchPenalty > 0)
            {
                appliedPenalty = new AppliedPenalty(PenaltyType.AutoLocalization, Convert.ToInt32(translationMemoryMatch.Penalties.AutomatchPenalty));
                appliedPenalties.Add(appliedPenalty);
                malus += appliedPenalty.Malus;
            }

            // Context penalty
            if (translationMemoryMatch.Penalties.ContextPenalty > 0)
            {
                appliedPenalty = new AppliedPenalty(PenaltyType.Unknown, Convert.ToInt32(translationMemoryMatch.Penalties.ContextPenalty));
                appliedPenalties.Add(appliedPenalty);
                malus += appliedPenalty.Malus;
            }

            // Filter penalty
            if (translationMemoryMatch.Penalties.FilterPenalty > 0)
            {
                appliedPenalty = new AppliedPenalty(PenaltyType.FilterPenalty, Convert.ToInt32(translationMemoryMatch.Penalties.FilterPenalty));
                appliedPenalties.Add(appliedPenalty);
                malus += appliedPenalty.Malus;
            }

            return malus;
        }

        /// <summary>
        /// Looks up a segment in the SDLX translation memory.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="searchContext">The search context.</param>
        /// <param name="concordanceSearch">If set to <c>true</c> this is a concordance search.</param>
        /// <returns>
        /// An array of <see cref="SdlxTm.TmMatch" /> objects.
        /// </returns>
        private SearchResults[] TmLookup(IList<Segment> segments, string searchContext, bool concordanceSearch)
        {
            var searchStrings = new Dictionary<int, string>();
                        
            var i = 0;
            foreach (var segment in segments)
            {
                i++;
                searchStrings.Add(i, this.EmitSegment(segment));                
            }
            
            var translationMemoryFindXml = this.BuildTmFindRequestXml(searchStrings, searchContext, concordanceSearch);            
            var bulkSearchResults = new List<SearchResults>();
            int returnValue = SdlxTmTranslationProvider.TmController.Find(this.translationMemoryReference, ref translationMemoryFindXml);
            if (returnValue != 0)
            {
                return bulkSearchResults.ToArray();
            }

            // Now we have to parse the results            
            var xmlResults = new XmlDocument();
            xmlResults.LoadXml(translationMemoryFindXml);
            var xmlRoot = xmlResults.DocumentElement;

            foreach (var searchString in searchStrings)
            {
                var translationMemoryMatches = this.ExtractTmMatchesWithId(xmlRoot, searchString.Key, searchString.Value);
                var segment = segments[searchString.Key - 1];
                var searchResults = this.ConvertTmMatchesToSearchResults(
                    translationMemoryMatches,
                    segment,
                    concordanceSearch);
               
                bulkSearchResults.Add(searchResults);                
            }

            return bulkSearchResults.ToArray();
        }

        /// <summary>
        /// Builds the find XML.
        /// </summary>
        /// <param name="searchStrings">The search strings.</param>
        /// <param name="searchContext">The search context.</param>
        /// <param name="concordanceSearch">if set to <c>true</c> [concordance search].</param>
        /// <returns>the built request XML</returns>
        private string BuildTmFindRequestXml(Dictionary<int, string> searchStrings, string searchContext, bool concordanceSearch)
        {
            // Do a lookup in the TM in the sequence
            // Start of document
            StringBuilder sw = new StringBuilder();
            XmlWriter wr = XmlWriter.Create(sw);
            wr.WriteStartDocument();
            wr.WriteStartElement("tmcdata");

            // TM
            wr.WriteStartElement("tm");
            wr.WriteAttributeString("id", this.translationMemoryReference.ToString());

            // Search parameters
            wr.WriteStartElement("search_params");
            wr.WriteAttributeString("minmatch", this.fuzzyThreshold.ToString());
            wr.WriteAttributeString("max_matches", this.maximumMatches.ToString());
            wr.WriteAttributeString("field_values", "yes");
            if (concordanceSearch)
            {
                wr.WriteAttributeString("concordance", "yes");
            }

            wr.WriteAttributeString("usealtfuzzyscoring", this.useTradosScoring ? "yes" : "no");

            // Penalties
            wr.WriteStartElement("penalties");
            wr.WriteAttributeString("context", concordanceSearch ? "0" : this.contextPenalty.ToString());
            wr.WriteAttributeString("filter", "0");
            wr.WriteAttributeString("default", "0");
            wr.WriteEndElement();

            // Languages
            wr.WriteStartElement("languages");
            wr.WriteAttributeString("source", this.sourceLanguage);
            wr.WriteAttributeString("target", this.targetLanguage);
            wr.WriteEndElement();

            // Automatch
            wr.WriteStartElement("automatch");
            wr.WriteAttributeString("dates", this.useAutoMatch && !concordanceSearch ? "yes" : "no");
            wr.WriteAttributeString("numbers", this.useAutoMatch && !concordanceSearch ? "yes" : "no");
            wr.WriteAttributeString("acronyms", this.useAutoMatch && !concordanceSearch ? "yes" : "no");
            wr.WriteEndElement();

            // </search_params>
            wr.WriteEndElement();

            foreach (var searchString in searchStrings)
            {
                // Search data
                wr.WriteStartElement("search_data");
                wr.WriteAttributeString("id", searchString.Key.ToString(CultureInfo.InvariantCulture));
                wr.WriteStartElement("source");
                wr.WriteValue(searchString.Value);
                wr.WriteEndElement();
                wr.WriteStartElement("context");
                wr.WriteValue(concordanceSearch ? "TEXT" : searchContext);
                wr.WriteEndElement();
                wr.WriteEndElement();
            }

            // </tm>
            wr.WriteEndElement();

            // </tmcdata>
            wr.WriteEndElement();

            // Done
            wr.WriteEndDocument();
            wr.Flush();
            wr.Close();
            
            return sw.ToString();
        }

        /// <summary>
        /// Extracts the tm matches with identifier.
        /// </summary>
        /// <param name="xmlRoot">The XML root.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="searchString">The search string.</param>
        /// <returns>the list of <see cref="TmMatch"/> matching the given ID</returns>
        private List<TmMatch> ExtractTmMatchesWithId(XmlElement xmlRoot, int id, string searchString)
        {
            var translationMemoryMatches = new List<SdlxTm.TmMatch>();            
            var matchingResults = xmlRoot.SelectNodes(string.Format("//tm/tu[@id='{0}']", id.ToString()));

            if ((matchingResults == null) || (matchingResults.Count == 0))
            {
                return translationMemoryMatches;
            }

            // Find all the results
            foreach (XmlNode translationUnit in matchingResults)
            {
                // Create a new match and extract information from the <tu> element
                SdlxTm.TmMatch thisMatch = new SdlxTm.TmMatch();
                thisMatch.UniqueId = Convert.ToInt32(translationUnit.SelectSingleNode("@id").Value);
                thisMatch.TranslationId = Convert.ToInt32(translationUnit.SelectSingleNode("@transid").Value);
                thisMatch.MatchScore =
                    SdlxTm.TmPenalties.ConvertToDouble(translationUnit.SelectSingleNode("@match_score").Value, 75);

                // Record the penalties that have been applied to this match
                SdlxTm.TmPenalties penalties = new SdlxTm.TmPenalties(translationUnit);
                thisMatch.Penalties = penalties;
                thisMatch.Penalties.ApplyPenalty = 0;
                thisMatch.MatchScore -= thisMatch.Penalties.ApplyPenalty;

                // Find the source language
                string sourceLanguage = string.Empty;
                XmlNode srclangAttribute = translationUnit.SelectSingleNode("@srclang");
                if (srclangAttribute == null)
                {
                    sourceLanguage = translationUnit.SelectSingleNode("tuv/@xml:lang", null).Value;
                }
                else
                {
                    sourceLanguage = srclangAttribute.Value;
                }

                // Find the target language
                string targetLanguage =
                    translationUnit.SelectSingleNode("tuv[@xml:lang!='" + sourceLanguage + "']/@xml:lang", null)
                        .Value;

                // Load the source and target variants
                thisMatch.SourceMatch =
                    new SdlxTm.TmTranslationVariant(
                        translationUnit.SelectSingleNode("tuv[@xml:lang='" + sourceLanguage + "']", null));
                thisMatch.TargetMatch =
                    new SdlxTm.TmTranslationVariant(
                        translationUnit.SelectSingleNode("tuv[@xml:lang='" + targetLanguage + "']", null));

                // Don't give 100% to non identical matches
                if ((thisMatch.MatchScore == 100) && (thisMatch.SourceMatch.Text != searchString))
                {
                    thisMatch.MatchScore -= 1;
                }

                // Store this match if it's good enough
                if (thisMatch.MatchScore >= this.fuzzyThreshold)
                {
                    translationMemoryMatches.Add(thisMatch);
                }
            }

            // Now we need to sort the matches
            translationMemoryMatches.Sort(CompareTmMatches);

            // Truncate the list
            while (translationMemoryMatches.Count > this.maximumMatches)
            {
                translationMemoryMatches.RemoveAt(translationMemoryMatches.Count - 1);
            }

            return translationMemoryMatches;
        }

        /// <summary>
        /// Converts the tm matches to search results.
        /// </summary>
        /// <param name="translationMemoryMatches">The translation memory matches.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="concordanceSearch">if set to <c>true</c> [concordance search].</param>
        /// <returns>the <see cref="SearchResults"/></returns>
        private SearchResults ConvertTmMatchesToSearchResults(IEnumerable<TmMatch> translationMemoryMatches, Segment segment, bool concordanceSearch)
        {
            // Prepare our results
            SearchResults searchResults = new SearchResults();
            searchResults.SourceSegment = segment.Duplicate();

            // Tokenize the search segment ready to compare it to our result segment
            Tokenizer myTokenizer = new Tokenizer();
            searchResults.SourceSegment.Tokens = myTokenizer.Tokenize(searchResults.SourceSegment);
            SegmentEditDistanceComputer editDistanceComputer = new SegmentEditDistanceComputer();
            TagAssociations alignedTags = new TagAssociations();
            List<AppliedPenalty> appliedPenalties = new List<AppliedPenalty>();

            foreach (SdlxTm.TmMatch translationMemoryMatch in translationMemoryMatches)
            {
                // Accumulate applied penalties
                appliedPenalties.Clear();
                int malus = this.ApplyPenalties(appliedPenalties, translationMemoryMatch);

                // Get the source and target segments (collections of SegmentElements)
                Segment sourceSegment = this.GetSegment(translationMemoryMatch.SourceMatch.Text, concordanceSearch);
                Segment targetSegment = this.GetSegment(translationMemoryMatch.TargetMatch.Text, false);

                // Calculate the edit distance for this match
                sourceSegment.Tokens = myTokenizer.Tokenize(sourceSegment);
                EditDistance myEditDistance = editDistanceComputer.ComputeEditDistance(searchResults.SourceSegment.Tokens, sourceSegment.Tokens, false, BuiltinRecognizers.RecognizeAll, out alignedTags);

                // Create and populate the new translation unit
                TranslationUnit translationUnit = new TranslationUnit();
                translationUnit.SourceSegment = sourceSegment;
                translationUnit.TargetSegment = targetSegment;
                translationUnit.ResourceId = new PersistentObjectToken(translationUnit.GetHashCode(), Guid.Empty);
                translationUnit.Origin = TranslationUnitOrigin.TM;

                // Set up the scoring result
                ScoringResult scoringResult = new ScoringResult();
                scoringResult.BaseScore = Convert.ToInt32(Math.Floor(translationMemoryMatch.MatchScore + malus));
                scoringResult.EditDistance = myEditDistance;
                scoringResult.AppliedPenalties = appliedPenalties;
                scoringResult.IsStructureContextMatch = translationMemoryMatch.Penalties.ContextPenalty == 0;
                if (concordanceSearch)
                {
                    scoringResult.MatchingConcordanceRanges = this.concordanceRanges;
                }

                // Set up the search result
                SearchResult searchResult = new SearchResult(translationUnit);
                searchResult.ScoringResult = scoringResult;

                // Should never happen, but to be on the safe side:
                if (translationUnit.SystemFields == null)
                {
                    translationUnit.SystemFields = new SystemFields();
                }

                // Set up the field values
                this.SetFieldValues(translationMemoryMatch, translationUnit);

                // Create deep copy for the TP in case client modifies the TP but still wants to 
                // track the original "memory" TU
                searchResult.TranslationProposal = translationUnit.Duplicate();

                // Add this search result to the list
                searchResults.Add(searchResult);
            }

            return searchResults;
        }

        /// <summary>
        /// Update the working TM in the sequence with the requested segment
        /// </summary>
        /// <param name="sourceSegment">The source segment.</param>
        /// <param name="targetSegment">The target segment.</param>
        /// <param name="context">The segment context.</param>
        /// <param name="updateExisting">A value indicating whether existing segments should be overwritten.</param>
        /// <returns>Success or failure indicator</returns>
        private bool TmUpdate(string sourceSegment, string targetSegment, string context, bool updateExisting)
        {
            // Start of document
            StringBuilder sw = new StringBuilder();
            XmlWriter wr = XmlWriter.Create(sw);
            wr.WriteStartDocument();
            wr.WriteStartElement("tmcdata");

            // TM
            wr.WriteStartElement("tm"); // <tm>
            wr.WriteAttributeString("id", this.translationMemoryReference.ToString());
            wr.WriteAttributeString("action", "update");
            wr.WriteAttributeString("update_existing", updateExisting ? "yes" : "no");
            wr.WriteAttributeString("create_index", "yes");

            // Translation Unit
            wr.WriteStartElement("tu"); // <tu>
            wr.WriteAttributeString("id", "1");
            wr.WriteAttributeString("srclang", this.sourceLanguage);

            // Source segment
            wr.WriteStartElement("tuv"); // <tuv>
            wr.WriteAttributeString("xml", "lang", null, this.sourceLanguage);
            wr.WriteStartElement("seg"); // <seg>
            wr.WriteString(sourceSegment);
            wr.WriteEndElement(); // </seg>
            wr.WriteEndElement(); // </tuv>

            // Target segment
            wr.WriteStartElement("tuv"); // <tuv>
            wr.WriteAttributeString("xml", "lang", null, this.targetLanguage);

            // Context
            wr.WriteStartElement("prop"); // <prop>
            wr.WriteAttributeString("type", "x-ALS:Context");
            wr.WriteString(context);
            wr.WriteEndElement(); // </prop>

            // Source file
            wr.WriteStartElement("prop"); // <prop>
            wr.WriteAttributeString("type", "x-ALS:Source File");
            wr.WriteString(PluginResources.PluginProviderSdlxTm_SourceFile);
            wr.WriteEndElement(); // </prop>

            // Segment
            wr.WriteStartElement("seg"); // <seg>
            wr.WriteString(targetSegment);
            wr.WriteEndElement(); // </seg>
            wr.WriteEndElement(); // </tuv>
            wr.WriteEndElement(); // </tu>
            wr.WriteEndElement(); // </tm>
            wr.WriteEndElement(); // </tmcdata>

            // Finalise document
            wr.WriteEndDocument();
            wr.Flush();
            wr.Close();

            // Now update the tm
            string translationMemoryUpdateXml = sw.ToString();
            int returnValue = SdlxTmTranslationProvider.TmController.Update(this.translationMemoryReference, ref translationMemoryUpdateXml);

            return returnValue == 0;
        }
    }
}