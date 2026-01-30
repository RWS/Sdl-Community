using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Parsers.Number.Model
{
	public class NumberToken
	{
		private readonly bool? _omitLeadingZero;

		public NumberToken(string text, List<NumberPart> numberParts, bool? omitLeadingZero)
		{
			_omitLeadingZero = omitLeadingZero;

			Text = text;
			NumberParts = numberParts;

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

		public string Currency { get; }
		public string DecimalSeparator { get; }
		public string GroupSeparator { get; }
		public bool HasCurrency { get; }
		public bool HasDecimalSeparator { get; }
		public bool HasGroupSeparator { get; }
		public bool HasSign { get; }
		public List<NumberPart> NumberParts { get; }
		public string Sign { get; }
		public string Text { get; }

		public bool Valid
		{
			get
			{
				var validityOfNumberParts = NumberParts?.Count > 0 && NumberParts.All(a => a.Type != NumberPart.NumberType.Invalid);
				var omitZeroValidity = true;

				if (_omitLeadingZero is not null)
				{
					omitZeroValidity = !IsLeadingZeroOmitted() || IsLeadingZeroOmitted() && _omitLeadingZero.Value;
				}

				return validityOfNumberParts && omitZeroValidity;
			}
		}

		public bool IsLeadingZeroOmitted() => HasSign
			? Text[1].ToString().Equals(DecimalSeparator)
			: Text[0].ToString().Equals(DecimalSeparator);
	}
}