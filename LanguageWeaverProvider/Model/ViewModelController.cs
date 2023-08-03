using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.Model
{
	public class ViewModelController
	{
		public string Name { get; set; }
		public ICredentialsViewModel Provider { get; set; }
	}
}