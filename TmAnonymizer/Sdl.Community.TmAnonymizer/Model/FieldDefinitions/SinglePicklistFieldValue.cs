using System;
using System.Xml.Serialization;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[XmlInclude(typeof(PicklistItem))]
	[Serializable]
	public class SinglePicklistFieldValue : FieldValue
	{
		public PicklistItem Value { get; set; }
		public PicklistItem PreviousValue { get; set; }

		public override bool Merge(FieldValue rhs)
		{
			if (rhs == null)
			{
				throw new ArgumentNullException();
			}

			if (!(rhs is SinglePicklistFieldValue r))
			{
				throw new ArgumentException(StringResources.Cant_compare_different_field_types);
			}

			if (Equals(rhs))
			{
				return false;
			}

			Value = r.Value;

			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			throw new LanguagePlatformException(ErrorCode.EditScriptInvalidOperationForFieldValueType);
		}

		public override bool Add(string s)
		{
			return false;
		}

		public override void Clear()
		{
			Value = null;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			var picklistFieldValue = obj as SinglePicklistFieldValue;

			if (picklistFieldValue != null && (Value == null || picklistFieldValue.Value == null))
			{
				return Value == null && picklistFieldValue.Value == null;
			}
			
			return picklistFieldValue != null && 
			       string.Equals(Value.Name, picklistFieldValue.Value.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.SinglePicklist;
			set { }
		}

		public override string GetValueString()
		{
			return string.IsNullOrEmpty(Value.Name)
				? "\"\""
				: "\"" + Value.Name.EscapeString() + "\"";
		}

		public override string ToString()
		{
			return Value.Name;
		}
	}
}
