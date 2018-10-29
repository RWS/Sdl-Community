using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	[Serializable]
	public class MultipleStringFieldValue : FieldValue
	{
		public HashSet<string> Values { get; set; }
		public HashSet<string> PreviousValues { get; set; }

		public override bool Merge(FieldValue rhs)
		{
			if (!(rhs is MultipleStringFieldValue other))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}

			var retVal = false;
			
			foreach (var s in other.Values)
			{
				retVal |= Add(s);
			}

			return retVal;
		}

		public override bool Add(string v)
		{
			if (Values == null)
			{
				Values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			return Values.Add(v);
		}

		public override bool Add(FieldValue rhs)
		{
			return Merge(rhs);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != GetType())
				return false;

			var multipleStringFieldValue = obj as MultipleStringFieldValue;

			if (multipleStringFieldValue != null && 
			    (Values == null || Values.Count == 0 || 
			     multipleStringFieldValue.Values == null || 
			     multipleStringFieldValue.Values.Count == 0))
			{
				return (Values == null || Values.Count == 0) &&
				       (multipleStringFieldValue.Values == null || multipleStringFieldValue.Values.Count == 0);

			}

			if (multipleStringFieldValue != null && Values.Count != multipleStringFieldValue.Values.Count)
			{
				return false;
			}
			
			return Values.All(v => multipleStringFieldValue != null && multipleStringFieldValue.HasValue(v));
		}

		public bool HasValue(string val)
		{
			if (Values == null)
			{
				return false;
			}
			var searchedValue = val.ToLower();

			foreach (var value in Values)
			{
				if (value.IndexOf(searchedValue, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public override void Clear()
		{			
			Values?.Clear();
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.MultipleString;
			set { }
		}

		public override string GetValueString()
		{
			var sb = new StringBuilder();

			sb.Append("(");

			var first = true;

			if (Values != null)
			{
				foreach (var s in Values)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						sb.Append(", ");
					}

					sb.Append("\"");
					sb.Append(s.EscapeString());
					sb.Append("\"");
				}
			}

			sb.Append(")");
			return sb.ToString();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			var first = true;
			foreach (var item in Values)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}

				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append(", ");
				}

				sb.Append(item);
			}

			return sb.ToString();
		}
	}
}
