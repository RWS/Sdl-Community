using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public struct ScoredItem
	{
		public ScoredItem(int i, double s)
		{
			Item = i;
			Score = s;
		}

		public int Item;
		public double Score;
	}
}
