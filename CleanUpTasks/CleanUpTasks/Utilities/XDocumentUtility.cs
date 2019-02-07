using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public static class XDocumentUtility
    {
        /// <summary>
        /// Credit goes to http://stackoverflow.com/a/1229009/906773
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string ToStringWithDeclaration(this XDocument doc)
        {

            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new Utf8StringWriter(builder))
            {
                doc.Save(writer);
            }

            return builder.ToString();
        }
    }
}