using GoogleCloudTranslationProvider.ViewModel;

namespace GoogleCloudTranslationProvider.Models
{
	public class ViewDetails : BaseViewModel
	{
		public string Name { get; set; }

		public BaseViewModel ViewModel { get; set; }
	}
}