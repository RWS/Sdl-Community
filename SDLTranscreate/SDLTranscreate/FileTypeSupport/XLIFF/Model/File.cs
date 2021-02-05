using System;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class File: ICloneable
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

		public object Clone()
		{
			var file = new File
			{
				Original = Original,
				SourceLanguage = SourceLanguage,
				TargetLanguage = TargetLanguage,
				DataType = DataType,
				Header = Header.Clone() as Header,
				Body = Body.Clone() as Body
			};
			
			return file;
		}
	}
}
