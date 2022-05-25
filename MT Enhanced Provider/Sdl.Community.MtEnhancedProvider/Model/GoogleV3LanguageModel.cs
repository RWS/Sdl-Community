using System.Globalization;

namespace Sdl.Community.MtEnhancedProvider.Model
{
	public class GoogleV3LanguageModel
	{
		public string GoogleLanguageCode { get; set; }
		public bool SupportTarget { get; set; }
		public bool SupportSource { get; set; }
		public CultureInfo CultureInfo { get; set; }
	}
}
