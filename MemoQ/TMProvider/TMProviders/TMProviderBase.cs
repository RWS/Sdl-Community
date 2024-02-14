using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Concurrent;
using System.Threading;

namespace TMProvider
{
    public enum Operations { None, Lookup, Update, Add, Concordance }

    public class LookupData
    {
        public Operations Operation { get; private set; }
        /// <summary>
        /// null for update
        /// </summary>
        public List<QuerySegment> Segments { get; private set; }
        /// <summary>
        /// null for lookup
        /// </summary>
        public List<TMHit> UpdateTUs { get; private set; }
        public string SourceLangCode { get; private set; }
        public string TargetLangCode { get; private set; }
        public LookupSegmentRequest LookupSettings { get; private set; }
        public ConcordanceRequest ConcordanceSettings { get; private set; }
        public bool CanReverseLangs { get; private set; }
        public bool AllowSubLangs { get; private set; }
        public int MatchThreshold { get; private set; }

        public LookupData(Operations op, List<QuerySegment> segments, List<TMHit> updateTUs, string sourceLangCode, string targetLangCode,
            LookupSegmentRequest lookupSettings, ConcordanceRequest concSettings, bool canRevLang, bool allowSubLang, int matchThreshold)
        {
            this.Operation = op;
            this.Segments = segments;
            this.SourceLangCode = sourceLangCode;
            this.TargetLangCode = targetLangCode;
            this.LookupSettings = lookupSettings;
            this.ConcordanceSettings = concSettings;
            this.CanReverseLangs = canRevLang;
            this.AllowSubLangs = allowSubLang;
            this.MatchThreshold = matchThreshold;
            this.UpdateTUs = updateTUs;
        }
    }

    public abstract class TMProviderBase
    {
        #region Properties

        protected MemoQTMProviderTypes providerType;
        /// <summary>
        /// The type of memoQ provider.
        /// </summary>
        public MemoQTMProviderTypes ProviderType { get { return providerType; } }

        protected string providerName;
        public string ProviderName { get { return providerName; } }

        protected MemoQTMSettings settings;
        /// <summary>
        /// All the data needed for the lookup (URL, username, password, chosen TMs)
        /// </summary>
        public MemoQTMSettings Settings { get { return settings; } }

        protected bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { isLoggedIn = value; }
        }

        protected bool isActive = true;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// Don't add or remove items directly. Use the functions to keep this list synchronized with the settings.
        /// </summary>
        protected List<TMInfo> usedTMs = new List<TMInfo>();
        public TMInfo[] GetUsedTMs()
        {
            return usedTMs.ToArray();
        }

        #endregion

        #region Abstract methods

        public abstract List<Exception> ExceptionList { get; }
        public abstract void ClearExceptions();
        public abstract void Login(string username, string pwd, LoginTypes loginType);

        #endregion

        #region Results

        protected volatile List<TMHit>[] lookupResults;
        /// <summary>
        /// The results are only accessed by the main thread when all the other threads are finished.
        /// This is the reason for not locking.
        /// </summary>
        public List<TMHit>[] LookupResults
        {
            get
            {
                return lookupResults;
            }
        }

        protected volatile List<ConcordanceResult> concordanceResults;
        /// <summary>
        /// The results are only accessed by the main thread when all the other threads are finished.
        /// This is the reason for not locking.
        /// </summary>
        public List<ConcordanceResult> ConcordanceResults
        {
            get
            {
                return concordanceResults;
            }
        }

        protected volatile bool hasError = false;
        public bool HasError
        {
            get
            {
                return hasError;
            }
        }

        protected ConcurrentDictionary<string, byte> tmErrors = new ConcurrentDictionary<string, byte>();
        public List<string> TMErrors
        {
            get { return tmErrors.Keys.ToList(); }
        }

        #endregion

        #region Thread operations

        public void DoTMOperation(object lookupData)
        {
            try
            {
                LookupData data = (LookupData)lookupData;
                int segCount = 0;
                if (data.Segments != null) segCount = data.Segments.Count;
                else if (data.UpdateTUs != null) segCount = data.UpdateTUs.Count;
                hasError = false;
                lookupResults = null;
                tmErrors.Clear();


                if (!(lookupData is LookupData)) throw new ArgumentException("Invalid lookup data.");

                if (data.Operation == Operations.Lookup) lookupResults = onLookup(data);
                else if (data.Operation == Operations.Update) onUpdate(data);
                else if (data.Operation == Operations.Add) onAdd(data);
                else if (data.Operation == Operations.Concordance) concordanceResults = onConcordance(data);
                else throw new InvalidOperationException("Invalid operation");

            }

            catch (Exception e)
            {
                lookupResults = null;
                hasError = true;
                collectException(e);
            }
        }

        abstract protected void RetryLogin(string user, string password, LoginTypes loginType);

        /// <summary>
        /// Derived classes can override this to perform the background processing.
        /// </summary>
        abstract protected List<TMHit>[] onLookup(LookupData data);

        /// <summary>
        /// Derived classes can override this to perform the background processing.
        /// </summary>
        abstract protected void onUpdate(LookupData data);

        /// <summary>
        /// Derived classes can override this to perform the background processing.
        /// </summary>
        abstract protected void onAdd(LookupData data);

        /// <summary>
        /// Derived classes can override this to perform the background processing.
        /// </summary>
        abstract protected List<ConcordanceResult> onConcordance(LookupData data);

        #endregion

        #region TM operations

        /// <summary>
        /// Call this when creating a class from settings to fill up the TMInfos from the server based on the TM guids.
        /// </summary>
        protected void fillUsedTMsFromSettings()
        {
            if (this.settings == null) throw new Exception("Assign settings first!");
            // used TMs: only guids are stored but we need full TM info
            // get TM info for used TMs from the server
            TMInfo[] allTMs = new TMInfo[0];
            this.usedTMs = new List<TMInfo>();
            try
            {
                allTMs = ListTMs();
            }
            catch (Exception e)
            {
                collectException(e);
            }

            if (allTMs == null || allTMs.Length == 0) return;

            foreach (TMInfo info in allTMs)
            {
                int i = 0;
                foreach (Guid usedTM in this.Settings.UsedTMs)
                {
                    if (info.TMGuid == usedTM)
                    {
                        info.Purpose = this.settings.TMSDLRights[i];
                        this.usedTMs.Add(info);
                        break;
                    }
                    i++;
                }
            }
        }

        public void AddUsedTM(TMInfo tmInfo)
        {
            usedTMs.Add(tmInfo);
            settings.AddUsedTM(tmInfo.TMGuid, tmInfo.Purpose);
        }

        public void RemoveUsedTM(Guid tmGuid)
        {
            int ix = usedTMs.FindIndex(info => info.TMGuid == tmGuid);
            if (ix != -1) usedTMs.RemoveAt(ix);
            settings.RemoveUsedTM(tmGuid);
        }

        public void RemoveUsedTM(TMInfo tmInfo)
        {
            usedTMs.Remove(tmInfo);
            settings.RemoveUsedTM(tmInfo.TMGuid);
        }

        public void ClearUsedTMs()
        {
            usedTMs.Clear();
            settings.ClearUsedTMs();
        }

        public string GetTMName(Guid guid)
        {
            TMInfo tm = usedTMs.Find(item => item.TMGuid == guid);
            if (tm == null) return "";
            else return tm.FriendlyName;
        }

        public abstract TMInfo[] ListTMs();

        public TMInfo[] ListTMs(bool strictSublang, List<string> languages)
        {
            if (!isActive) return new TMInfo[0];

            TMInfo[] allTMs = ListTMs();

            if (allTMs == null || allTMs.Length == 0) return allTMs;

            List<TMInfo> rightTMs = new List<TMInfo>();
            foreach (TMInfo info in allTMs)
            {
                if (strictSublang && languages.Contains(info.SourceLanguageCode) && languages.Contains(info.TargetLanguageCode))
                {
                    rightTMs.Add(info);
                }
                else if (!strictSublang)
                {
                    bool match = false;
                    string l;
                    foreach (string source in languages)
                    {
                        if (source.Length >= 3) l = source.Substring(0, 3);
                        else continue;
                        if (info.SourceLanguageCode.StartsWith(l))
                        {
                            match = true;
                            break;
                        }
                    }
                    foreach (string target in languages)
                    {
                        if (target.Length >= 3) l = target.Substring(0, 3);
                        else continue;
                        if (match && info.TargetLanguageCode.StartsWith(l))
                        {
                            rightTMs.Add(info);
                            break;
                        }
                    }
                }
            }
            return rightTMs.ToArray();
        }

        public TMInfo[] GetLookupTMs()
        {
            return usedTMs.FindAll(tm => tm.Purpose == TMPurposes.Lookup || tm.Purpose == TMPurposes.LookupUpdate).ToArray();
        }

        public TMInfo[] GetUpdateTMs()
        {
            return usedTMs.FindAll(tm => tm.Purpose == TMPurposes.Update || tm.Purpose == TMPurposes.LookupUpdate).ToArray();
        }

        /// <summary>
        /// Gets an array of the TMs that can be used for lookup for the language pair (memoQ codes!). Use full 3+2 letter language codes.
        /// </summary>
        /// <returns></returns>
        public TMInfo[] GetLookupTMs(string sourceLangCode, string targetLangCode, bool canReverse, bool strictSublang)
        {
            string s = sourceLangCode, t = targetLangCode;
            if (!strictSublang)
            {
                if (sourceLangCode.Length >= 3) s = sourceLangCode.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");
                if (targetLangCode.Length >= 3) t = targetLangCode.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");

                if (canReverse)
                {
                    return usedTMs.FindAll(tm => (tm.Purpose == TMPurposes.Lookup || tm.Purpose == TMPurposes.LookupUpdate) &&
                        ((tm.SourceLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)) ||
                        (tm.SourceLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)))).ToArray();
                }
                else
                {
                    return usedTMs.FindAll(tm => (tm.Purpose == TMPurposes.Lookup || tm.Purpose == TMPurposes.LookupUpdate) &&
                        tm.SourceLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                }
            }
            else
            {
                // even if there's strict sublang matching, whatever is a general lang in memoQ will match the language from Trados
                // because in Trados there are only sublanguages and no general languages like "eng"
                List<TMInfo> tms = new List<TMInfo>();
                foreach (TMInfo tm in usedTMs)
                {
                    // purpose not right
                    if (tm.Purpose != TMPurposes.Lookup && tm.Purpose != TMPurposes.LookupUpdate) continue;
                    // match memoq general language to trados sublanguage
                    if (tm.SourceLanguageCode.Length == 3) s = sourceLangCode.Substring(0, 3);
                    if (tm.TargetLanguageCode.Length == 3) t = targetLangCode.Substring(0, 3);

                    // equal languages
                    if (tm.SourceLanguageCode.Equals(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.Equals(t, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tms.Add(tm);
                    }
                    // equal reversed
                    else if (canReverse && tm.SourceLanguageCode.Equals(t, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tms.Add(tm);
                    }
                }
                return tms.ToArray();
            }
        }


        /// <summary>
        /// Gets an array of the TMs that can be used for update (and delete) for the language pair (memoQ codes!). Use full 3+2 letter language codes.
        /// </summary>
        /// <returns></returns>
        public TMInfo[] GetUpdateTMs(string sourceLangCode, string targetLangCode, bool strictSublang)
        {
            string s = sourceLangCode, t = targetLangCode;
            if (!strictSublang)
            {
                if (sourceLangCode.Length >= 3) s = sourceLangCode.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");
                if (targetLangCode.Length >= 3) t = targetLangCode.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");

                return usedTMs.FindAll(tm => (tm.Purpose == TMPurposes.Update || tm.Purpose == TMPurposes.LookupUpdate) &&
                        tm.SourceLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            }
            else
            {
                // even if there's strict sublang matching, whatever is a general lang in memoQ will match the language from Trados
                // because in Trados there are only sublanguages and no general languages like "eng"
                List<TMInfo> tms = new List<TMInfo>();
                foreach (TMInfo tm in usedTMs)
                {
                    // purpose not right
                    if (tm.Purpose != TMPurposes.Update && tm.Purpose != TMPurposes.LookupUpdate) continue;
                    // match memoq general language to trados sublanguage
                    if (tm.SourceLanguageCode.Length == 3) s = sourceLangCode.Substring(0, 3);
                    if (tm.TargetLanguageCode.Length == 3) t = targetLangCode.Substring(0, 3);

                    // equal languages
                    if (tm.SourceLanguageCode.Equals(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.Equals(t, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tms.Add(tm);
                    }

                }
                return tms.ToArray();

            }
        }

        public bool SupportsLangDir(string source, string target, bool canReverse, bool strictSublang)
        {
            string s = source, t = target;
            bool supports;
            if (!strictSublang)
            {
                if (source.Length >= 3) s = source.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");
                if (target.Length >= 3) t = target.Substring(0, 3);
                else throw new InvalidOperationException("The source language code is not valid.");

                if (canReverse)
                {
                    supports = ListTMs().Any(tm => (tm.SourceLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) && tm.TargetLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)) ||
                        (tm.SourceLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase) && tm.TargetLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)));
                }
                else
                {
                    supports = ListTMs().Any(tm => tm.SourceLanguageCode.StartsWith(s, StringComparison.InvariantCultureIgnoreCase) && tm.TargetLanguageCode.StartsWith(t, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            else
            {
                supports = false;
                foreach (TMInfo tm in usedTMs)
                {
                    // match memoq general language to trados sublanguage
                    if (tm.SourceLanguageCode.Length == 3) s = source.Substring(0, 3);
                    if (tm.TargetLanguageCode.Length == 3) t = target.Substring(0, 3);

                    // equal languages
                    if (tm.SourceLanguageCode.Equals(s, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.Equals(t, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                    // equal reversed
                    else if (canReverse && tm.SourceLanguageCode.Equals(t, StringComparison.InvariantCultureIgnoreCase) &&
                        tm.TargetLanguageCode.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return supports;
        }

        public void SetTMPurpose(Guid tmGuid, TMPurposes purpose)
        {
            int ix = usedTMs.FindIndex(info => info.TMGuid == tmGuid);
            if (ix != -1)
            {
                usedTMs[ix].Purpose = purpose;
                settings.SetTMPurpose(tmGuid, purpose);
            }
        }


        #endregion

        #region ICloneable

        public abstract TMProviderBase Clone();

        #endregion

        #region Exceptions

        protected abstract bool collectException(Exception e, string tmGuid = "", string tmName = "");

        #endregion // Exceptions
    }


}
