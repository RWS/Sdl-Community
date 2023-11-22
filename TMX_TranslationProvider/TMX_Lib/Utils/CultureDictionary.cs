using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Utils
{
	internal class CultureDictionary
	{
		private Dictionary<string, CultureInfo> _cultures = new Dictionary<string, CultureInfo>();

		public CultureInfo Culture(string language)
		{
			lock (this)
			{
				if (_cultures.TryGetValue(language, out var cult))
					return cult;
				try
				{
					cult = new CultureInfo(language);
				}
				catch (Exception e)
				{
					// backup - sometimes, we actually have invalid entries that mark the language as "English" instead of "en-US"
					cult = new CultureInfo(language.ToLower().Substring(0, 2));
				}
				_cultures.Add(language, cult);
				return cult;
			}
		}
	}
}
