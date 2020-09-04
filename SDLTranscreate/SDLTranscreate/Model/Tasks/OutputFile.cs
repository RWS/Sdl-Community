namespace Sdl.Community.Transcreate.Model.Tasks
{
	public class OutputFile
	{
		public OutputFile()
		{			
			Purpose = "WorkFile";
		}
		
		public string LanguageFileGuid { get; set; }
	
		public string Purpose { get; set; }
	}
}
