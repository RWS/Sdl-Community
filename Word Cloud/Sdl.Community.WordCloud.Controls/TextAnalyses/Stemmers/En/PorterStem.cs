
using System.Globalization;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Stemmers.En
{
    //   Porter stemmer in CSharp, based on the C port. The original paper is in
    //       Porter, 1980, An algorithm for suffix stripping, Program, Vol. 14,
    //       no. 3, pp 130-137,
    //   See also http://www.tartarus.org/~martin/PorterStemmer

    /// <summary>
    /// The Stemmer class transforms a word into its root form.
    /// </summary>
    public struct PorterStem
    {
        private int m_CurrentIndex;
        private int m_EndIndex;
        private readonly char[] m_Term;
        private bool m_IsReduced;

        public PorterStem(string term)
            : this()
        {
            CultureInfo enUs = CultureInfo.GetCultureInfo(1033);
            m_Term = 
                term
                    .ToLower(enUs)
                    .ToCharArray();
            
            m_EndIndex = m_Term.Length - 1;
        }

        public override string ToString()
        {
            ReduceToStem();
            int length = m_EndIndex + 1;
            return new string(m_Term, 0, length);
        }

        public void ReduceToStem()
        {
            if (m_EndIndex <= 1 || m_IsReduced) return;

            TrimCommonEndings();
            TurnTerminalY2I();
            DoubleDuffices2Singles();
            HandleFullNess();
            HandleAntEnce();
            RemoveFinalE();
            m_IsReduced = true;
        }

        private static bool IsConsonantAt(char[] array, int index)
        {
            switch (array[index])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return false;
                case 'y':
                    return (index == 0) ? true : !IsConsonantAt(array, index - 1);
                default:
                    return true;
            }
        }

        /* m() measures the number of consonant sequences between 0 and currentIndex. if c is
           a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
           presence,

              <c><v>       gives 0
              <c>vc<v>     gives 1
              <c>vcvc<v>   gives 2
              <c>vcvcvc<v> gives 3
              ....
        */
        private static int CountConsonantsInSequence(char[] array, int start, int end)
        {
            int counter = 0;
            int index = start;
            while (true)
            {
                if (index > end) return counter;
                if (!IsConsonantAt(array, index)) break;
                index++;
            }
            index++;
            while (true)
            {
                while (true)
                {
                    if (index > end) return counter;
                    if (IsConsonantAt(array, index)) break;
                    index++;
                }
                index++;
                counter++;
                while (true)
                {
                    if (index > end) return counter;
                    if (!IsConsonantAt(array, index)) break;
                    index++;
                }
                index++;
            }
        }

        /* vowelinstem() is true <=> 0,...currentIndex contains a vowel */
        private static bool ContainsVowel(char[] term, int start, int end)
        {
            int index;
            for (index = start; index <= end; index++)
            {
                if (!IsConsonantAt(term, index))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsConsonant(char[] term, int start, int end)
        {
            int index;
            for (index = start; index <= end; index++)
            {
                if (IsConsonantAt(term, index))
                {
                    return true;
                }
            }
            return false;
        }

        /* doublec(index) is true <=> index,(currentIndex-1) contain a double consonant. */
        private static bool IsDoubleConsonantAt(char[] term, int index)
        {
            if (index < 1)
            {
                return false;
            }

            if (term[index] != term[index - 1])
            {
                return false;
            }
            return IsConsonantAt(term, index);
        }

        /* cvc(index) is true <=> index-2,index-1,index has the form consonant - vowel - consonant
           and also if the second c is not w,x or y. this is used when trying to
           restore an e at the end of a short word. e.g.

              cav(e), lov(e), hop(e), crim(e), but
              snow, box, tray.

        */
        private static bool IsConsonantVowelConsonantAt(char[] term, int index)
        {
            if (index < 2 || !IsConsonantAt(term, index) || IsConsonantAt(term, index - 1) || !IsConsonantAt(term, index - 2))
            {
                return false;
            }
            int ch = term[index];
            if (ch == 'w' || ch == 'x' || ch == 'y')
            {
                return false;
            }
            return true;
        }

        private bool TrimIfEndsWith(string s)
        {
            if (EndsWith(m_Term, m_EndIndex, s))
            {
                m_CurrentIndex = m_EndIndex - s.Length;
                return true;
            }
            return false;
        }

        private static bool EndsWith(char[] term, int end, string ending)
        {
            int startOffset = end - ending.Length + 1;
            if (startOffset < 0)
            {
                return false;
            }
            for (int index = 0; index < ending.Length; index++)
            {
                if (term[startOffset + index] != ending[index])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ReplaceEnding(char[] term, string originalEnding, string newEnding, ref int end)
        {
            if (!EndsWith(term, end, originalEnding))
            {
                return false;
            }

            int length = newEnding.Length;
            int startOffest = end - originalEnding.Length + 1;
            for (int index = 0; index < length; index++)
            {
                term[startOffest + index] = newEnding[index];
            }
            end = end - originalEnding.Length + newEnding.Length;
            return true;
        }

        /* setto(s) sets (currentIndex+1),...endIndex to the characters in the string s, readjusting endIndex. */
        private void SetEndingTo(string s)
        {
            int l = s.Length;
            int o = m_CurrentIndex + 1;
            char[] sc = s.ToCharArray();
            for (int index = 0; index < l; index++)
                m_Term[o + index] = sc[index];
            m_EndIndex = m_CurrentIndex + l;
        }

        /* r(s) is used further down. */
        private void SetEndingIfContainsConsonants(string s)
        {
            if (ContainsConsonant(m_Term, 0, m_CurrentIndex))
            {
                SetEndingTo(s);
            }
        }

        /* step1() gets rid of plurals and -ed or -ing. e.g.
               caresses  ->  caress
               ponies    ->  poni
               ties      ->  ti
               caress    ->  caress
               cats      ->  cat

               feed      ->  feed
               agreed    ->  agree
               disabled  ->  disable

               matting   ->  mat
               mating    ->  mate
               meeting   ->  meet
               milling   ->  mill
               messing   ->  mess

               meetings  ->  meet

        */

        private void TrimCommonEndings()
        {
            if (m_Term[m_EndIndex] == 's')
            {
                if (ReplaceEnding(m_Term, "sses", "ss", ref m_EndIndex))
                {
                    //m_EndIndex -= 2;
                }
                else if (ReplaceEnding(m_Term, "ies", "i", ref m_EndIndex))
                {
                    //Setto("i");
                }
                else if (m_Term[m_EndIndex - 1] != 's')
                {
                    ReplaceEnding(m_Term, "s", string.Empty, ref m_EndIndex);
                }
            }

            if (TrimIfEndsWith("eed"))
            {
                if (CountConsonantsInSequence(m_Term, 0, m_CurrentIndex) > 0)
                    m_EndIndex--;
            }
            else if ((TrimIfEndsWith("ed") || TrimIfEndsWith("ing")) && ContainsVowel(m_Term, 0, m_CurrentIndex))
            {
                m_EndIndex = m_CurrentIndex;
                if (TrimIfEndsWith("at"))
                    SetEndingTo("ate");
                else if (TrimIfEndsWith("bl"))
                    SetEndingTo("ble");
                else if (TrimIfEndsWith("iz"))
                    SetEndingTo("ize");
                else if (IsDoubleConsonantAt(m_Term, m_EndIndex))
                {
                    m_EndIndex--;
                    int ch = m_Term[m_EndIndex];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        m_EndIndex++;
                }
                else if (CountConsonantsInSequence(m_Term, 0, m_CurrentIndex) == 1 && IsConsonantVowelConsonantAt(m_Term, m_EndIndex)) SetEndingTo("e");
            }
        }

        /* step2() turns terminal y to i when there is another vowel in the stem. */
        private void TurnTerminalY2I()
        {
            if (TrimIfEndsWith("y") && ContainsVowel(m_Term, 0, m_CurrentIndex))
                m_Term[m_EndIndex] = 'i';
        }

        /* step3() maps double suffices to single ones. so -ization ( = -ize plus
           -ation) maps to -ize etc. note that the string before the suffix must give
           m() > 0. */
        private void DoubleDuffices2Singles()
        {
            if (m_EndIndex == 0)
                return;

            switch (m_Term[m_EndIndex - 1])
            {
                case 'a':
                    if (TrimIfEndsWith("ational"))
                    {
                        SetEndingIfContainsConsonants("ate");
                        break;
                    }
                    if (TrimIfEndsWith("tional"))
                    {
                        SetEndingIfContainsConsonants("tion");
                        break;
                    }
                    break;
                case 'c':
                    if (TrimIfEndsWith("enci"))
                    {
                        SetEndingIfContainsConsonants("ence");
                        break;
                    }
                    if (TrimIfEndsWith("anci"))
                    {
                        SetEndingIfContainsConsonants("ance");
                        break;
                    }
                    break;
                case 'e':
                    if (TrimIfEndsWith("izer"))
                    {
                        SetEndingIfContainsConsonants("ize");
                        break;
                    }
                    break;
                case 'l':
                    if (TrimIfEndsWith("bli"))
                    {
                        SetEndingIfContainsConsonants("ble");
                        break;
                    }
                    if (TrimIfEndsWith("alli"))
                    {
                        SetEndingIfContainsConsonants("al");
                        break;
                    }
                    if (TrimIfEndsWith("entli"))
                    {
                        SetEndingIfContainsConsonants("ent");
                        break;
                    }
                    if (TrimIfEndsWith("eli"))
                    {
                        SetEndingIfContainsConsonants("e");
                        break;
                    }
                    if (TrimIfEndsWith("ousli"))
                    {
                        SetEndingIfContainsConsonants("ous");
                        break;
                    }
                    break;
                case 'o':
                    if (TrimIfEndsWith("ization"))
                    {
                        SetEndingIfContainsConsonants("ize");
                        break;
                    }
                    if (TrimIfEndsWith("ation"))
                    {
                        SetEndingIfContainsConsonants("ate");
                        break;
                    }
                    if (TrimIfEndsWith("ator"))
                    {
                        SetEndingIfContainsConsonants("ate");
                        break;
                    }
                    break;
                case 's':
                    if (TrimIfEndsWith("alism"))
                    {
                        SetEndingIfContainsConsonants("al");
                        break;
                    }
                    if (TrimIfEndsWith("iveness"))
                    {
                        SetEndingIfContainsConsonants("ive");
                        break;
                    }
                    if (TrimIfEndsWith("fulness"))
                    {
                        SetEndingIfContainsConsonants("ful");
                        break;
                    }
                    if (TrimIfEndsWith("ousness"))
                    {
                        SetEndingIfContainsConsonants("ous");
                        break;
                    }
                    break;
                case 't':
                    if (TrimIfEndsWith("aliti"))
                    {
                        SetEndingIfContainsConsonants("al");
                        break;
                    }
                    if (TrimIfEndsWith("iviti"))
                    {
                        SetEndingIfContainsConsonants("ive");
                        break;
                    }
                    if (TrimIfEndsWith("biliti"))
                    {
                        SetEndingIfContainsConsonants("ble");
                        break;
                    }
                    break;
                case 'g':
                    if (TrimIfEndsWith("logi"))
                    {
                        SetEndingIfContainsConsonants("log");
                        break;
                    }
                    break;
                default:
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
        private void HandleFullNess()
        {
            switch (m_Term[m_EndIndex])
            {
                case 'e':
                    if (TrimIfEndsWith("icate"))
                    {
                        SetEndingIfContainsConsonants("ic");
                        break;
                    }
                    if (TrimIfEndsWith("ative"))
                    {
                        SetEndingIfContainsConsonants("");
                        break;
                    }
                    if (TrimIfEndsWith("alize"))
                    {
                        SetEndingIfContainsConsonants("al");
                        break;
                    }
                    break;
                case 'i':
                    if (TrimIfEndsWith("iciti"))
                    {
                        SetEndingIfContainsConsonants("ic");
                        break;
                    }
                    break;
                case 'l':
                    if (TrimIfEndsWith("ical"))
                    {
                        SetEndingIfContainsConsonants("ic");
                        break;
                    }
                    if (TrimIfEndsWith("ful"))
                    {
                        SetEndingIfContainsConsonants("");
                        break;
                    }
                    break;
                case 's':
                    if (TrimIfEndsWith("ness"))
                    {
                        SetEndingIfContainsConsonants("");
                        break;
                    }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
        private void HandleAntEnce()
        {
            if (m_EndIndex == 0)
                return;

            switch (m_Term[m_EndIndex - 1])
            {
                case 'a':
                    if (TrimIfEndsWith("al")) break;
                    return;
                case 'c':
                    if (TrimIfEndsWith("ance")) break;
                    if (TrimIfEndsWith("ence")) break;
                    return;
                case 'e':
                    if (TrimIfEndsWith("er")) break;
                    return;
                case 'i':
                    if (TrimIfEndsWith("ic")) break;
                    return;
                case 'l':
                    if (TrimIfEndsWith("able")) break;
                    if (TrimIfEndsWith("ible")) break;
                    return;
                case 'n':
                    if (TrimIfEndsWith("ant")) break;
                    if (TrimIfEndsWith("ement")) break;
                    if (TrimIfEndsWith("ment")) break;
                    /* element etc. not stripped before the m */
                    if (TrimIfEndsWith("ent")) break;
                    return;
                case 'o':
                    if (TrimIfEndsWith("ion") && m_CurrentIndex >= 0 && (m_Term[m_CurrentIndex] == 's' || m_Term[m_CurrentIndex] == 't')) break;
                    /* currentIndex >= 0 fixes Bug 2 */
                    if (TrimIfEndsWith("ou")) break;
                    return;
                    /* takes care of -ous */
                case 's':
                    if (TrimIfEndsWith("ism")) break;
                    return;
                case 't':
                    if (TrimIfEndsWith("ate")) break;
                    if (TrimIfEndsWith("iti")) break;
                    return;
                case 'u':
                    if (TrimIfEndsWith("ous")) break;
                    return;
                case 'v':
                    if (TrimIfEndsWith("ive")) break;
                    return;
                case 'z':
                    if (TrimIfEndsWith("ize")) break;
                    return;
                default:
                    return;
            }
            if (CountConsonantsInSequence(m_Term, 0, m_CurrentIndex) > 1)
                m_EndIndex = m_CurrentIndex;
        }

        /* step6() removes a final -e if m() > 1. */
        private void RemoveFinalE()
        {
            m_CurrentIndex = m_EndIndex;

            if (m_Term[m_EndIndex] == 'e')
            {
                int a = CountConsonantsInSequence(m_Term, 0, m_CurrentIndex);
                if (a > 1 || a == 1 && !IsConsonantVowelConsonantAt(m_Term, m_EndIndex - 1))
                    m_EndIndex--;
            }
            if (m_Term[m_EndIndex] == 'l' && IsDoubleConsonantAt(m_Term, m_EndIndex) && CountConsonantsInSequence(m_Term, 0, m_CurrentIndex) > 1)
                m_EndIndex--;
        }
    }
}