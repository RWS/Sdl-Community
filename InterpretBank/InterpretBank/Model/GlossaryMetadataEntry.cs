using System.Collections.Generic;
using InterpretBank.Model.Interface;

namespace InterpretBank.Model
{
	public class GlossaryMetadataEntry : IGlossaryEntry
	{
		public GlossaryMetadataEntry()
		{
			
		}
		//public GlossaryMetadataEntry(Dictionary<string, string> row)
		//{
		//	GlossaryCreator = row[nameof(GlossaryCreator)];
		//	GlossaryDataCreation = row[nameof(GlossaryDataCreation)];
		//	GlossaryDescription = row[nameof(GlossaryDescription)];
		//	ID = row[nameof(ID)];
		//	GlossarySetting = row[nameof(GlossarySetting)];
		//	Tag1 = row[nameof(Tag1)];
		//	Tag2 = row[nameof(Tag2)];
		//}

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
				if(value is not null) propertyInfo?.SetValue(this, System.Convert.ChangeType(value, propertyType));
			}
		}

		public string GlossaryCreator { get; set; }
		public string GlossaryDataCreation { get; set; }
		public string GlossaryDescription { get; set; }
		public string GlossarySetting { get; set; }
		public int ID { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }

		public List<string> GetColumns() =>
			new()
			{
				nameof(ID),
				nameof(Tag1),
				nameof(Tag2),
				nameof(GlossarySetting),
				nameof(GlossaryDescription),
				nameof(GlossaryDataCreation),
				nameof(GlossaryCreator),
			};

		public List<string> GetValues() =>
			new()
			{
				Tag1,
				Tag2,
				GlossarySetting,
				GlossaryDescription,
				GlossaryDataCreation,
				GlossaryCreator
			};
	}
}