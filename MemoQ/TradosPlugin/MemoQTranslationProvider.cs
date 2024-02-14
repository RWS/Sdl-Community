using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Sdl.LanguagePlatform.Core;
using SdlTM = Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

using TMProvider;

namespace TradosPlugin
{
    /// <summary>
    /// A general translation provider for all different types of memoQ TM providers.
    /// </summary>
    public class MemoQTranslationProvider : ITranslationProvider
    {
        #region memoQ-related members

        // Trados calls these methods from different threads (at least the lookup)
        private object synchObject = new object();

        private List<TMProviderBase> memoQTMProviders = null;
        /// <summary>
        /// The TM providers of any kind.
        /// </summary>
        public List<TMProviderBase> MemoQTMProviders
        {
            get { return memoQTMProviders; }
            set { memoQTMProviders = value; }
        }

        private GeneralTMSettings generalTMSettings;
        /// <summary>
        /// Settings for storing options for all TMs in all the providers.
        /// </summary>
        public GeneralTMSettings GeneralTMSettings
        {
            get { return generalTMSettings; }
            set { generalTMSettings = value; }
        }

        private List<Exception> exceptionList = new List<Exception>();

        public MemoQTranslationProvider(GeneralTMSettings generalSettings, List<TMProviderBase> tmProviders)
        {
            if (tmProviders == null)
            {
                memoQTMProviders = new List<TMProviderBase>();
            }
            else this.memoQTMProviders = tmProviders;
            this.generalTMSettings = generalSettings;
        }

        public void AddMemoQProvider(TMProviderBase tmProvider)
        {
            if (tmProvider != null) memoQTMProviders.Add(tmProvider);
        }

        public void RemoveMemoQProvider(string hostURL)
        {
            int ix = memoQTMProviders.FindIndex(p => p.Settings.URL == hostURL);
            if (ix != -1) memoQTMProviders.RemoveAt(ix);
        }

        public SdlTM.SearchResults[] AsyncLookup(SdlTM.SearchSettings settings, SdlTM.TranslationUnit[] translationUnits, bool[] mask,
            CultureInfo sLang, CultureInfo tLang, bool canReverseLandDir)
        {
            SdlTM.SearchResults[] res = lookupTranslationUnits(settings, translationUnits, mask, sLang, tLang, canReverseLandDir);
            return res;
        }

        public SdlTM.SearchResults[] AsyncLookup(SdlTM.SearchSettings settings, Segment[] segments, bool[] mask,
            CultureInfo sLang, CultureInfo tLang, bool canReverseLandDir)
        {
            SdlTM.SearchResults[] res = lookupSegments(settings, segments, mask, sLang, tLang, canReverseLandDir);
            return res;
        }

        public SdlTM.ImportResult[] AsyncUpdate(SdlTM.TranslationUnit[] translationUnits, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang,
            bool[] mask)
        {
            SdlTM.ImportResult[] res = updateEntries(translationUnits, tradosSourceLang, tradosTargetLang, mask);
            return res;
        }

        public SdlTM.ImportResult[] AsyncAdd(SdlTM.TranslationUnit[] translationUnits, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang,
           bool[] mask)
        {
            SdlTM.ImportResult[] res = addEntries(translationUnits, tradosSourceLang, tradosTargetLang, mask);
            return res;
        }

        public SdlTM.SearchResults AsyncConcordance(SdlTM.SearchSettings settings, string segment, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang)
        {
            SdlTM.SearchResults res = concordance(settings, segment, tradosSourceLang, tradosTargetLang);
            return res;
        }

        #endregion

        #region Conversion and lookup/update/etc.

        /// <summary>
        /// Does a lookup for the source segments of the translation units, but only for those where the mask is set to true.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="translationUnits"></param>
        /// <param name="mask">Which TU to look up. Can be null: all of them has to be looked up.</param>
        /// <returns></returns>
        private SdlTM.SearchResults[] lookupTranslationUnits(SdlTM.SearchSettings settings, SdlTM.TranslationUnit[] translationUnits, bool[] mask,
            CultureInfo tradosSourceLang, CultureInfo tradosTargetLang, bool canReverseLangDir)
        {
            lock (synchObject)
            {
                int allSegCount = translationUnits.Length;
                SdlTM.SearchResults[] allRes = new SdlTM.SearchResults[allSegCount];
                try
                {
                    bool lookupAll = false;
                    if (mask == null) lookupAll = true;
                    List<TMProvider.QuerySegment> lookupSegments = new List<TMProvider.QuerySegment>();

                    // lookup
                    for (int i = 0; i < allSegCount; i++)
                    {
                        // init search results with empty lists
                        allRes[i] = DataConverters.TradosSearchResultsFromTradosSegment(translationUnits[i].SourceSegment);
                        allRes[i].Results = new List<SdlTM.SearchResult>();
                        // only add those that need to be looked up
                        // if lookupAll is true, then mask is null!
                        if (!lookupAll && !mask[i]) continue;
                        SdlTM.TranslationUnit tu = translationUnits[i];
                        // disable context for now !!!
                        TMProvider.QuerySegment qs = DataConverters.TradosTUToQuerySegment(tu, null, null);// i == 0 ? null : translationUnits[i - 1], i == allSegCount - 1 ? null : translationUnits[i + 1]);
                        lookupSegments.Add(qs);
                    }
                    lookupQuerySegments(lookupSegments, settings, allSegCount, mask, tradosSourceLang, tradosTargetLang, ref allRes, canReverseLangDir);
                    // adjust tags to match the source
                    int k = 0;

                    foreach (SdlTM.SearchResults r in allRes)
                    {
                        for (int s = 0; s < r.Count; s++)
                        {
                            SdlTM.SearchResult sr = r.Results[s];
                            adjustTags(translationUnits[k].SourceSegment, ref sr);
                        }
                        k++;
                    }
                    return allRes;
                }
                catch (Exception e)
                {
                    //System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("TMTBPluginResources", typeof(TradosPlugin.MyTranslationProviderLanguageDirection).Assembly);
                    exceptionList.Add(new LookupException(PluginResources.Error_Lookup, e));
                    return allRes;
                }
            }
        }

        /// <summary>
        /// Does a lookup for the source segments of the translation units, but only for those where the mask is set to true.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="translationUnits"></param>
        /// <param name="mask">Which TU to look up. Can be null: all of them has to be looked up.</param>
        /// <returns></returns>
        private SdlTM.SearchResults[] lookupSegments(SdlTM.SearchSettings settings, Segment[] segments, bool[] mask,
            CultureInfo tradosSourceLang, CultureInfo tradosTargetLang, bool canReverseLangDir)
        {
            // Studio calls lookup asynchronously -> had to make this thread-safe
            lock (synchObject)
            {
                int allSegCount = segments.Length;
                SdlTM.SearchResults[] allRes = new SdlTM.SearchResults[allSegCount];
                try
                {
                    List<TMProvider.QuerySegment> lookupSegments = new List<TMProvider.QuerySegment>();

                    // lookup
                    for (int i = 0; i < segments.Length; i++)
                    {
                        // init search results with empty lists
                        allRes[i] = DataConverters.TradosSearchResultsFromTradosSegment(segments[i]);
                        //allRes[i].Results = new List<SdlTM.SearchResult>();
                        // only add those that need to be looked up
                        if (mask != null && !mask[i]) continue;
                        Segment tu = segments[i];
                        TMProvider.QuerySegment qs = DataConverters.TradosSegmentToQuerySegment(tu, i == 0 ? null : segments[i - 1], i == segments.Length - 1 ? null : segments[i + 1]);
                        lookupSegments.Add(qs);
                    }
                    // do the lookup
                    lookupQuerySegments(lookupSegments, settings, allSegCount, mask, tradosSourceLang, tradosTargetLang, ref allRes, canReverseLangDir);
                    // adjust tags to match the source
                    int k = 0;
                    foreach (SdlTM.SearchResults r in allRes)
                    {
                        for (int s = 0; s < r.Count; s++)
                        {
                            SdlTM.SearchResult sr = r.Results[s];
                            adjustTags(segments[k], ref sr);
                        }
                        k++;
                    }
                    return allRes;
                }
                catch (Exception e)
                {
                    //System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("TMTBPluginResources", typeof(TradosPlugin.MyTranslationProviderLanguageDirection).Assembly);
                    exceptionList.Add(new LookupException(PluginResources.Error_Lookup, e));
                    return allRes;
                }
            }
        }

        private void lookupQuerySegments(List<TMProvider.QuerySegment> lookupSegments, SdlTM.SearchSettings settings, int allSegCount, bool[] mask,
            CultureInfo tradosSourceLang, CultureInfo tradosTargetLang, ref SdlTM.SearchResults[] allRes, bool canReverseLangDir)
        {
            // Studio calls lookup asynchronously -> had to make this thread-safe
            lock (synchObject)
            {

                bool lookupAll = false;
                if (mask == null) lookupAll = true;

                string sLang = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosSourceLang.Name);
                string tLang = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosTargetLang.Name);
                // if memoQ doesn't support these languages
                if (sLang == null || tLang == null) return;
                bool[] threadsRanOK;

                TMProvider.LookupSegmentRequest memoQOptions = DataConverters.TradosLookupSettingsToMemoQOptions(settings, true);
                TMProvider.LookupData data = new LookupData(Operations.Lookup, lookupSegments, null, sLang, tLang, memoQOptions, null, canReverseLangDir,
                    !settings.CheckMatchingSublanguages, settings.MinScore);

                // go through each provider and do the lookup for the selected TMs
                // in one thread for each provider
                doThreadOperationsForAllProviders(data, out threadsRanOK);

                // merge and convert all the results
                foreach (TMProviderBase provider in this.memoQTMProviders)
                {
                    // if there's no result 
                    if (provider.LookupResults == null) continue;

                    // we have the data, clean it up: it only contains data for the unmasked segments, but the result needs all
                    // merge the results with all the hits
                    // watch out - there are some TUs where search was not performed, and these are not in the returned hits
                    for (int i = 0, current = 0; i < allSegCount; i++)
                    {
                        // if lookupAll is true, then mask is null!
                        if (!lookupAll && !mask[i]) continue;
                        // no hits for this segment
                        if (provider.LookupResults[current] == null)
                        {
                            current++;
                            continue;
                        }

                        // add each hit to the SearchResults
                        for (int j = 0; j < provider.LookupResults[current].Count; j++)
                        {
                            SdlTM.SearchResult oneHit = DataConverters.TMHitToTradosSearchResult(provider.LookupResults[current][j], tradosSourceLang, tradosTargetLang);
                            //allRes[i].Results.Add(oneHit);
                            // filter out matches that are below the threshold
                            // #27104 - because fragment hits might also come back with very low score
                            if(oneHit.ScoringResult.BaseScore >= settings.MinScore) allRes[i].Add(oneHit);
                        }

                        current++;
                    }
                }

                // decide on multiple translations
                foreach (SdlTM.SearchResults r in allRes)
                {
                    bool multipleTranslations = r.Results.Count(unit => unit.ScoringResult.BaseScore >= 100) > 1;
                    r.MultipleTranslations = true; // multipleTranslations;
                }

                // throw new error if there was something wrong with some providers
                string badProviders = checkProvidersForErrors(threadsRanOK);
                if (!String.IsNullOrEmpty(badProviders))
                {
                    exceptionList.Add(new Exception(String.Format(PluginResources.Error_NoResult_Providers, badProviders)));
                }
            }
        }

        private SdlTM.ImportResult[] updateEntries(SdlTM.TranslationUnit[] translationUnits, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang,
            bool[] mask)
        {
            try
            {
                bool[] threadsRanOK;
                LookupData data = collectDataForUpdateAdd(Operations.Update, tradosSourceLang, tradosTargetLang, translationUnits, mask);
                SdlTM.ImportResult[] importRes = createResultForUpdateAdd(Sdl.LanguagePlatform.TranslationMemory.Action.Overwrite, translationUnits, mask);

                // go through each provider and do the update for the selected TMs
                // in one thread for each provider
                if (data != null) doThreadOperationsForAllProviders(data, out threadsRanOK);
                else return importRes;

                // show message if there was something wrong with some providers
                string badProviders = checkProvidersForErrors(threadsRanOK);
                if (!String.IsNullOrEmpty(badProviders))
                {
                    ExceptionHelper.ShowMessage(new Exception(String.Format(PluginResources.Error_Update_Providers, badProviders)));
                }
                return importRes;
            }

            catch (Exception e)
            {
                exceptionList.Add(e);
                //throw new Exception("An error came from the TM provider during update. " + e.Message);
                return null;
            }

        }

        private SdlTM.ImportResult[] addEntries(SdlTM.TranslationUnit[] tus, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang, bool[] mask)
        {
            try
            {
                LookupData data = collectDataForUpdateAdd(Operations.Add, tradosSourceLang, tradosTargetLang, tus, mask);
                bool[] threadsRanOK;
                SdlTM.ImportResult[] res = createResultForUpdateAdd(Sdl.LanguagePlatform.TranslationMemory.Action.Add, tus, mask);

                if (data != null) doThreadOperationsForAllProviders(data, out threadsRanOK);
                else return res;
                // show message if there was something wrong with some providers
                string badProviders = checkProvidersForErrors(threadsRanOK);
                if (!String.IsNullOrEmpty(badProviders))
                {
                    exceptionList.Add(new Exception(String.Format(PluginResources.Error_Add_Providers, badProviders)));
                }

                return res;
            }

            catch (Exception e)
            {
                exceptionList.Add(e);
                //throw new Exception("An error came from the TM provider during update. " + e.Message);
                return null;
            }
        }

        private SdlTM.SearchResults concordance(SdlTM.SearchSettings settings, string segment, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang)
        {
            SdlTM.SearchResults allRes = new SdlTM.SearchResults();
            allRes.Results = new List<SdlTM.SearchResult>();
            string sLang, tLang;
            sLang = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosSourceLang.Name);
            tLang = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosTargetLang.Name);
            // if memoQ doesn't support these languages
            if (sLang == null || tLang == null) return allRes;

            QuerySegment qs = new QuerySegment(null, segment, null, null);
            TMProvider.ConcordanceRequest memoqOptions = DataConverters.TradosConcordanceSettingsToMemoQOptions(settings, generalTMSettings);
            LookupData data = new LookupData(Operations.Concordance, new List<QuerySegment> { qs }, null, sLang, tLang, null, memoqOptions, generalTMSettings.CanReverseLangDir,
                !settings.CheckMatchingSublanguages, settings.MinScore);

            bool[] threadsRanOK;
            doThreadOperationsForAllProviders(data, out threadsRanOK);

            // gather and convert results
            foreach (TMProviderBase provider in memoQTMProviders)
            {
                if (provider.ConcordanceResults == null) continue;
                foreach (ConcordanceResult r in provider.ConcordanceResults)
                {
                    DataConverters.AddConcordanceToTradosSearchResults(r, segment, ref allRes, settings.Mode == SdlTM.SearchMode.ConcordanceSearch, tradosSourceLang, tradosTargetLang);
                }
            }

            if (allRes.Results != null && allRes.Results.Count > 0) allRes.SourceSegment = allRes.Results[0].TranslationProposal.SourceSegment;

            // show message if there was something wrong with some providers
            string badProviders = checkProvidersForErrors(threadsRanOK);
            if (!String.IsNullOrEmpty(badProviders))
            {
                exceptionList.Add(new Exception(String.Format(PluginResources.Error_Concordance_Providers, badProviders)));
            }

            return allRes;
        }

        private void doThreadOperationsForAllProviders(LookupData data, out bool[] threadsRanOK)
        {
            int i = 0;
            //ManualResetEvent[] finishedEvents = new ManualResetEvent[memoQTMProviders.Count];
            threadsRanOK = new bool[memoQTMProviders.Count];
            Task[] tasks = new Task[memoQTMProviders.Count];
            foreach (TMProviderBase provider in this.memoQTMProviders)
            {
                try
                {
                    // https://msdn.microsoft.com/en-us/library/dd537609%28v=vs.110%29.aspx
                    // start task
                    tasks[i] = Task.Run(() => (provider as TMProviderBase).DoTMOperation(data));
                    threadsRanOK[i] = true;
                    i++;
                }
                catch (Exception e)
                {
                    threadsRanOK[i] = false;
                }
            }

            Task.WaitAll(tasks);
            // waiting would not be necessary IF the task had a return value AND we used the results here because: "If the Result property is accessed before the computation finishes, 
            // the property blocks the calling thread until the value is available."
        }

        private LookupData collectDataForUpdateAdd(Operations op, CultureInfo tradosSourceLang, CultureInfo tradosTargetLang, SdlTM.TranslationUnit[] tus, bool[] mask)
        {
            string s, t;
            s = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosSourceLang.Name);
            t = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(tradosTargetLang.Name);
            // if memoQ doesn't support these languages
            if (s == null || t == null) return null;

            // collect data for lookup - only unmasked segments
            List<TMHit> hits = new List<TMHit>();
            List<QuerySegment> qss = new List<QuerySegment>();
            for (int i = 0; i < tus.Length; i++)
            {
                if (mask == null || mask[i])
                {
                    // we need data for update: the tm, the key and the modified date
                    // disable context for now !!!
                    TMProvider.TranslationUnit tmtu = DataConverters.TradosTUToMemoQTU(tus[i], null, null, s, t);
                    //    i > 0 ? tus[i - 1].SourceSegment : null,
                    //    i < tus.Length - 1 ? tus[i + 1].SourceSegment : null);
                    Guid tmguid;
                    if(tus[i].FieldValues.Exists(AppData.TMGuidFieldName)) tmguid = new Guid(tus[i].FieldValues[AppData.TMGuidFieldName].GetValueString().Trim('"'));
                    else tmguid = new Guid();

                    hits.Add(new TMHit(tmtu, 0, tmguid, ""));
                    qss.Add(DataConverters.TradosTUToQuerySegment(tus[i], null, null));
                }
            }
            LookupData data = new LookupData(op, qss, hits, s, t, null, null, false,
                    !generalTMSettings.StrictSublanguageMatching, 100);
            return data;
        }

        private SdlTM.ImportResult[] createResultForUpdateAdd(Sdl.LanguagePlatform.TranslationMemory.Action action, SdlTM.TranslationUnit[] tus, bool[] mask)
        {
            SdlTM.ImportResult[] importRes = new SdlTM.ImportResult[tus.Length];
            for (int i = 0; i < tus.Length; i++)
            {
                if(tus[i] == null) importRes[i] = new SdlTM.ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.InvalidOperation);
                else importRes[i] = new SdlTM.ImportResult(action, ErrorCode.OK);
            }

            return importRes;
        }

        private void adjustTags(Segment originalSegment, ref SdlTM.SearchResult sr)
        {

            Segment s = sr.MemoryTranslationUnit.SourceSegment;
            Segment t = sr.MemoryTranslationUnit.TargetSegment;
            TagAdjuster.AdjustTagsFromOriginalSource(originalSegment, ref s, ref t);
            s = sr.TranslationProposal.SourceSegment;
            t = sr.TranslationProposal.TargetSegment;
            TagAdjuster.AdjustTagsFromOriginalSource(originalSegment, ref s, ref t);

        }

        /// <summary>
        /// Checks if the providers are OK, and returns the names of those that aren't, with the names of the TMs that had errors. Returns empty string if everything is OK.
        /// </summary>
        /// <param name="providersOK"></param>
        /// <returns>Empty string if everything is OK.</returns>
        private string checkProvidersForErrors(bool[] providersOK)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < providersOK.Length; i++)
            {
                TMProviderBase p = memoQTMProviders[i] as TMProviderBase;
                if (p.HasError || (p.TMErrors != null && p.TMErrors.Count > 0) || !providersOK[i])
                {
                    sb.Append(this.memoQTMProviders[i].ProviderName);
                    if (p.TMErrors != null && p.TMErrors.Count > 0)
                    {
                        sb.Append(": ");
                        foreach (string tm in p.TMErrors) sb.Append(tm + ", ");
                    }
                }
            }
            if (sb.ToString().EndsWith(", "))
            {
                sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
        }

        public bool HasExceptions()
        {
            if (exceptionList != null && exceptionList.Count > 0) return true;
            if (this.memoQTMProviders.Any(p => p.ExceptionList.Count > 0)) return true;
            return false;
        }

        /// <summary>
        /// Get one error message for all the providers and all the errors.
        /// </summary>
        public string GetExceptionsMessage(bool clearExceptions)
        {
            StringBuilder sb = new StringBuilder();
            string m = ExceptionHelper.GetAllExceptionsMessage(this.exceptionList, "");
            if (!String.IsNullOrEmpty(m)) sb.Append(m + "\n");
            if (clearExceptions) this.exceptionList.Clear();
            foreach (TMProviderBase provider in this.memoQTMProviders)
            {
                m = ExceptionHelper.GetAllExceptionsMessage(provider.ExceptionList, provider.ProviderName);
                if (!String.IsNullOrEmpty(m)) sb.Append(m + "\n");
                if (clearExceptions) provider.ClearExceptions();
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public void WriteExceptionsToLog(bool clearExceptions)
        {

            foreach (TMProviderBase provider in this.memoQTMProviders)
            {
                ExceptionHelper.WriteExceptionsToLog(this.exceptionList);
                if (provider.ExceptionList.Count > 0) ExceptionHelper.WriteExceptionsToLog(provider.ExceptionList, provider.ProviderName);
                if (clearExceptions) provider.ClearExceptions();
            }
        }

        #endregion


        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new MyTranslationProviderLanguageDirection(this, languageDirection.SourceCulture, languageDirection.TargetCulture, this.generalTMSettings.CanReverseLangDir);
        }

        public bool IsReadOnly
        {
            // readonly if all the used TMs are only for lookup
            // this is not a good idea, as Trados Studio displays a warning, and all the errors are hidden, and there's no info why it's read-only
            get
            {
                //if (this.memoQTMProviders == null || this.memoQTMProviders.Count == 0) return true;
                //bool canWrite;
                //foreach (TMProviderBase provider in this.memoQTMProviders)
                //{
                //    canWrite = provider.GetUsedTMs().Any(tm => tm.Purpose == TMPurposes.Update || tm.Purpose == TMPurposes.LookupUpdate);
                //    if (canWrite) return false;
                //}
                //return true;
                return false;
            }
        }

        public void LoadState(string translationProviderState)
        {
            int a = 0;
        }

        public string Name
        {
            get { return PluginResources.Plugin_Name; }
        }

        public void RefreshStatusInfo()
        {
        }

        public string SerializeState()
        {
            return "Serialized state";
        }

        public ProviderStatusInfo StatusInfo
        {
            // avarga del !!! 
            get { return new ProviderStatusInfo(true, ""); }
        }

        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        public bool SupportsFilters
        {
            get { return false; }
        }

        public bool SupportsFuzzySearch
        {
            get { return true; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            if (this.memoQTMProviders == null || this.memoQTMProviders.Count == 0) return false;

            // supports the language direction if there is a TM in all the TMs that has the same languages
            string s, t;
            s = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(languageDirection.SourceCulture.Name);
            t = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(languageDirection.TargetCulture.Name);
            // if memoQ doesn't support one of the languages
            if (s == null || t == null) return false;

            foreach (TMProviderBase provider in this.memoQTMProviders)
            {
                bool supports = provider.SupportsLangDir(s, t, true, false);
                if (supports) return true;
            }
            return false;
        }

        public bool SupportsMultipleResults
        {
            get { return true; }
        }

        public bool SupportsPenalties
        {
            get { return true; }
        }

        public bool SupportsPlaceables
        {
            get { return false; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsStructureContext
        {
            get { return false; }
        }

        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsTranslation
        {
            get { return true; }
        }

        public bool SupportsUpdate
        {
            get { return true; }
        }

        public bool SupportsWordCounts
        {
            get { return true; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TranslationMethod.TranslationMemory; }
        }

        public Uri Uri
        {
            get
            {
                List<MemoQTMSettings> settings = this.memoQTMProviders.Select(tm => tm.Settings).ToList();
                Uri wholeURI = SettingsURICreator.CreateURIFromSettings(this.generalTMSettings, settings);
                return wholeURI;
            }
        }

        #endregion
    }
}

