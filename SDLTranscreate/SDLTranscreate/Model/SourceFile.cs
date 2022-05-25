namespace Trados.Transcreate.Model
{
	public class SourceFile
	{

		public enum Actions
		{
			None,
			Add,
			Persist,
			Overwrite
		}

		public SourceFile()
		{
			Action = Actions.None;
		}
		
		public string FilePath { get; set; }

		public FileData FileData { get; set; }
		
		public string FolderPathInProject { get; set; }
		
		public string FileName { get; set; }
		
		public Actions Action { get; set; }
	}
}
