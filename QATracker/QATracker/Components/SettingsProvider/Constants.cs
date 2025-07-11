using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QATracker.Components.SettingsProvider
{
    public class Constants
    {
        // Verifiers
        public const string SettingsTagVerifier = "SettingsTagVerifier";
        public const string SettingsTermVerifier = "SettingsTermVerifier";
        public const string QaVerificationSettings = "QAVerificationSettings";
        public const string NumberVerifierSettings = "Number Verifier";

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

        //Internal verifier names
        public const string QaCheckerVerifierName = "QA Verification Settings";
        public const string TagVerifierName = "Tag Verifier";
        public const string TermVerifierName = "Term Verifier";


        //3rd party verifier names
        public const string NumberVerifierName = "Trados Number Verifier";


        //3rd party verifier categories

        //Trados Number Verifier
        public const string NumberVerifierSettingsCategory = "NumberVerifierSettings";
    }
}
