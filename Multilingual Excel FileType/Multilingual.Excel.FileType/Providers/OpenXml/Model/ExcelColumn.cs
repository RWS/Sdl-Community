using System;

namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelColumn: ICloneable
	{
		public uint Index { get; set; }

		public string Name { get; set; }

		public string Text { get; set; }

		public object Clone()
		{
			return new ExcelColumn
			{
				Index = Index,
				Name = Name,
				Text = Text
			};
		}
	}
}
