using System.IO;
using System.Text;

namespace Sdl.Community.MTCloud.Provider.Service.QEReportCreator
{
	public sealed class StringWriterWithEncoding : StringWriter
	{
		public override Encoding Encoding { get; }

		public StringWriterWithEncoding(Encoding encoding)
		{
			Encoding = encoding;
		}
	}
}
