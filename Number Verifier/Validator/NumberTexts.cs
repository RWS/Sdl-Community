using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberTexts
	{
		public List<NumberText> Texts { get; } = new();

		public List<string> this[string key]
		{
			get => Texts.FirstOrDefault(n => n.Text == key)?.Separators;
			set => Texts.Add(new NumberText { Text = key, Separators = value });
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
				if (result is NumberText.ComparisonResult.SameSequence or NumberText.ComparisonResult.Equal)
					indexList.Add((i, result));
			}
			return indexList;
		}
	}
}