using System;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[Serializable]
	public class IntFieldValue : FieldValue
	{
		public int Value { get; set; }

		public override bool Merge(FieldValue rhs)
		{
			if (Equals(rhs))
			{
				return false;
			}

			Value = ((IntFieldValue)rhs).Value;
			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			if (!(rhs is IntFieldValue v))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}

			Value += v.Value;
			return true;
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

			return obj is IntFieldValue intFieldValue && Value == intFieldValue.Value;
		}

		public override void Clear()
		{
			Value = 0;
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.Integer;
			set { }
		}

		public override string GetValueString()
		{
			return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
