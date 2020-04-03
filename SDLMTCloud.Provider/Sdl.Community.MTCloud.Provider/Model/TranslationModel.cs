using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationModel : BaseViewModel
	{	
		public string DisplayName { get; set; }

		public string Source { get; set; }

		public string Target { get; set; }

		public MTCloudLanguagePair MTCloudLanguagePair { get; set; }
				
		public string Model { get; set; }
	}
}