using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberTexts
	{
		public List<NumberText> Texts { get; } = new();

		public void AddNumberText(string text, List<string> separators, int startIndex, int endIndex)
		{
			Texts.Add(new NumberText { Text = text, Separators = separators, StartIndex = startIndex, Length = endIndex });
		}

		public NumberText this[int key]
		{
			get
			{
				return key >= Texts.Count ? null : Texts[key];
			}
		}

		public List<(int, NumberText.ComparisonResult)> IndexesOf(NumberText other)
		{
			var indexList = new List<(int, NumberText.ComparisonResult)>();
			for (var i = 0; i < Texts.Count; i++)
			{
				var result = Texts[i].Compare(other);
				if (result > NumberText.ComparisonResult.DifferentSequence)
					indexList.Add((i, result));
			}

			indexList.OrderByDescending(i => i);
			return indexList;
		}
	}
}