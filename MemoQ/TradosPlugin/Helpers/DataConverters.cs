using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.FileTypeSupport.Framework.BilingualApi;

using TMProvider;
using AutoMapper;
using AutoMapper.Mappers;

namespace TradosPlugin
{
    public static class DataConverters
    {

        static DataConverters()
        {
            Mapper.AllowNullDestinationValues = true;

        }

        #region Trados - memoQ converters

        /* Trados segment:
         * http://producthelp.sdl.com/SDK/TranslationMemoryApi/3.0/html/c735a426-f836-29c0-b917-71840eb3e259.htm
         * 
        */
        /// <summary>
        /// Language: if the memoQ hit had a language code, and it could be mapped to a culturinfo, this will become the language.
        /// Otherwise we'll use the language that comes from Trados.        /// </summary>
        /// <param name="tmHit"></param>
        /// <param name="trSourceLang"></param>
        /// <param name="trTargetLang"></param>
        /// <returns></returns>
        public static Sdl.LanguagePlatform.TranslationMemory.TranslationUnit TMHitToTradosTU(TMHit tmHit, CultureInfo trSourceLang, CultureInfo trTargetLang)
        {
            Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu = MemoQTUToTradosTU(tmHit.TranslationUnit, trSourceLang, trTargetLang);
            tu.FieldValues.Add(new SingleStringFieldValue(AppData.TMNameFieldName, tmHit.TMName));
            tu.FieldValues.Add(new SingleStringFieldValue(AppData.TMGuidFieldName, tmHit.TMGuid.ToString()));

            if (tmHit.TMGuid != null) tu.FieldValues.Add(new SingleStringFieldValue(AppData.TMGuidFieldName, tmHit.TMGuid.ToString()));

            return tu;
        }

        /// <summary>
        /// Language: if the memoQ hit had a language code, and it could be mapped to a culturinfo, this will become the language.
        /// Otherwise we'll use the language that comes from Trados.        /// </summary>
        /// <param name="memoqTU"></param>
        /// <param name="trSourceLang"></param>
        /// <param name="trTargetLang"></param>
        /// <returns></returns>
        public static Sdl.LanguagePlatform.TranslationMemory.TranslationUnit MemoQTUToTradosTU(TMProvider.TranslationUnit memoqTU, CultureInfo trSourceLang, CultureInfo trTargetLang)
        {
            SegmentXml segConverter = new SegmentXml();
            string sPar, tPar;
            CultureInfo sc = null, tc = null;

            Segment s = segConverter.Xml2Segment(memoqTU.SourceSegment, out sPar);
            //if (!String.IsNullOrEmpty(memoqTU.SourceLangCode)) sc = LanguageMapper.GetCultureInfoFrom3LetterCode(memoqTU.SourceLangCode);
            sc = trSourceLang;
            s.Culture = sc;
            if (sc != null) s.CultureName = sc.Name;
            Segment t = segConverter.Xml2Segment(memoqTU.TargetSegment, out tPar);
            //if (!String.IsNullOrEmpty(memoqTU.SourceLangCode)) tc = LanguageMapper.GetCultureInfoFrom3LetterCode(memoqTU.SourceLangCode);
            tc = trTargetLang;
            t.Culture = tc;
            if (tc != null) t.CultureName = tc.Name;

            Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu = new Sdl.LanguagePlatform.TranslationMemory.TranslationUnit(s, t);
            tu.SystemFields.ChangeDate = memoqTU.Modified;
            tu.SystemFields.ChangeUser = memoqTU.Modifier;
            tu.SystemFields.CreationDate = memoqTU.Created;
            tu.SystemFields.CreationUser = memoqTU.Creator;
            tu.SystemFields.UseDate = memoqTU.Modified;
            tu.SystemFields.UseUser = memoqTU.Modifier;
            // we don't know the rest (when it was last used, etc.)
            // other metadata
            tu.FieldValues = new FieldValues();
            // avarga del !!! Localize !!! Localize???
            if (!String.IsNullOrEmpty(memoqTU.Document) && memoqTU.Document != " ") tu.FieldValues.Add(new SingleStringFieldValue("Document", memoqTU.Document));
            if (!String.IsNullOrEmpty(memoqTU.ContextID) && memoqTU.ContextID != " ") tu.FieldValues.Add(new SingleStringFieldValue("Context ID", memoqTU.ContextID));
            if (!String.IsNullOrEmpty(memoqTU.Client) && memoqTU.Client != " ") tu.FieldValues.Add(new SingleStringFieldValue("Client", memoqTU.Client));
            if (!String.IsNullOrEmpty(memoqTU.Subject) && memoqTU.Subject != " ") tu.FieldValues.Add(new SingleStringFieldValue("Subject", memoqTU.Subject));
            if (!String.IsNullOrEmpty(memoqTU.Domain) && memoqTU.Domain != " ") tu.FieldValues.Add(new SingleStringFieldValue("Domain", memoqTU.Domain));
            // add key as a meta field, so that we know it when we update/delete
            tu.FieldValues.Add(new IntFieldValue(AppData.KeyFieldName, memoqTU.Key));

            // avarga del - I don't know how I could assign this (sPar, tPar)
            // tu.DocumentSegmentPair.Source.ParentParagraph.
            //tu.ComputePlaceables();
            //tu.Format = TranslationUnitFormat.Unknown;
            tu.Origin = TranslationUnitOrigin.TM;
            tu.ConfirmationLevel = Sdl.Core.Globalization.ConfirmationLevel.Translated;
            // tu.ResourceId = new PersistentObjectToken(tmName, tmGuid);
            // avarga del - what else? contexts? structurecontexts?

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);
            return tu;
        }

        // from TM hit to SearchResult
        /// <summary>
        /// Language: if the memoQ hit had a language code, and it could be mapped to a culturinfo, this will become the language.
        /// Otherwise we'll use the language that comes from Trados.
        /// </summary>
        /// <param name="tmHit"></param>
        /// <param name="trSourceLang"></param>
        /// <param name="trTargetLang"></param>
        /// <returns></returns>
        public static SearchResult TMHitToTradosSearchResult(TMHit tmHit, CultureInfo trSourceLang, CultureInfo trTargetLang)
        {

            Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tmTU = TMHitToTradosTU(tmHit, trSourceLang, trTargetLang);
            SearchResult item = new SearchResult(tmTU);
            // avarga del - what are these??
            //item.ContextData = new TuContext();
            // avarga del TODO ??? to add tags as placeables?
            //item.MemoryPlaceables = new List<Placeable>();

            item.MemoryTranslationUnit = tmTU;
            // avarga del TODO ??? add tags as placeable associations?
            //item.PlaceableAssociations = new List<PlaceableAssociation>();
            item.ScoringResult = new ScoringResult();
            item.ScoringResult.BaseScore = tmHit.MatchRate;
            item.TranslationProposal = tmTU;
            return item;

        }

        public static QuerySegment TradosTUToQuerySegment(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tradosTU, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit precedingTradosTU, Sdl.LanguagePlatform.TranslationMemory.TranslationUnit followingTradosTU)
        {
            SegmentXml segConverter = new SegmentXml();
            string xmlSegment = segConverter.Segment2Xml(tradosTU, true);
            // following segment
            string following;
            if (precedingTradosTU == null) following = null;
            else following = segConverter.Segment2Xml(precedingTradosTU, true);
            string preceding;
            if (followingTradosTU == null) preceding = null;
            else preceding = segConverter.Segment2Xml(followingTradosTU, true);
            // disable context for now !!!
            return new QuerySegment("", xmlSegment, null, null); //preceding, following);
        }

        public static QuerySegment TradosSegmentToQuerySegment(Segment tradosSegment, Segment precedingTradosSegment, Segment followingTradosSegment)
        {
            SegmentXml segConverter = new SegmentXml();
            string xmlSegment = segConverter.Segment2Xml(tradosSegment);
            // following segment
            string preceding;
            if (precedingTradosSegment == null) preceding = null;
            else preceding = segConverter.Segment2Xml(precedingTradosSegment);
            string following;
            if (followingTradosSegment == null) following = null;
            else following = segConverter.Segment2Xml(followingTradosSegment);
            // disable context for now !!!
            return new QuerySegment("", xmlSegment, null, null);// preceding, following);
        }

        public static SearchResults TradosSearchResultsFromTradosSegment(Segment segment)
        {
            SortSpecification sortSpec = new SortSpecification();
            sortSpec.Add(new SortCriterium("ScoringResult", SortDirection.Descending));
            sortSpec.Add(new SortCriterium("ChangeDate", SortDirection.Descending));

            SearchResults results = new SearchResults(sortSpec);
            // add source tags
            //results.DocumentPlaceables = new List<Placeable>();
            //int ix = 0;
            //foreach (SegmentElement se in segment.Elements)
            //{
            //    if (!(se is Tag)) continue;
            //    Tag tag = se as Tag;
            //    // avarga del ??? how would I know what's in the target, when there are several targets?
            //    PlaceableType pt = PlaceableType.None;
            //    switch (tag.Type)
            //    {
            //        case TagType.End:
            //            pt = PlaceableType.PairedTagEnd;
            //            break;
            //        case TagType.LockedContent:
            //            pt = PlaceableType.LockedContent;
            //            break;
            //        case TagType.Standalone:
            //            pt = PlaceableType.StandaloneTag;
            //            break;
            //        case TagType.Start:
            //            pt = PlaceableType.PairedTagStart;
            //            break;
            //        case TagType.TextPlaceholder:
            //            pt = PlaceableType.TextPlaceholder;
            //            break;
            //        case TagType.Undefined:
            //            pt = PlaceableType.None;
            //            break;
            //        case TagType.UnmatchedEnd:
            //            pt = PlaceableType.None;
            //            break;
            //        case TagType.UnmatchedStart:
            //            pt = PlaceableType.None;
            //            break;
            //    }
            //    results.DocumentPlaceables.Add(new Placeable(pt, ix, -1));
            //    ix++;
            //}

            results.SourceSegment = segment;
            //result.SourceHash = s.GetHashCode();
            // avarga del: does this have to be provided?

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradosSettings"></param>
        /// <param name="reverseLookup">Only lookup in the reverse language direction.</param>
        /// <returns></returns>
        public static LookupSegmentRequest TradosLookupSettingsToMemoQOptions(SearchSettings tradosSettings, bool reverseLookup)
        {
            // avarga del ??? how to convert?
            LookupSegmentRequest options = new LookupSegmentRequest(tradosSettings.MinScore, tradosSettings.MaxResults == 1, false, false, reverseLookup, InlineTagStrictness.Permissive);
            return options;
        }

        public static ConcordanceRequest TradosConcordanceSettingsToMemoQOptions(SearchSettings tradosSettings, GeneralTMSettings tmSettings)
        {
            Column col = tradosSettings.Mode == SearchMode.TargetConcordanceSearch ? Column.Target : Column.Source;
            bool reverse = tradosSettings.Mode == SearchMode.TargetConcordanceSearch;
            ConcordanceRequest options = new ConcordanceRequest(0, 0, false, tmSettings.ConcordanceCaseSensitive, col, FirstLast.First, true,
                Meta.User, tmSettings.ConcordancNumericEquivalence, reverse, null, 1000);
            return options;
        }

        public static TMProvider.TranslationUnit TradosTUToMemoQTU(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tradosTU, Segment precedingSeg, Segment followingSeg, string sourceMemoQLangCode, string targetMemoQLangCode)
        {
            SegmentXml segConverter = new SegmentXml();
            TMProvider.TranslationUnit mqTU = new TMProvider.TranslationUnit();
            mqTU.Created = tradosTU.SystemFields.CreationDate;
            mqTU.Creator = tradosTU.SystemFields.CreationUser;
            if (tradosTU.DocumentProperties != null) mqTU.Document = Path.GetFileName(tradosTU.DocumentProperties.LastOpenedAsPath);
            mqTU.PrecedingSegment = segConverter.Segment2Xml(precedingSeg);
            mqTU.FollowingSegment = segConverter.Segment2Xml(followingSeg);
            mqTU.Modified = tradosTU.SystemFields.ChangeDate;
            mqTU.Modifier = tradosTU.SystemFields.ChangeUser;
            mqTU.SourceSegment = segConverter.Segment2Xml(tradosTU, true);
            mqTU.TargetSegment = segConverter.Segment2Xml(tradosTU, false);
            mqTU.SourceLangCode = sourceMemoQLangCode;
            mqTU.TargetLangCode = targetMemoQLangCode;
            // other meta fields
            if (tradosTU.FieldValues != null)
            {
                mqTU.Key = tradosTU.FieldValues.Exists(AppData.KeyFieldName) ? Int32.Parse(tradosTU.FieldValues[AppData.KeyFieldName].GetValueString().Trim('"')) : 0;
                mqTU.Subject = tradosTU.FieldValues.Exists("Subject") ? tradosTU.FieldValues["Subject"].GetValueString() : "";
                mqTU.Project = tradosTU.FieldValues.Exists("Project") ? tradosTU.FieldValues["Project"].GetValueString() : "";
                mqTU.Client = tradosTU.FieldValues.Exists("Client") ? tradosTU.FieldValues["Client"].GetValueString() : "";
                mqTU.Domain = tradosTU.FieldValues.Exists("Domain") ? tradosTU.FieldValues["Domain"].GetValueString() : "";
                mqTU.ContextID = tradosTU.FieldValues.Exists("Context ID") ? tradosTU.FieldValues["Context ID"].GetValueString() : null;
            }
            // no custom meta yet
            // I don't have information about the rest :(
            // for example key??
            return mqTU;
        }

        /// <summary>
        /// Language: if the memoQ concordance result had a language code, and it could be mapped to a culturinfo, this will become the language.
        /// Otherwise we'll use the language that comes from Trados.        /// </summary>
        /// <param name="concordanceResult"></param>
        /// <param name="searchText"></param>
        /// <param name="allRes"></param>
        /// <param name="trSourceLang"></param>
        /// <param name="trTargetLang"></param>
        public static void AddConcordanceToTradosSearchResults(ConcordanceResult concordanceResult, string searchText, ref SearchResults allRes, bool sourceSearch, CultureInfo trSourceLang, CultureInfo trTargetLang)
        {
            foreach (ConcordanceItem item in concordanceResult.ConcResult)
            {
                Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu = MemoQTUToTradosTU(item.TMEntry, trSourceLang, trTargetLang);
                // remove entry ID meta field (no meaning in concordance)
                if(tu.FieldValues.Exists(TMProvider.AppData.KeyFieldName)) tu.FieldValues.Remove(TMProvider.AppData.KeyFieldName);
                SearchResult sr = new SearchResult(tu);
                sr.MemoryTranslationUnit = tu;
                sr.ScoringResult = new ScoringResult();
                //sr.ScoringResult.BaseScore = 
                sr.TranslationProposal = tu;
                sr.ScoringResult.MatchingConcordanceRanges = new List<SegmentRange>();
                //foreach (RangeInfo range in item.ConcordanceTextRanges)
                //{
                //    // find trados segmentelement index from the memoQ text position
                //    // avarga del !!! TODO
                //}
                Segment tuSegment = sourceSearch ? tu.SourceSegment : tu.TargetSegment;
                sr.ScoringResult.MatchingConcordanceRanges = collectConcordanceRanges(tuSegment, searchText);

                allRes.Add(sr);
            }
        }

        private static List<SegmentRange> collectConcordanceRanges(Segment segment, string searchString)
        {
            List<SegmentRange> concordanceMatchRanges = new List<SegmentRange>();
            int elIx = 0;
            foreach (var element in segment.Elements)
            {
                var text = element as Text;
                if (text != null && !String.IsNullOrEmpty(text.Value))
                {
                    int index = text.Value.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
                    while (index >= 0 && index < text.Value.Length)
                    {
                        SegmentRange segmentRange = new SegmentRange(elIx, index, index + searchString.Length - 1);
                        concordanceMatchRanges.Add(segmentRange);

                        index += searchString.Length;
                        if (index < text.Value.Length) index = text.Value.IndexOf(searchString, index, StringComparison.OrdinalIgnoreCase);
                    }

                }
                elIx++;
            }
            return concordanceMatchRanges;

        }

        #endregion

        #region Extension methods for memoQ to Trados

        /// <summary>
        /// Language: if the memoQ concordance result had a language code, and it could be mapped to a culturinfo, this will become the language.
        /// Otherwise we'll use the language that comes from Trados.        /// </summary>
        /// </summary>
        /// <param name="concordanceResult"></param>
        /// <param name="allRes"></param>
        /// <param name="trSourceLang"></param>
        /// <param name="trTargetLang"></param>
        public static void AddToTradosSearchResults(this ConcordanceResult concordanceResult, ref SearchResults allRes, CultureInfo trSourceLang, CultureInfo trTargetLang)
        {
            foreach (ConcordanceItem item in concordanceResult.ConcResult)
            {
                Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tu = MemoQTUToTradosTU(item.TMEntry, trSourceLang, trTargetLang);
                SearchResult sr = new SearchResult(tu);
                allRes.Add(sr);
            }
        }

        #endregion

        #region Extension methods for Trados to memoQ


        #endregion


    }
}
