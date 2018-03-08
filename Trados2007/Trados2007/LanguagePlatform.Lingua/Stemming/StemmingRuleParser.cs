using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
    internal class StemmingRuleParser
    {
        private System.Text.StringBuilder _Keyword = new StringBuilder();
        private int _RuleP = 0;
        private string _Rule;
        private StemmingRuleSet _RuleSet;

        public StemmingRuleParser(StemmingRuleSet ruleset)
        {
            _RuleSet = ruleset;
        }

        /// <summary>
        /// Read a stemming _Rule from a string in the old TRADOS format. 
        /// </summary>
        /// <param name="rule">The textual _Rule</param>
        /// <returns>A new stemming _Rule, parsed from the input string</returns>
        public void Add(string rule)
        {
            _Rule = rule;
            _RuleP = 0;

            StemmingRule result = new StemmingRule();

            int state = 0;
            int len = _Rule.Length;

            while (state != 99)
            {
                while (_RuleP < len && System.Char.IsWhiteSpace(_Rule, _RuleP))
                    ++_RuleP;

                switch (state)
                {
                    case 0:
                        // waiting for initial keyword
                        switch (GetIdentifier().ToLowerInvariant())
                        {
                            case "replace":
                                state = 6;
                                break;
                            case "stripdiacritics":
                                state = 1;
                                result.Action = StemmingRule.StemAction.StripDiacritics;
                                break;
                            case "tolower":
                                result.Action = StemmingRule.StemAction.MapToLower;
                                state = 1;
                                break;
                            case "deletelastdoublevowels":
                                result.Action = StemmingRule.StemAction.DeleteLastDoubleVowels;
                                state = 1;
                                break;
                            case "deletelastdoubleconsonants":
                                result.Action = StemmingRule.StemAction.DeleteLastDoubleConsonants;
                                state = 1;
                                break;
                            case "testonbaseword":
                                result.Action = StemmingRule.StemAction.TestOnBaseWord;
                                state = 1;
                                break;
                            case "set":
                                // Variable Setting
                                state = 12;
                                break;
                            default:
								throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationIllegalKeywordInRule, _Rule);
                        }
                        break;

                    case 1:
                        // priority...
                        Expect("priority");
                        state = 2;
                        break;

                    case 2:
                        // priority number
                        result.Priority = GetNumber();
                        state = 3;
                        break;

                    case 3:
                        // "and"
                        Expect("and");
                        state = 4;
                        break;

                    case 4:
                        // "restart" - continuation
                        state = 10;
                        switch (GetIdentifier().ToLowerInvariant())
                        {
                            case "continue":
                                result.ContinuationOnSuccess = StemmingRule.StemContinuation.Continue;
                                break;
                            case "restart":
                                result.ContinuationOnSuccess = StemmingRule.StemContinuation.Restart;
                                break;
                            case "stop":
                                result.ContinuationOnSuccess = StemmingRule.StemContinuation.Stop;
                                state = 5; // no continuation priority for stop
                                break;
                            default:
								throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationIllegalContinuation, _Rule);
                        }

                        result.ContinuationOnFail = StemmingRule.StemContinuation.Continue;
                        result.ContinuationPriority = 0;
                        break;

                    case 5:
                        // final semicolon
                        if (_Rule[_RuleP] == ';')
                            state = 99; // success
                        else
							throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationTrailingJunk, _Rule);
                        break;

                    case 6:
                        // _Rule/affix type
                        switch (GetIdentifier().ToLowerInvariant())
                        {
                            case "prefix":
                                result.Action = StemmingRule.StemAction.Prefix;
                                break;
                            case "suffix":
                                result.Action = StemmingRule.StemAction.Suffix;
                                break;
                            case "infix":
                                result.Action = StemmingRule.StemAction.Infix;
                                break;
                            case "properinfix":
                                result.Action = StemmingRule.StemAction.ProperInfix;
                                break;
                            case "circumfix":
                                result.Action = StemmingRule.StemAction.Circumfix;
                                break;
                            case "form":
                                result.Action = StemmingRule.StemAction.Form;
                                break;
                            case "prefixedinfix":
                                result.Action = StemmingRule.StemAction.PrefixedInfix;
                                break;
                            default:
								throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationUnknownRuleType, _Rule);
                        }
                        state = 7;
                        break;

                    case 7:
                        // affix pattern
                        result.Affix = GetQuotedString();
                        state = 8;
                        break;

                    case 8:
                        // "with"
                        Expect("with");
                        state = 9;
                        break;

                    case 9:
                        // replacement pattern
                        result.Replacement = GetQuotedString();
                        state = 1;
                        break;

                    case 10:
                        // "at" <cprio>
                        if (_Rule[_RuleP] == ';')
                            state = 5;
                        else
                        {
                            Expect("at");
                            state = 11;
                        }
                        break;

                    case 11:
                        result.ContinuationPriority = GetNumber();
                        state = 5;
                        break;

                    case 12: // Variable Setting - got "set", expect variable name
                        {
                            string id = GetIdentifier().ToLowerInvariant();
                            switch (id)
                            {
                                case "minwordlength":
                                    _RuleSet.MinimumWordLength = GetNumber();
                                    break;
                                case "minstemlength":
                                    _RuleSet.MinimumStemLength = GetNumber();
                                    break;
                                case "minstempercentage":
                                    _RuleSet.MinimumStemPercentage = GetNumber();
                                    break;
                                case "maxruleapplications":
                                    _RuleSet.MaximumRuleApplications = GetNumber();
                                    break;
                                default:
									throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationInvalidVariableName, id);
                            }
                            state = 5;
                        }
                        break;
                }
            }

            if (result != null && result.Action != StemmingRule.StemAction.None)
                _RuleSet.Add(result);
        }

        private void Expect(string expectation)
        {
            string id = GetIdentifier();
			if (!id.Equals(expectation, StringComparison.OrdinalIgnoreCase))
				throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationInvalidRule);
        }

        private string GetIdentifier()
        {
            System.Text.StringBuilder identifier = new StringBuilder();
            while (_RuleP < _Rule.Length && System.Char.IsLetter(_Rule[_RuleP]))
            {
                identifier.Append(_Rule[_RuleP]);
                ++_RuleP;
            }
            return identifier.ToString();
        }

        private int GetNumber()
        {
            // skip whitespace
            while (_RuleP < _Rule.Length && System.Char.IsWhiteSpace(_Rule, _RuleP))
                ++_RuleP;

            System.Text.StringBuilder value = new StringBuilder();
            while (_RuleP < _Rule.Length && System.Char.IsDigit(_Rule[_RuleP]))
            {
                value.Append(_Rule[_RuleP]);
                ++_RuleP;
            }
            if (value.Length == 0)
				throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationInvalidRule);
            return Int32.Parse(value.ToString());
        }

        private string GetQuotedString()
        {
            System.Text.StringBuilder value = new StringBuilder();

            if (_Rule[_RuleP] != '"')
				throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationInvalidRule);
            ++_RuleP;

            while (_RuleP < _Rule.Length && _Rule[_RuleP] != '"')
            {
                value.Append(_Rule[_RuleP]);
                ++_RuleP;
            }
            if (_RuleP >= _Rule.Length || _Rule[_RuleP] != '"')
				throw new Core.LanguagePlatformException(Core.ErrorCode.SegmentationInvalidRule);
            ++_RuleP;

            return value.ToString();
        }

    }
}
