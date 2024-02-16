using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;

using System.Windows.Forms;

using System.Globalization;
using Sdl.Core.Globalization;

namespace TradosPlugin
{
    public class MyTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
    {
        #region ITranslationProviderLanguageDirection Members

        private CultureInfo sourceLanguage;
        private CultureInfo targetLanguage;
        private bool canReverseLangDir;
        private Dictionary<PersistentObjectToken, TMProvider.UpdateData> currentLookupResults = new Dictionary<PersistentObjectToken, TMProvider.UpdateData>();

        public MyTranslationProviderLanguageDirection(ITranslationProvider memoQTranslationProvider, CultureInfo sourceLang, CultureInfo targetLang, bool canReverseLangDir)
        {
            this.translationProvider = memoQTranslationProvider;
            this.sourceLanguage = sourceLang;
            this.targetLanguage = targetLang;
            this.canReverseLangDir = canReverseLangDir;
        }

        
        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncAdd(translationUnits, this.sourceLanguage, this.targetLanguage, null);
            string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
            if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            return allRes;
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncAdd(translationUnits, this.sourceLanguage, this.targetLanguage, mask);
            string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);

            // if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            TMProvider.Log.WriteToLog(m);

            return allRes;
        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncAdd(new TranslationUnit[] { translationUnit }, this.sourceLanguage, this.targetLanguage, null);

            string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
            //if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            TMProvider.Log.WriteToLog(m);

            if (allRes != null && allRes.Length > 1) return allRes[0];
            else return null;

        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncAdd(translationUnits, this.sourceLanguage, this.targetLanguage, null);

            string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
            //if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            TMProvider.Log.WriteToLog(m);

            return allRes;

        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncAdd(translationUnits, this.sourceLanguage, this.targetLanguage, mask);

            string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
            //if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            TMProvider.Log.WriteToLog(m);

            return allRes;
        }

        public bool CanReverseLanguageDirection
        {
            get { return canReverseLangDir; }
        }

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, new Segment[] { segment },
                null, this.sourceLanguage, this.targetLanguage, true);

            handleExceptions(res);
            saveLookupTUs(res);
            if (res != null && res.Length > 0) return res[0];

            else return null;
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, segments,
                null, this.sourceLanguage, this.targetLanguage, true);

            saveLookupTUs(res);

            handleExceptions(res);
            return res;
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, segments,
                mask, this.sourceLanguage, this.targetLanguage, true);

            saveLookupTUs(res);

            handleExceptions(res);

            return res;
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            SearchResults res = (this.translationProvider as MemoQTranslationProvider).AsyncConcordance(settings, segment, this.sourceLanguage, this.targetLanguage);

            handleExceptions(res);

            return res;
        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, new TranslationUnit[] { translationUnit },
                null, this.sourceLanguage, this.targetLanguage, true);

            saveLookupTUs(res);

            handleExceptions(res);

            if (res == null || res.Length == 0) return null;
            else return res[0];
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            // return lookupTranslationUnits(settings, translationUnits, null);
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, translationUnits,
                null, this.sourceLanguage, this.targetLanguage, true);

            saveLookupTUs(res);

            handleExceptions(res);

            return res;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            //return lookupTranslationUnits(settings, translationUnits, mask);
            SearchResults[] res = (this.translationProvider as MemoQTranslationProvider).AsyncLookup(settings, translationUnits,
               mask, this.sourceLanguage, this.targetLanguage, true);

            saveLookupTUs(res);

            handleExceptions(res);

            return res;
        }

        public CultureCode SourceLanguage
        {
            get { return sourceLanguage; }
        }

        public CultureCode TargetLanguage
        {
            get { return targetLanguage; }
        }

        private ITranslationProvider translationProvider;
        public ITranslationProvider TranslationProvider
        {
            get { return translationProvider; }
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            //throw new Exception("Update is unfortunately not allowed in this version of the memoQ TM plugin.");
            //ImportResult r = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.InvalidOperation);
            //return r;

            TranslationUnit tu = addUpdateData(translationUnit);

            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncUpdate(new TranslationUnit[] { translationUnit }, this.sourceLanguage, this.targetLanguage, null);
            //string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage();
            // if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            (this.translationProvider as MemoQTranslationProvider).WriteExceptionsToLog(true);

            if (allRes.Length > 0) return allRes[0];
            else return null;
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            //throw new Exception("Update is unfortunately not allowed in this version of the memoQ TM plugin.");
            //ImportResult r = new ImportResult(Sdl.LanguagePlatform.TranslationMemory.Action.Error, ErrorCode.InvalidOperation);
            //ImportResult[] rs = new ImportResult[translationUnits.Length];
            //for (int i = 0; i < rs.Length; i++) rs[i] = r;
            //return rs;

            // prepare data for update
            TranslationUnit[] updateUnits = new TranslationUnit[translationUnits.Length];
            for (int i = 0; i < translationUnits.Length; i++)
            {
                updateUnits[i] = addUpdateData(translationUnits[i]);
            }

            ImportResult[] allRes = (this.translationProvider as MemoQTranslationProvider).AsyncUpdate(updateUnits, this.sourceLanguage, this.targetLanguage, null);
            //string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage();
            // if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            (this.translationProvider as MemoQTranslationProvider).WriteExceptionsToLog(true);

            return allRes;
        }

        #endregion

        #region helpers

        private void handleExceptions(SearchResults[] res)
        {
            //TMProvider.Log.WriteToLog(m);
            (this.translationProvider as MemoQTranslationProvider).WriteExceptionsToLog(false);

            // if there are no results
            if (res == null || res.Length == 0 || res.All(r => r == null || r.Results == null || r.Results.Count == 0))
            {
                string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
                if (!String.IsNullOrEmpty(m)) throw new Exception(m); // ExceptionHelper.ShowMessage(m);
            }
        }

        private void handleExceptions(SearchResults res)
        {
            //TMProvider.Log.WriteToLog(m);
            (this.translationProvider as MemoQTranslationProvider).WriteExceptionsToLog(false);

            if (res == null || res.Results == null || res.Results.Count == 0)
            {
                string m = (this.translationProvider as MemoQTranslationProvider).GetExceptionsMessage(true);
                if (!String.IsNullOrEmpty(m)) throw new Exception(m); // ExceptionHelper.ShowMessage(m);
            }
        }

        private void saveLookupTUs(SearchResults[] res)
        {
            currentLookupResults.Clear();
            if (res == null || res.Length == 0) return;

            foreach (SearchResults srOneSegm in res)
            {
                if (srOneSegm == null || srOneSegm.Results == null || srOneSegm.Results.Count == 0) continue;
                foreach (SearchResult sr in srOneSegm.Results)
                {
                    if (currentLookupResults.ContainsKey(sr.TranslationProposal.ResourceId)) continue;
                    // no key or TM
                    if (!sr.TranslationProposal.FieldValues.Exists(TMProvider.AppData.KeyFieldName)) continue;
                    if (!sr.TranslationProposal.FieldValues.Exists(TMProvider.AppData.TMGuidFieldName)) continue;
                    
                    int entryID = Int32.Parse(sr.TranslationProposal.FieldValues[TMProvider.AppData.KeyFieldName].GetValueString());
                    string s = sr.TranslationProposal.FieldValues[TMProvider.AppData.TMGuidFieldName].GetValueString().Trim('"');
                    Guid tmguid = new Guid(s);
                    currentLookupResults.Add(sr.TranslationProposal.ResourceId, new TMProvider.UpdateData(entryID, tmguid, sr.TranslationProposal.SystemFields.ChangeDate));
                    // these custom fields are not needed any more -> remove them as they take up a lot of space in each row
                    sr.TranslationProposal.FieldValues.Remove(TMProvider.AppData.TMGuidFieldName);
                    sr.TranslationProposal.FieldValues.Remove(TMProvider.AppData.KeyFieldName);
                }
            }
        }

        private TranslationUnit addUpdateData(TranslationUnit tu)
        {
            // this is a bug in Trados: the fieldvalues are null when we get the tu, and then the constructor complains
            tu.FieldValues = new FieldValues();
            TranslationUnit newTU = tu.Duplicate();
            if (currentLookupResults.ContainsKey(tu.ResourceId))
            {
                TMProvider.UpdateData data = currentLookupResults[tu.ResourceId];
                newTU.FieldValues = new FieldValues(tu.FieldValues);
                newTU.FieldValues.Add(new SingleStringFieldValue(TMProvider.AppData.TMGuidFieldName, data.TMGuid.ToString()));
                newTU.FieldValues.Add(new SingleStringFieldValue(TMProvider.AppData.KeyFieldName, data.EntryID.ToString()));
                newTU.SystemFields.ChangeDate = data.Modified;
            }
            return newTU;
        }

        #endregion
    }
}
