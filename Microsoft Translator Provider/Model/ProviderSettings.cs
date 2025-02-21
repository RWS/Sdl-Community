using MicrosoftTranslatorProvider.ViewModel;

namespace MicrosoftTranslatorProvider.Model
{
	public class ProviderSettings : BaseViewModel
	{
		bool _useCustomName = false;
		string _customName = null;
		bool _includeTags = true;
		bool _resendDrafts = false;
		bool _usePrelookup = false;
		string _preLookupFilePath = null;
		bool _usePostLookup = false;
		string _postLookupFilePath = null;
		bool _configureLanguages = false;

		public bool UseCustomName
		{
			get => _useCustomName;
			set
			{
				_useCustomName = value;
				OnPropertyChanged();
			}
		}

		public string CustomName
		{
			get => _customName;
			set
			{
				_customName = value;
				OnPropertyChanged();
			}
		}

		public bool IncludeTags
		{
			get => _includeTags;
			set
			{
				_includeTags = value;
				OnPropertyChanged();
			}
		}

		public bool ResendDrafts
		{
			get => _resendDrafts;
			set
			{
				_resendDrafts = value;
				OnPropertyChanged();
			}
		}

		public bool UsePreLookup
		{
			get => _usePrelookup;
			set
			{
				_usePrelookup = value;
				OnPropertyChanged();
			}
		}

		public string PreLookupFilePath
		{
			get => _preLookupFilePath;
			set
			{
				_preLookupFilePath = value;
				OnPropertyChanged();
			}
		}

		public bool UsePostLookup
		{
			get => _usePostLookup;
			set
			{
				_usePostLookup = value;
				OnPropertyChanged();
			}
		}

		public string PostLookupFilePath
		{
			get => _postLookupFilePath;
			set
			{
				_postLookupFilePath = value;
				OnPropertyChanged();
			}
		}

		public bool ConfigureLanguages
		{
			get => _configureLanguages;
			set
			{
				_configureLanguages = value;
				OnPropertyChanged();
			}
		}

		public ProviderSettings Clone()
		{
			return MemberwiseClone() as ProviderSettings;
		}
	}
}