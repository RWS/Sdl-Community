namespace Sdl.Studio.SpotCheck.Helpers
{
    class WordCounter
    {
        public static int Count(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int asianWordCount = 0;
            int nonAsianWordCount = 0;
            bool inWord = false;
            foreach (char c in text)
            {
                if (c > 0x3000 && c <= 0xFFFF)
                {
                    ++asianWordCount;
                }
                else
                {
                    if (!inWord && c != ' ' && c != '\t')
                    {
                        inWord = true;
                        ++nonAsianWordCount;
                    }
                    else if (inWord && (c == ' ' || c == '\t'))
                    {
                        inWord = false;
                    }
                }
            }
            return asianWordCount + nonAsianWordCount;
        }
    }
}
