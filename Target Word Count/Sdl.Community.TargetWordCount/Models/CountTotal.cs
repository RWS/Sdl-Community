using System.Collections.Generic;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.TargetWordCount.Models
{
	public sealed class CountTotal
    {
        public const string ContextMatch = "ContextMatch";
        public const string CrossFileRepetitions = "CrossFileRepetitions";
        public const string EightyFivePercent = "EightyFivePercent";
        public const string FiftyPercent = "FiftyPercent";
        public const string Locked = "Locked";
        public const string New = "New";
        public const string NinetyFivePercent = "NinetyFivePercent";
        public const string OneHundredPercent = "OneHundredPercent";
        public const string PerfectMatch = "PerfectMatch";
        public const string Repetitions = "Repetitions";
        public const string SeventyFivePercent = "SeventyFivePercent";
        public const string Total = "Total";

        private readonly Dictionary<string, CountData> info = new Dictionary<string, CountData>()
                {
                    { Locked, new CountData() },
                    { ContextMatch, new CountData() },
                    { Repetitions, new CountData() },
                    { CrossFileRepetitions, new CountData() },
                    { OneHundredPercent, new CountData() },
                    { NinetyFivePercent, new CountData() },
                    { EightyFivePercent, new CountData() },
                    { SeventyFivePercent, new CountData() },
                    { FiftyPercent, new CountData() },
                    { New, new CountData() },
                    { PerfectMatch, new CountData() },
                    { Total, new CountData() },
                };

        public CountUnit CountMethod { get; set; }

        public string FileName { get; set; }

        public int LockedSpaceCountTotal { get; set; } = 0;

        public int UnlockedSpaceCountTotal { get; set; } = 0;

        public Dictionary<string, CountData> Totals { get { return info; } }

        public void Increment(string key, CountData countData)
        {
            info[key].Increment(countData);
        }

        public void Increment(CountTotal total)
        {
			if (total != null)
			{
				info[Locked].Increment(total.info[Locked]);
				info[ContextMatch].Increment(total.info[ContextMatch]);
				info[Repetitions].Increment(total.info[Repetitions]);
				info[CrossFileRepetitions].Increment(total.info[CrossFileRepetitions]);
				info[OneHundredPercent].Increment(total.info[OneHundredPercent]);
				info[NinetyFivePercent].Increment(total.info[NinetyFivePercent]);
				info[EightyFivePercent].Increment(total.info[EightyFivePercent]);
				info[SeventyFivePercent].Increment(total.info[SeventyFivePercent]);
				info[FiftyPercent].Increment(total.info[FiftyPercent]);
				info[New].Increment(total.info[New]);
				info[PerfectMatch].Increment(total.info[PerfectMatch]);
				info[Total].Increment(total.info[Total]);
			}
        }
    }
}