using System;

namespace IATETerminologyProvider.Model
{
	public class EntryModelItem
	{
		public Guid Guid { get; set; }
		public EntryModel Entry { get; set; }
		public string Text { get; set; }
		public override string ToString()
		{
			return Text;
		}
	}
}
