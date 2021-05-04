namespace Sdl.Community.StarTransit.Shared.Models
{
	public class TransitFileInfo
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public string Extension { get; set; }

		//TM or MT file
		public bool IsMtFile { get; set; }
	}
}
