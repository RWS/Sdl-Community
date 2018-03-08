using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
    public class StemmingRuleSet
    {
        /// <summary>
        /// don't apply substitutions for words shorter than this:
        /// </summary>
        private const int DefaultMinimumWordLength = 3;
        /// <summary>
        /// don't apply a substitution when the stem becomes shorter
        /// than this:
        /// </summary>
        private const int DefaultMinimumStemLength = 2;
        /// <summary>
        /// don't appliy a substitution when the stem becomes shorter
        /// than this percent of the original word's length:
        /// </summary>
        private const int DefaultMinimumStemPercentage = 30;
        /// <summary>
        /// a _Rule is not applied when the stem becomes shorter than
        /// any of the both parameters
        /// </summary>

        /// <summary>
        /// how many rules may be applied? - avoid recursion
        /// </summary>
        private const int DefaultMaximumRuleApplications = 100;

        private List<StemmingRule> _Rules;
        private System.Globalization.CultureInfo _Culture;
        private int _MinimumWordLength = DefaultMinimumWordLength;
        private int _MinimumStemLength = DefaultMinimumStemLength;
        private int _MinimumStemPercentage = DefaultMinimumStemPercentage;
        private int _MaximumRuleApplications = DefaultMaximumRuleApplications;

        // TODO parameterless c-tor for XML serialization

        public StemmingRuleSet(System.Globalization.CultureInfo culture)
        {
            _Rules = new List<StemmingRule>();
            _Culture = culture;
        }

        // TODO enumerator?

        public StemmingRule this[int index]
        {
            get { return _Rules[index]; }
        }

        public void Add(StemmingRule r)
        {
            _Rules.Add(r);
            Sort();
        }

        public int Count
        {
            get { return _Rules.Count; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Globalization.CultureInfo Culture
        {
            // TODO setter for culture name for XML deserialization
            get { return _Culture; }
        }

        public int MinimumStemLength
        {
            get { return _MinimumStemLength; }
            set { _MinimumStemLength = value; }
        }

        public int MaximumRuleApplications
        {
            get { return _MaximumRuleApplications; }
            set { _MaximumRuleApplications = value; }
        }

        public int MinimumStemPercentage
        {
            get { return _MinimumStemPercentage; }
            set { _MinimumStemPercentage = value; }
        }

        public int MinimumWordLength
        {
            get { return _MinimumWordLength; }
            set { _MinimumWordLength = value; }
        }

        private void Sort()
        {
            _Rules.Sort(StemmingRule.Compare);
        }


    }
}
