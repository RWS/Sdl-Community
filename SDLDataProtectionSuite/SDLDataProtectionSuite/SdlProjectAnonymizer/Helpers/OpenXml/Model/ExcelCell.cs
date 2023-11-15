using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml.Model
{
	public class ExcelCell: ICloneable
	{
		public string Value { get; set; }

		public ExcelColumn Column { get; set; }

		public string Background { get; set; }
		
		public object Clone()
		{
			return new ExcelCell
			{
				Background = Background,
				Value = Value,
				Column = Column.Clone() as ExcelColumn
			};

		}
	}
}
