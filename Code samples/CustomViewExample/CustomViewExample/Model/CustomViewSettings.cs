using System;

namespace CustomViewExample.Model
{
	public class CustomViewSettings : ICloneable
	{
		public bool Option1 { get; set; }
		
		public bool Option2 { get; set; }

		public object Clone()
		{
			return new CustomViewSettings
			{
				Option1 = Option1,
				Option2 = Option2
			};
		}
	}
}
