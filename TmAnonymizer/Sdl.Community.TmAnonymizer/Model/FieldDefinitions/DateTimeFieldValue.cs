using System;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[Serializable]
	public class DateTimeFieldValue : FieldValue
	{
		private DateTime _dateTime;

		public DateTime Value
		{
			get => _dateTime;
			set => _dateTime = Normalize(value);
		}

		public override bool Merge(FieldValue rhs)
		{
			if (Equals(rhs))
			{
				return false;
			}

			Value = ((DateTimeFieldValue)rhs).Value;
			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			if (!(rhs is DateTimeFieldValue v))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			
			Value = Value.Add(new TimeSpan(v.Value.Ticks));
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
	
			return obj is DateTimeFieldValue dateTimeFieldValue && DateTime.Equals(Value, dateTimeFieldValue.Value);
		}

		public override void Clear()
		{
			Value = Normalize(DateTime.MinValue);
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.DateTime;
			set { }
		}

		public override string GetValueString()
		{
			return "\"" + _dateTime.ToString("R", System.Globalization.CultureInfo.InvariantCulture).EscapeString() + "\"";
		}

		public override string ToString()
		{
			return _dateTime.ToString("R");
		}

		private static DateTime Normalize(DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Utc)
				return dt;

			if (dt == default(DateTime)
				|| dt == DateTime.MinValue
				|| dt == DateTime.MaxValue)
			{
				return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
			}

			return dt.ToUniversalTime();
		}
	}
}
