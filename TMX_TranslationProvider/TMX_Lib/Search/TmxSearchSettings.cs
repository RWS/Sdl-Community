using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;

namespace TMX_Lib.Search
{
	public class TmxSearchSettings
	{
		public SearchMode Mode  = SearchMode.NormalSearch;
		public int MaxResults = 5;
		public int MinScore = 70;
		public SortSpecification SortSpecification = new SortSpecification();

		private SearchSettings _sett;

		public bool IsConcordanceSearch => Mode == SearchMode.ConcordanceSearch || Mode == SearchMode.TargetConcordanceSearch;

		public Penalty FindPenalty(PenaltyType pt) => _sett.FindPenalty(pt);

		private TmxSearchSettings()
		{

		}

		// for testing only
		public static TmxSearchSettings Default()
		{
			return FromSearchSettings(new SearchSettings());
		}

		public static TmxSearchSettings FromSearchSettings(SearchSettings sett)
		{
			return new TmxSearchSettings
			{
				_sett = sett,
				Mode = sett.Mode,
				MaxResults = sett.MaxResults,
				MinScore = sett.MinScore,
				SortSpecification = sett.SortSpecification ?? new SortSpecification(),
			};
		}
	}
}
