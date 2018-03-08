using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
	/// <summary>
	/// Represents a stemming rule.
	/// </summary>
    public class StemmingRule
    {
		/// <summary>
		/// The action to be applied by the rule.
		/// </summary>
        public enum StemAction
        {
            None = 0,
            Prefix = 1,			// GE-gangen
            Suffix = 2,			// schön-ER
            Infix = 3,			// ab-GE-törnt
            ProperInfix = 4,		// infix not at word bondaries
            Circumfix = 5,		// GE-brauch-T
            Form = 6,			// matches full form
            MapToLower = 7,
            StripDiacritics = 8,
            DeleteLastDoubleConsonants = 9,
            DeleteLastDoubleVowels = 10,
            TestOnBaseWord = 11,
            PrefixedInfix

        }

        // the continuation determines which next _Rule is tried next after
        // a _Rule was applied successfully (i.e. when a replacement was
        // made). The interpretation engine is not part of this class,
        // however, a _Rule should now whether it is "final" or not. When a
        // _Rule could not be applied, the default behaviour is th
        // "Continue".
        public enum StemContinuation
        {
            Continue = 0,		// apply next _Rule
            Restart = 1,		// start again with first _Rule
            Stop = 2			// stop applying rules
        }

        private StemAction _Action = StemAction.None;
        private StemContinuation _ContinuationOnSuccess;
        private StemContinuation _ContinuationOnFail;
        private int _Length;
        private string _Affix;
        private string _Replacement;
        private int _Priority;
        private int _ContinuationPriority;

        public static int Compare(StemmingRule a, StemmingRule b)
        {
            // primary: decreasing by prio
            int result = b._Priority - a._Priority;
            if (result == 0)
            {
                // secondary: decreasing by length
                result = b._Length - a._Length;
            }
            return result;
        }

        public StemmingRule()
        {
            _Action = StemAction.None;
        }

        public StemContinuation ContinuationOnSuccess
        {
            get { return _ContinuationOnSuccess; }
            set { _ContinuationOnSuccess = value; }
        }

        public StemAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        public StemContinuation ContinuationOnFail
        {
            get { return _ContinuationOnFail; }
            set { _ContinuationOnFail = value; }
        }

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public string Affix
        {
            get { return _Affix; }
            set { _Affix = value; }
        }

        public string Replacement
        {
            get { return _Replacement; }
            set { _Replacement = value; }
        }

        public int Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }

        public int ContinuationPriority
        {
            get { return _ContinuationPriority; }
            set { _ContinuationPriority = value; }
        }


    }
}
