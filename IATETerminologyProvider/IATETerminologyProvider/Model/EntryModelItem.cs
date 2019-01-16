namespace IATETerminologyProvider.Model
{
	public class EntryModelItem
	{
		public EntryModel Entry { get; set; }
		public string Text { get; set; }
		public override string ToString()
		{
			return Text;
		}
	}
}
