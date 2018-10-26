namespace Sdl.Community.SdlTmAnonymizer.Model.FieldDefinitions
{
	public class PicklistItem
	{
		public PicklistItem()
		{
			
		}
		public PicklistItem(string name)
		{
			Name = name;
		}

		public PicklistItem(PicklistItem other)
		{
			ID = other.ID;
			Name = other.Name;
			PreviousName = other.PreviousName;
		}

		public int? ID { get; set; }
		
		public string Name { get; set; }

		public string PreviousName { get; set; }
	}
}
