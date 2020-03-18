using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.MTCloud.Languages.Provider.Model
{
	public class MTCloudLanguage: INotifyPropertyChanged
	{
		private string _language;
		private string _region;
		private string _tradosCode;
		private string _mtCode;
		private string _mtCodeLocale;

		public int Index { get; set; }

		public string Language
		{
			get => _language;
			set
			{
				if (_language == value)
				{
					return;
				}

				_language = value;
				OnPropertyChanged(nameof(Language));
			}
		}

		public string Region
		{
			get => _region;
			set
			{
				if (_region == value)
				{
					return;
				}

				_region = value;
				OnPropertyChanged(nameof(Region));
			}
		}

		public string TradosCode
		{
			get => _tradosCode;
			set
			{
				if (_tradosCode == value)
				{
					return;
				}

				_tradosCode = value;
				OnPropertyChanged(nameof(TradosCode));
			}
		}
		public string MTCode
		{
			get => _mtCode;
			set
			{
				if (_mtCode == value)
				{
					return;
				}

				_mtCode = value;
				OnPropertyChanged(nameof(MTCode));
			}
		}

		public string MTCodeLocale
		{
			get => _mtCodeLocale;
			set
			{
				if (_mtCodeLocale == value)
				{
					return;
				}

				_mtCodeLocale = value;
				OnPropertyChanged(nameof(MTCodeLocale));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
