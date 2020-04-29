using System.Xml.Serialization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[XmlInclude(typeof(SingleStringFieldValue))]
	[XmlInclude(typeof(MultipleStringFieldValue))]
	[XmlInclude(typeof(SinglePicklistFieldValue))]
	[XmlInclude(typeof(MultiplePicklistFieldValue))]
	[XmlInclude(typeof(DateTimeFieldValue))]
	[XmlInclude(typeof(IntFieldValue))]
	public abstract class FieldValue
	{
		public string Name { get; set; }

		public abstract bool Merge(FieldValue rhs);

		public abstract bool Add(FieldValue rhs);

		public abstract bool Add(string s);

		public abstract void Clear();

		public abstract FieldValueType ValueType { get; set; }

		public abstract string GetValueString();
	}
}
