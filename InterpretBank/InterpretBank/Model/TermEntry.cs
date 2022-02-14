using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InterpretBank.Model.Interface;

namespace InterpretBank.Model
{
	public class TermEntry : IGlossaryEntry
	{
		public string CommentAll { get; set; }
		public string ID { get; set; }
		public List<LanguageEquivalent> LanguageEquivalents { get; set; } = new();
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }

		public TermEntry(){}

		public TermEntry(Dictionary<string, string> row)
		{
			CommentAll = row[nameof(CommentAll)];
			ID = row[nameof(ID)];
			Tag1 = row[nameof(Tag1)];
			Tag2 = row[nameof(Tag2)];

			var indices = row.Keys.Where(key => key.Contains("Term")).Select(key => key.Substring(4));

			foreach (var index in indices)
			{
				LanguageEquivalents.Add(new LanguageEquivalent
				{
					LanguageIndex = int.Parse(index),
					Commenta = row[$"Comment{index}a"],
					Commentb = row[$"Comment{index}b"],
					Term = row[$"Term{index}"]
				});
			}
		}

		public string this[string property]
		{
			get
			{
				var propertyInfo = GetType().GetProperty(property);
				return propertyInfo?.GetValue(this, null).ToString();
			}
			set
			{
				var propertyInfo = GetType().GetProperty(property);

				if (propertyInfo is null)
				{
					var firstDigit = property.FirstOrDefault(char.IsDigit);
					var indexAsString = Regex.Match(property, "\\d+").Value;
					var index = int.Parse(indexAsString);

					if (index == -1) return;

					var propertyWithoutIndex = Regex.Replace(property, "\\d+", "");

					var correspondingElement = LanguageEquivalents.FirstOrDefault(le => le.LanguageIndex == index) ??
					                           new LanguageEquivalent {LanguageIndex = index};

					var type = typeof(LanguageEquivalent);
					var subProperty = type.GetProperty(propertyWithoutIndex);

					var subPropertyType = subProperty?.PropertyType;
					subProperty?.SetValue(correspondingElement, Convert.ChangeType(value, subPropertyType));
					LanguageEquivalents.Add(correspondingElement);
				}
				else
				{
					var propertyType = propertyInfo.PropertyType;
					propertyInfo.SetValue(this, Convert.ChangeType(value, propertyType));
				}
			}
		}

		public static List<string> GetTermColumns(List<int> languageIndices, bool withLocation = true, bool isRead = false)
		{
			var columns = new List<string> { "CommentAll" };

			if (withLocation)
			{
				columns.Insert(0, "Tag2");
				columns.Insert(0, "Tag1");
				if (isRead) columns.Insert(0, "ID");
			}

			foreach (var le in languageIndices)
			{
				columns.AddRange(LanguageEquivalent.GetColumns(le));
			}

			return columns;
		}

		public void Add(LanguageEquivalent languageEquivalent)
		{
			LanguageEquivalents.Add(languageEquivalent);
		}

		public List<string> GetColumns()
		{
			var columns = new List<string> { "Tag1", "Tag2", "CommentAll" };
			foreach (var le in LanguageEquivalents)
			{
				columns.AddRange(le.GetColumns());
			}

			return columns;
		}

		public List<string> GetValues()
		{
			var values = new List<string> { Tag1, Tag2, CommentAll };
			foreach (var le in LanguageEquivalents)
			{
				values.AddRange(le.GetValues());
			}

			return values;
		}

		public Dictionary<string, string> ToRow()
		{
			throw new NotImplementedException();
		}
	}
}