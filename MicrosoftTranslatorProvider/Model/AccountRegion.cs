using MicrosoftTranslatorProvider.ViewModel;

namespace MicrosoftTranslatorProvider.Model
{
	public class AccountRegion : BaseViewModel
	{
		public string Name { get; set; }

		public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}