using System;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[Serializable]
	public class SingleStringFieldValue : FieldValue
	{
		public string Value { get; set; }

		public override bool Merge(FieldValue rhs)
		{
			if (Equals(rhs))
			{
				return false;
			}

			Value = ((SingleStringFieldValue)rhs).Value;
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

			return obj is SingleStringFieldValue stringFieldValue && 
			       string.Equals(Value, stringFieldValue.Value, StringComparison.OrdinalIgnoreCase);
		}

		public override void Clear()
		{
			Value = null;
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.SingleString;
			set { }
		}		

		public override string GetValueString()
		{
			return string.IsNullOrEmpty(Value)
				? "\"\""
				: "\"" + Value.EscapeString() + "\"";
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
