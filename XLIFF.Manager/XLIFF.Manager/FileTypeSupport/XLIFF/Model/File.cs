namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	public class File
	{
		public File()
		{
			Header = new Header();
			Body = new Body();
		}

		public string Original { get; set; }

		public string SourceLanguage { get; set; }

		public string TargetLanguage { get; set; }

		public string DataType { get; set; }

		public Header Header { get; set; }

		public Body Body { get; set; }
	}
}
