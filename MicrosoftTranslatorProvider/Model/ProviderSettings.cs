using MicrosoftTranslatorProvider.ViewModel;

namespace MicrosoftTranslatorProvider.Model
{
	public class ProviderSettings : BaseViewModel
	{
		private bool _useCustomName = false;
		private string _customName = null;
		private bool _includeTags = true;
		private bool _resendDrafts = false;
		private bool _usePrelookup = false;
		private string _preLookupFilePath = null;
		private bool _usePostLookup = false;
		private string _postLookupFilePath = null;

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

		public ProviderSettings Clone()
		{
			return MemberwiseClone() as ProviderSettings;
		}
	}
}