using System.IO;
using System.Text;

namespace Sdl.Community.MTCloud.Provider.Service.QEReportCreator
{
	public sealed class StringWriterWithEncoding : StringWriter
	{
		public StringWriterWithEncoding(Encoding encoding)
		{
			Encoding = encoding;
		}

		public override Encoding Encoding { get; }
	}
}