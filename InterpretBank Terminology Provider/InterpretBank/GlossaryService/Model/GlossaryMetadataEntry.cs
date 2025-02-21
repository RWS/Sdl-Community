using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.GlossaryService.Model
{
	public class GlossaryMetadataEntry : IGlossaryEntry
	{
		private static PropertyInfo[] _properties;

		public string GlossaryCreator { get; set; }
		public string GlossaryDataCreation { get; set; }
		public string GlossaryDescription { get; set; }
		public string GlossarySetting { get; set; }
		public int ID { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		private static PropertyInfo[] Properties => _properties ??= typeof(GlossaryMetadataEntry).GetProperties();

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
				var propertyType = propertyInfo?.PropertyType;
				if (value is not null) propertyInfo?.SetValue(this, System.Convert.ChangeType(value, propertyType));
			}
		}

		public List<string> GetColumns(bool includeId = false)
		{
			return Properties
				.Select(propertyInfo => propertyInfo.Name)
				.Where(propertyName => propertyName != "Item" && (propertyName != nameof(ID) || includeId))
				.ToList();
		}

		public List<object> GetValues()
		{
			return GetColumns()
				.Select(column => GetType().GetProperty(column))
				.Select(propertyInfo => propertyInfo?.GetValue(this, null))
				.ToList();
		}
	}
}