using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
	public class RuleBasedStemmer : IStemmer
	{
		private StemmingRuleSet _Rules;
		private Resources.LanguageResources _Resources;

		public RuleBasedStemmer(Resources.LanguageResources resources)
		{
			_Resources = resources;
			_Rules = resources.StemmingRules;
		}

		public string Stem(string word)
		{
			string stem = word;
			int appliedRules = StemInternal(ref stem);
			return stem;
		}

		private bool ReplaceAffix(ref string word, StemmingRule.StemAction affixType,
			string affix, string replacement)
		{
			// length of replacement
			int replLen = String.IsNullOrEmpty(replacement) ? 0 : replacement.Length;

			// for circumfix only: position of |, and lengths of prefix and
			// suffix
			string circumPrefix = null;
			string circumSuffix = null;

			// length of affix and word
			int alen = affix.Length;
			int wlen = word.Length;
			int start;

			if (affixType == StemmingRule.StemAction.Circumfix)
			{
				// don't increase word's length by applying circumfix rule
				if (alen - 1 > wlen)
					return false;

				int bar = affix.IndexOf('|');
				if (bar < 0)
					throw new Core.LanguagePlatformException(Core.ErrorCode.StemmerErrorInStemmingRule, affix);

				// circumfixes can only be deleted
				// TODO: reconsider

				circumPrefix = affix.Substring(0, bar);
				circumSuffix = affix.Substring(bar + 1);
			}
			else if (alen > wlen)
			{
				return false;
			}

			switch (affixType)
			{
			case StemmingRule.StemAction.Prefix:
				if (!word.StartsWith(affix))
					return false;
				word = word.Remove(0, alen);
				if (!String.IsNullOrEmpty(replacement))
					word = replacement + word;
				break;

			case StemmingRule.StemAction.Suffix:
				if (!word.EndsWith(affix))
					return false;

				word = word.Remove(wlen - alen, alen);
				if (!String.IsNullOrEmpty(replacement))
					word = word + replacement;
				break;

			case StemmingRule.StemAction.Infix:
				start = word.IndexOf(affix);
				if (start < 0)
					return false;
				word = word.Remove(start, alen);
				if (!String.IsNullOrEmpty(replacement))
					word = word.Insert(start, replacement);
				break;

			case StemmingRule.StemAction.PrefixedInfix:
				start = word.IndexOf(affix);
				if (start <= 0)
					return false;
				word = word.Remove(start, alen);
				if (!String.IsNullOrEmpty(replacement))
					word = word.Insert(start, replacement);
				break;

			case StemmingRule.StemAction.ProperInfix:
				start = word.IndexOf(affix);
				if (start < 0 || start == 0 || start == wlen - alen)
					return false;

				word = word.Remove(start, alen);
				if (!String.IsNullOrEmpty(replacement))
					word = word.Insert(start, replacement);
				break;

			case StemmingRule.StemAction.Circumfix:
				if (word.StartsWith(circumPrefix) && word.EndsWith(circumSuffix))
				{
					// TODO what if the cp and the cs overlap?
					// don't change the order of these two statements
					word = word.Remove(wlen - circumSuffix.Length);
					word = word.Remove(circumPrefix.Length);
				}
				else
					return false;
				break;

			case StemmingRule.StemAction.Form:
				if (word.Equals(affix))
					word = replacement;
				else
					return false;
				break;

			default:
				throw new ArgumentException("Illegal affix type");
			}

			return true;
		}

		private bool ApplyRule(ref string form, int shortestStemLength, bool specialRulesOnly,
			StemmingRule rule)
		{
			int patternLength = String.IsNullOrEmpty(rule.Affix) ? 0 : rule.Affix.Length;
			int wordLength = form.Length;
			int replacementLength = String.IsNullOrEmpty(rule.Replacement) ? 0 : rule.Replacement.Length;

			bool result = false;

			// we allow that the pattern length is equal to the word length,
			// in order to support substitutions such as "went"/Prefix->"go"

			// special rules (tolower, tobase) are always executed, no matter
			// whether the minwordlength is below the original length.
			if (rule.Action == StemmingRule.StemAction.MapToLower)
			{
				form = form.ToLowerInvariant();
				result = true;
			}
			else if (rule.Action == StemmingRule.StemAction.StripDiacritics)
			{
				form = Core.CharacterProperties.ToBase(form);
				form = StripPeripheralPunctuation(form);

				result = true;
			}
			else if (specialRulesOnly)
			{
				result = false;
			}
			else if (rule.Action == StemmingRule.StemAction.TestOnBaseWord)
			{
				result = _Resources.IsStopword(form);
			}
			else if (rule.Action == StemmingRule.StemAction.DeleteLastDoubleConsonants)
			{
				if (wordLength > 2)
				{
					for (int charPos = wordLength - 1; charPos > 0; --charPos)
					{
						if (form[charPos] == form[charPos - 1] && !Core.CharacterProperties.IsVowel(form[charPos]))
						{
							form = form.Remove(charPos);
							break;
						}
					}
				}
				result = true;
			}
			else if (rule.Action == StemmingRule.StemAction.DeleteLastDoubleVowels)
			{
				if (wordLength > 2)
				{
					for (int charPos = wordLength - 1; charPos > 0; --charPos)
					{
						if (form[charPos] == form[charPos - 1] && Core.CharacterProperties.IsVowel(form[charPos]))
						{
							form = form.Remove(charPos);
							break;
						}
					}
				}
				result = true;
			}
			else if (wordLength < patternLength)
			{
				result = false;
			}
			else if (wordLength - patternLength + replacementLength < shortestStemLength)
			{
				// Don't let result string become too short
				result = false;
			}
			else
			{
				result = ReplaceAffix(ref form, rule.Action, rule.Affix, rule.Replacement);
			}

			return result;
		}

		private void BruteForceStem(ref string word)
		{
			// default, brute force stemmer

			// TODO: use default parameters of rules? 

			word = Core.CharacterProperties.ToBase(word.ToLowerInvariant());
			word = StripPeripheralPunctuation(word);

			int len = word.Length;

			if (_Rules != null)
			{
				// check for base word
				if (_Resources.IsStopword(word))
					return;
			}

			// drop one of the first double chars from the back
			for (int charPos = len - 1; charPos > 2; --charPos)
			{
				if (word[charPos] == word[charPos - 1] && !Core.CharacterProperties.IsVowel(word[charPos]))
				{
					word = word.Remove(charPos, 1);
					--len;
					break;
				}
			}

			// cut off last 1/3 of the word but at most 3 characters
			int cut = Math.Min(len / 3, 3);

			// new Thu Oct 15 14:28:44 1998 (oli)
			if ((len > 3) && ((len - cut) % 2) != 0)
				++cut;

			// force at least 3 chars to be left (minstemlength)
			if (len >= 3 && len - cut < 3)
				cut = len - 3;

			word = word.Remove(len - cut, cut);
		}

		private static string StripPeripheralPunctuation(string form)
		{
			if (String.IsNullOrEmpty(form))
				return form;

			int s = 0;
			int l = form.Length;

			while (s < l && Char.IsPunctuation(form, s))
				++s;

			if (s == l)
				// punctuation only - return literal
				return form;

			int e = l - 1;
			while (e > s && Char.IsPunctuation(form, e))
				--e;

			if (s > 0 || e < (l - 1))
				return form.Substring(s, e - s + 1);
			else
				return form;
		}

		private int StemInternal(ref string word)
		{
			if (_Rules == null || _Rules.Count == 0)
			{
				BruteForceStem(ref word);
				return 1;
			}

			// apply all affix replacement rules, in order decreasing by
			// length, secondarily applying the priorities. Since the rules
			// are already sorted that way, we just iterate through the rule
			// table and test the desired continuation.

			bool applyRules = true;
			bool specialRulesOnly = (word.Length < _Rules.MinimumWordLength);

			int appliedRules = 0;

			StemmingRuleSetIterator ruleSink = new StemmingRuleSetIterator(_Rules);
			ruleSink.First(0);

			int shortestStemLength = Math.Max(_Rules.MinimumStemLength,
			 (word.Length * _Rules.MinimumStemPercentage) / 100);

			StemmingRule theRule;

			while (applyRules)
			{
				theRule = ruleSink.Current;

				if (appliedRules > _Rules.MaximumRuleApplications || theRule == null)
				{
					applyRules = false;
				}
				else
				{
					StemmingRule.StemContinuation continuation;

					if (ApplyRule(ref word, shortestStemLength, specialRulesOnly,
					 theRule))
					{
						++appliedRules;
						continuation = theRule.ContinuationOnSuccess;
					}
					else
					{
						continuation = theRule.ContinuationOnFail;
					}

					switch (continuation)
					{
					case StemmingRule.StemContinuation.Continue:
						ruleSink.Next(theRule.ContinuationPriority);
						break;

					case StemmingRule.StemContinuation.Restart:
						ruleSink.First(0);
						break;

					case StemmingRule.StemContinuation.Stop:
						applyRules = false;
						break;

					default:
						applyRules = false;
						break;
					}
				}
			}

			return appliedRules;
		}

	}
}
