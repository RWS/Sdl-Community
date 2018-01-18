using System;
using System.ComponentModel;

namespace Sdl.Community.BackupService.Helpers
{
	public static class Enums
	{
		public enum TimeTypes
		{		
			[Description("minutes")]
			Minutes = 0,
			[Description("hours")]
			Hours = 1
		}

		public static string GetDescription(this Enum value)
		{
			var type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				var field = type.GetField(name);
				if (field != null)
				{
					var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}
	}
}