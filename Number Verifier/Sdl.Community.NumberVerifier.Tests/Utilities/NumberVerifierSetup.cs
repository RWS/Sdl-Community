using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Tests.Utilities
{
	public static class NumberVerifierSetup
	{
		public static NormalizedNumber GetNormalizedNumber(string text, string decimalSeparators, string thousandSeparators, bool noSeparator, bool omitZero)
		{
			return new NormalizedNumber
			{
				Text = text,
				DecimalSeparators = decimalSeparators,
				ThousandSeparators = thousandSeparators,
				IsNoSeparator = noSeparator,
				OmitLeadingZero = omitZero,
				NormalizedNumberList = new List<string>(),
				InitialNumberList = new List<string>()
			};
		}
	}
}