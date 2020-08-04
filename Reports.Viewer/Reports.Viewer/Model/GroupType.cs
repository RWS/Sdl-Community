namespace Sdl.Community.Reports.Viewer.Model
{
	public class GroupType: BaseModel
	{
		public string Name { get; set; }
		public string Type { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
