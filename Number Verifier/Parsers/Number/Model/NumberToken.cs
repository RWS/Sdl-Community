using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Parsers.Number.Model
{
	public class NumberToken
	{
		public NumberToken(string text, List<NumberPart> numberParts)
		{
			Text = text;
			NumberParts = numberParts;
			Valid = numberParts?.Count > 0 && numberParts.All(a => a.Type != NumberPart.NumberType.Invalid);

			if (numberParts != null)
			{
				HasDecimalSeparator = numberParts.Any(a => a.Type == NumberPart.NumberType.DecimalSeparator);
				DecimalSeparator = numberParts.FirstOrDefault(a => a.Type == NumberPart.NumberType.DecimalSeparator)?.Value;
				HasGroupSeparator = numberParts.Any(a => a.Type == NumberPart.NumberType.GroupSeparator);
				GroupSeparator = numberParts.FirstOrDefault(a => a.Type == NumberPart.NumberType.GroupSeparator)?.Value;
				HasCurrency = numberParts.Any(a => a.Type == NumberPart.NumberType.Currency);
				Currency = numberParts.FirstOrDefault(a => a.Type == NumberPart.NumberType.Currency)?.Value;
				HasSign = numberParts.Any(a => a.Type == NumberPart.NumberType.Sign);
				Sign = numberParts.FirstOrDefault(a => a.Type == NumberPart.NumberType.Sign)?.Value;
			}
		}

		public string Text { get; }

		public List<NumberPart> NumberParts { get; }

		public bool Valid { get; }

		public bool HasDecimalSeparator { get; }
		
		public bool HasGroupSeparator { get; }
		
		public bool HasCurrency { get; }
		
		public bool HasSign { get; }

		public string DecimalSeparator { get; }

		public string GroupSeparator { get; }

		public string Currency { get; }
		
		public string Sign { get; }
	}
}
