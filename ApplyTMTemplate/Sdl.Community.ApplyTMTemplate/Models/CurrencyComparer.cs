using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class CurrencyComparer : IEqualityComparer<CurrencyFormat>
	{
		public bool Equals(CurrencyFormat x, CurrencyFormat y)
		{
			return x?.CurrencySymbolPositions[0] == y?.CurrencySymbolPositions[0] && x?.Symbol == y?.Symbol;
		}

		public int GetHashCode(CurrencyFormat obj)
		{
			return obj.Symbol.GetHashCode();
		}
	}
}