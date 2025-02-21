using System.Collections.Generic;
using Multilingual.XML.FileType.Models;

namespace Multilingual.XML.FileType.Providers.Excel.Interfaces
{
	public interface IPlaceholderPatternsProvider
	{
		bool SavePlaceholderPatterns(List<PlaceholderPattern> placeholderPatterns, string path);

		List<PlaceholderPattern> GetPlaceholderPatterns(string path, bool reset = false);
	}
}
