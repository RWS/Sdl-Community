using System.Collections.Generic;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.Providers.PlaceholderPatterns.Interfaces
{
	public interface IPlaceholderPatternsProvider
	{
		bool SavePlaceholderPatterns(List<PlaceholderPattern> placeholderPatterns, string path);

		List<PlaceholderPattern> GetPlaceholderPatterns(string path, bool reset = false);
	}
}
