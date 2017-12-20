using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using Sdl.Community.PostEdit.Compare.Core.Comparison.Text;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class Capitalization
	{
		public static Dictionary<string, int> CapitalLettersEdited(Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>> paragraphUnits)
		{
			var editedWordsDictionary = new Dictionary<string,int>();
			foreach (var paragraphUnit in paragraphUnits.Values)
			{
				foreach (var unit in paragraphUnit.Values)
				{
					foreach (var comparitionUnits in unit.ComparisonSegmentUnits)
					{
						var newAndRemovedUnits = comparitionUnits.ComparisonTextUnits.Where(
							u => (u.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New) ||
							     (u.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.Removed)).ToList();
						var capitalizeModifiedNumber = ComparitionUnitsModified(newAndRemovedUnits);
						if (capitalizeModifiedNumber > 0)
						{
							editedWordsDictionary.Add(unit.ParagraphId, capitalizeModifiedNumber);
						}
					}
					
				}
			}
			return editedWordsDictionary;
		}

		private static int ComparitionUnitsModified(List<TextComparer.ComparisonTextUnit> newAndRemovedUnits)
		{
			var editedWordsNumber = 0;
			var removedUnits =
				newAndRemovedUnits.Where(r => r.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.Removed).ToList();
			var addedUnits = newAndRemovedUnits.Where(a => a.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New).ToList();

			foreach (var addedUnit in addedUnits)
			{
				var corespondingRemovedUnit = removedUnits.FirstOrDefault(u => u.Text.ToLower().Equals(addedUnit.Text.ToLower()));
				if (corespondingRemovedUnit != null)
				{
					var same = addedUnit.Text.Equals(corespondingRemovedUnit.Text, StringComparison.Ordinal);
					if (!same)
					{
						editedWordsNumber++;
					}
				}
			}
			return editedWordsNumber;
		}
	}
}
