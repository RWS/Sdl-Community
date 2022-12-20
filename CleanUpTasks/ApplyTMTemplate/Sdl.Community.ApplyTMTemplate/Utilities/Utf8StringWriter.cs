using System.IO;
using System.Text;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding => Encoding.UTF8;
	}
}