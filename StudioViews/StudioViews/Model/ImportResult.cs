namespace Sdl.Community.StudioViews.Model
{
	public class ImportResult
	{
		public int UpdatedSegments { get; set; }

		public int IgnoredSegments { get; set; }

		public bool Success { get; set; }
		
		public string FilePath { get; set; }
		
		public string UpdatedFilePath { get; set; }
		
		public string BackupFilePath { get; set; }
		
		public string Message { get; set; }
	}
}
