using System;

namespace Sdl.Community.StudioViews.Model
{
	public class MrkInfo : ICloneable
	{
		public string Mtype { get; set; }
		public string Mid { get; set; }
		public string Content { get; set; }
		public object Clone()
		{
			return new MrkInfo
			{
				Mtype = Mtype,
				Mid = Mid,
				Content = Content
			};
		}
	}
}
