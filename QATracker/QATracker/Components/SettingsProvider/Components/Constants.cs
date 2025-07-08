using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QATracker.Components.SettingsProvider.Components
{
    public class Constants
    {
        // Verifiers
        public const string SettingsTagVerifier = "SettingsTagVerifier";
        public const string SettingsTermVerifier = "SettingsTermVerifier";
        public const string QaVerificationSettings = "QAVerificationSettings";
        public const string NumberVerifierSettings = "NumberVerifierSettings";

        //Categories

        //QaChecker categories

        public const string SegmentsVerification = "Segments Verification";
        public const string SegmentsToExclude = "Segments to Exclude";
        public const string Inconsistencies = "Inconsistencies";
        public const string Punctuation = "Punctuation";
        public const string Numbers = "Numbers";
        public const string WordList = "Word List";
        public const string RegularExpressions = "Regular Expressions";
        public const string TrademarkCheck = "Trademark Check";
        public const string LengthVerification = "Length Verification";


        //TagVerifier categories

        public const string Common = "Common";

        //TermVerifier categories
        public const string VerificationSettings = "Verification Settings";

    }
}
