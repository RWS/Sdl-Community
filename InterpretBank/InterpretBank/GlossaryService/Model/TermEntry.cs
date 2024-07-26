using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.GlossaryService.Model
{
	public class TermEntry : IGlossaryEntry
	{
		private static PropertyInfo[] _properties;
		public string CommentAll { get; set; }
		public int ID { get; set; }
		public List<LanguageEquivalent> LanguageEquivalents { get; set; } = new(10);
		public string RecordCreation { get; set; } = "CURRENT_DATE";
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		private static PropertyInfo[] Properties => _properties ??= typeof(TermEntry).GetProperties();

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
					var indexAsString = Regex.Match(property, "\\d+").Value;

					if (!int.TryParse(indexAsString, out var index)) return;

					var propertyWithoutIndex = Regex.Replace(property, "\\d+", "");
					var type = typeof(LanguageEquivalent);
					var subProperty = type.GetProperty(propertyWithoutIndex);

					if (subProperty is null) return;

					var subPropertyType = subProperty.PropertyType;
					var correspondingElement = LanguageEquivalents.FirstOrDefault(le => le.LanguageIndex == index);

					if (correspondingElement is null)
					{
						correspondingElement = new LanguageEquivalent { LanguageIndex = index };
						LanguageEquivalents.Add(correspondingElement);
					}

					subProperty.SetValue(correspondingElement, Convert.ChangeType(value, subPropertyType));
				}
				else
				{
					var propertyType = propertyInfo.PropertyType;
					propertyInfo.SetValue(this, Convert.ChangeType(value, propertyType));
				}
			}
		}

		public static List<string> GetColumns(List<int> languageIndices, bool withLocation = true, bool isRead = false)
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

		public List<string> GetColumns(bool includeId = false)
		{
			var columns = Properties
				.Select(propertyInfo => propertyInfo.Name)
				.Where(propertyName => propertyName != "Item" && propertyName != nameof(LanguageEquivalents) && (propertyName != nameof(ID) || includeId))
				.ToList();

			LanguageEquivalents.ForEach(le => columns.AddRange(le.GetColumns()));

			return columns;
		}

		public List<object> GetValues()
		{
			var values = GetColumns()
				.Select(column => GetType().GetProperty(column))
				.Select(propertyInfo => propertyInfo?.GetValue(this, null))
				.ToList();

			LanguageEquivalents.ForEach(le => values.AddRange(le.GetValues()));

			return values;
		}
	}
}