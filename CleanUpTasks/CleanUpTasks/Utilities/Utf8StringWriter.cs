using System.IO;
using System.Text;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	/// <summary>
    /// Credit goes to http://stackoverflow.com/a/1564727/906773
    /// </summary>
    public sealed class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder sb) : base(sb)
        {
        }

        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}