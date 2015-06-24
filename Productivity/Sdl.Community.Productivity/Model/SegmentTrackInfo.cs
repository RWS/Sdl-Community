using System;

namespace Sdl.Community.Productivity.Model
{
    public class SegmentTrackInfo
    {
        public string SegmentId { get; set; }

        public int NumberOfKeys { get; set; }

        public string Text { get; set; }

        public bool Translated { get; set; }

        public double ProductivityScore { get; set; }

        public DateTime UtcDateTime { get; set; }

        public void SetTrackInfo(int numberOfKeys, string text, bool translated)
        {
            NumberOfKeys += numberOfKeys;
            Text = text;
            UtcDateTime = DateTime.UtcNow;
            Translated = translated;
            CalculateProductivityScore();
        }

        public double CalculateProductivityScore()
        {
            var numberOfCharacters = Text.Length;

            if (NumberOfKeys > 0 && numberOfCharacters > 0)
            {
                //if (NumberOfKeys == numberOfCharacters)
                //{
                //    return ProductivityScore = 100;
                //}
                if (NumberOfKeys >= numberOfCharacters)
                {
                    return ProductivityScore = 0;
                }

                return ProductivityScore = Math.Round(100 - (double) NumberOfKeys/(double) numberOfCharacters*100d,0);
            }
            if (NumberOfKeys == 0 && numberOfCharacters > 0)
            {
                return ProductivityScore = 100;
            }
            if (numberOfCharacters == 0)
            {
                return ProductivityScore = 0;
                
            }

            return ProductivityScore;
        }

    }
}
