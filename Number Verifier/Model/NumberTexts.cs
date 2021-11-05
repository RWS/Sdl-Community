using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;

namespace Sdl.Community.NumberVerifier.Model
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

		public bool AreAllNumbersValid => Texts.All(tp => tp.IsValidNumber);


		//public PartsList GetFlattenedList()
		//{
		//	var initialParts = new List<string>();
		//	var normalizedParts = new List<string>();
		//	var separators = new List<string>();

		//	foreach (var textPart in _textParts)
		//	{
		//		initialParts.AddRange(textPart.Parts.InitialPartsList);
		//		normalizedParts.AddRange(textPart.Parts.NormalizedPartsList);
		//		separators.AddRange(textPart.Parts.Separators);
		//	}

		//	return new PartsList(initialParts, normalizedParts, separators);

		//	return null;
		//}

	}
}