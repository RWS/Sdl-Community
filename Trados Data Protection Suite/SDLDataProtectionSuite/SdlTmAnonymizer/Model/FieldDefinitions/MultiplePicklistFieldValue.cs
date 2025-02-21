using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.FieldDefinitions
{
	[XmlInclude(typeof(PicklistItem))]
	[Serializable]
	public class MultiplePicklistFieldValue : FieldValue
	{
		public List<PicklistItem> Values { get; set; }
		public List<PicklistItem> PreviousValues { get; set; }

		public override bool Merge(FieldValue rhs)
		{
			if (!(rhs is MultiplePicklistFieldValue other))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}

			var retVal = false;

			foreach (var item in other.Values)
				retVal |= Add(item);

			return retVal;
		}

		public override bool Add(FieldValue rhs)
		{
			return Merge(rhs);
		}

		public override bool Add(string s)
		{
			var v = new PicklistItem(s);
			return Add(v);
		}

		public bool Add(PicklistItem v)
		{
			if (HasValue(v))
			{
				return false;
			}

			Values.Add(v);
			return true;
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

			var multiplePicklistFieldValue = obj as MultiplePicklistFieldValue;

			if (multiplePicklistFieldValue != null && (Values == null || Values.Count == 0 ||
			                                           multiplePicklistFieldValue.Values == null ||
			                                           multiplePicklistFieldValue.Values.Count == 0))
			{
				return (Values == null || Values.Count == 0) && (multiplePicklistFieldValue.Values == null || multiplePicklistFieldValue.Values.Count == 0);
			}

			if (multiplePicklistFieldValue != null && Values.Count != multiplePicklistFieldValue.Values.Count)
			{
				return false;
			}

			
			return Values.All(pli => multiplePicklistFieldValue != null && multiplePicklistFieldValue.HasValue(pli));
		}

		public bool HasValue(PicklistItem v)
		{
			return HasValue(v.Name);
		}

		public bool HasValue(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentNullException();
			}

			return Values != null && Values.Any(i => i.Name.Equals(s, StringComparison.OrdinalIgnoreCase));
		}

		public override void Clear()
		{			
			Values.Clear();
		}

		public override FieldValueType ValueType
		{
			get => FieldValueType.MultiplePicklist;
			set { }
		}

		public override string GetValueString()
		{
			var sb = new StringBuilder();

			sb.Append("(");

			for (var p = 0; p < Values.Count; ++p)
			{
				if (p > 0)
				{
					sb.Append(", ");
				}

				sb.Append("\"");
				sb.Append(Values[p].Name.EscapeString());
				sb.Append("\"");
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
				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append(", ");
				}

				sb.Append(item.Name);
			}

			return sb.ToString();
		}
	}
}
