namespace Sdl.Community.XLIFF.Manager.Model.Tasks
{
	public class TaskFile
	{
		public TaskFile()
		{
			Guid = System.Guid.NewGuid().ToString();
			ParentTaskFileGuid = "00000000-0000-0000-0000-000000000000";
			Purpose = "WorkFile";
			Completed = "true";
		}
		
		public string Guid { get; set; }
		
		public string Completed { get; set; }
		
		public string ParentTaskFileGuid { get; set; }
		
		public string LanguageFileGuid { get; set; }
		
		public string Purpose { get; set; }
	}
}
