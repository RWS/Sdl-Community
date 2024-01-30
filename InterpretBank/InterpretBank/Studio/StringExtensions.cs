using System;
using System.Linq;

namespace InterpretBank.Studio
{
	public static class StringExtensions
	{
		public static string RemoveDashesAndDigits(this string text) => text.Where(c => !char.IsDigit(c) && c != '-')
			.Aggregate("", (current, c) => current + c);
	}
}