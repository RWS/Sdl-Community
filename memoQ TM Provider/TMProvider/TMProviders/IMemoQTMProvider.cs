using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TMProvider
{
    public interface IMemoQTMProvider
    {
        /// <summary>
        /// The type of memoQ provider.
        /// </summary>
        MemoQTMProviderTypes ProviderType { get; }

        string ProviderName { get; }

        /// <summary>
        /// All the data needed for the lookup (URL, username, password, chosen TMs)
        /// </summary>
        MemoQTMSettings Settings { get ;  }

        /// <summary>
        /// Whether the user is logged in.
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// True if user is logged in or can try login again. False if we've tried login a few times, and we'll not try any more.
        /// </summary>
        bool IsActive { get; }

        List<Exception> ExceptionList { get; }

        void ClearExceptions();

        /// <summary>
        /// Lists all the TMs on this provider.
        /// </summary>
        /// <returns></returns>
        TMInfo[] ListTMs();

        /// <summary>
        /// Lists the TMs where the source and target language are both in the listed languages.
        /// </summary>
        /// <param name="strictSublang">If true, only the main language is taken into consideration.</param>
        /// <param name="languages"></param>
        /// <returns></returns>
        TMInfo[] ListTMs(bool strictSublang, List<string> languages);

        /// <summary>
        /// Performs a lookup for a single segment in a TM.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="TM"></param>
        /// <param name="matchThreshold"></param>
        /// <returns></returns>
        TMHit[] LookupSegment(QuerySegment segment, LookupSegmentRequest options, Guid TM, int matchThreshold);

        /// <summary>
        /// Performs a batch lookup for the segments.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="TM"></param>
        /// <param name="matchThreshold"></param>
        /// <returns></returns>
        TMHit[][] LookupSegments(QuerySegment[] segment, LookupSegmentRequest options, Guid TM, int matchThreshold);

        ConcordanceResult ConcordanceLookup(string tmGuid, List<string> expressions, ConcordanceRequest options);
      
        void AddTMEntry(string tmGuid, TranslationUnit tmEntry);

        void AddTMEnties(string tmGuid, TranslationUnit[] tmEntries);

        void DeleteEntry(string tmGuid, int tuId);

        void UpdateEntry(string tmGuid, TranslationUnit tmEntry, int tuId);

        /// <summary>
        /// An array of all the TMs the user has chosen for any purpose.
        /// </summary>
        /// <returns></returns>
        TMInfo[] GetUsedTMs();

        /// <summary>
        /// Adds a TM to the list of used TMs in the provider and in the settings.
        /// </summary>
        /// <param name="tmInfo"></param>
        void AddUsedTM(TMInfo tmInfo);

        /// <summary>
        /// Removes the TM from the provider's TM list and from the settings.
        /// </summary>
        /// <param name="TMGuid"></param>
        void RemoveUsedTM(Guid TMGuid);

        /// <summary>
        /// Removes the TM from the provider's TM list and from the settings.
        /// </summary>
        /// <param name="TMGuid"></param>
        void RemoveUsedTM(TMInfo TM);

        /// <summary>
        /// Clears the list of used TMs.
        /// </summary>
        void ClearUsedTMs();

        /// <summary>
        /// Gets an array of the TMs that can be used for lookup.
        /// </summary>
        /// <returns></returns>
        TMInfo[] GetLookupTMs();

        /// <summary>
        /// Gets an array of the TMs that can be used for update (and delete). 
        /// </summary>
        /// <returns></returns>
        TMInfo[] GetUpdateTMs();

        /// <summary>
        /// Gets an array of the TMs that can be used for lookup for the language pair (memoQ codes!). Use full 3+2 letter language codes.
        /// </summary>
        /// <returns></returns>
        TMInfo[] GetLookupTMs(string sourceLangCode, string targetLangCode, bool canReverse, bool strictSublang);

        /// <summary>
        /// Gets an array of the TMs that can be used for update (and delete) for the language pair (memoQ codes!). Use full 3+2 letter language codes.
        /// </summary>
        /// <returns></returns>
        TMInfo[] GetUpdateTMs(string sourceLangCode, string targetLangCode, bool strictSublang);

        /// <summary>
        /// Sets the new rights/purpose for the TM.
        /// </summary>
        /// <param name="tmGuid"></param>
        /// <param name="purpose"></param>
        void SetTMPurpose(Guid tmGuid, TMPurposes purpose);

        /// <summary>
        /// Whether there is a TM that can be used for this language direction.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="canReverse"></param>
        /// <param name="strictSublang"></param>
        /// <returns></returns>
        bool SupportsLangDir(string source, string target, bool canReverse, bool strictSublang);

        IMemoQTMProvider Clone();

    }
}
