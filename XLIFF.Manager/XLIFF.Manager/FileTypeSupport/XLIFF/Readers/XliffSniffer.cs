using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers
{
	public class XliffSniffer
	{
		private const string NsPrefix = "sdlxliff";

		public Enumerators.XLIFFSupport GetXliffSupport(string filePath)
		{
			var buffer = new char[500];
			using (var streamReader = new StreamReader(filePath, Encoding.UTF8, true))
			{
				streamReader.Read(buffer, 0, buffer.Length);
			}

			var data = new string(buffer);
			if (string.IsNullOrEmpty(data))
			{
				return Enumerators.XLIFFSupport.none;
			}

			var regex = new Regex(NsPrefix + @"\:support\=""(?<value>[^""]*)""", RegexOptions.IgnoreCase);
			var match = regex.Match(data);
			if (match.Success)
			{
				var value = match.Groups["value"].Value;
				var success = Enum.TryParse(value, true, out Enumerators.XLIFFSupport support);
				if (success)
				{
					return support;
				}
			}

			return Enumerators.XLIFFSupport.none;
		}
	}
}
