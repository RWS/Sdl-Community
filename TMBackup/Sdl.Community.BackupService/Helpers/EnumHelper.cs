using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Sdl.Community.BackupService.Helpers
{
	public static class EnumHelper
	{
		public static string GetDescription(Enum value)
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				FieldInfo field = type.GetField(name);
				if (field != null)
				{
					DescriptionAttribute attr =
						   Attribute.GetCustomAttribute(field,
							 typeof(DescriptionAttribute)) as DescriptionAttribute;
					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}

		public static ArrayList GetTimeTypeDescription()
		{
			ArrayList list = new ArrayList();
			var enumValues = Enum.GetValues(typeof(Enums.TimeTypes));

			foreach (Enum value in enumValues)
			{
				list.Add(EnumHelper.GetDescription(value));
			}
			return list;
		}
	}
}