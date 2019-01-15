namespace IATETerminologyProvider.Model
{
	public class EntryModelObject
	{
		public EntryModel Entry { get; set; }
		public string Text { get; set; }
		public override string ToString()
		{
			return Text;
		}
	}
}
