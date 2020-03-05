using Sdl.Community.BeGlobalV4.Provider.ViewModel;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class MTCodeModel: BaseViewModel
	{
		private string _language;
		private string _region;
		private string _tradosCode;
		private string _mtCodeMain;
		private string _mtCodeLocale;

		public string Language
		{
			get => _language;
			set
			{
				_language = value;
				OnPropertyChanged(nameof(Language));
			}
		}

		public string Region
		{
			get => _region;
			set
			{
				_region = value;
				OnPropertyChanged(nameof(Region));
			}
		}

		public string TradosCode
		{
			get => _tradosCode;
			set
			{
				_tradosCode = value;
				OnPropertyChanged(nameof(TradosCode));
			}
		}
		public string MTCodeMain
		{
			get => _mtCodeMain;
			set
			{
				_mtCodeMain = value;
				OnPropertyChanged(nameof(MTCodeMain));
			}
		}

		public string MTCodeLocale
		{
			get => _mtCodeLocale;
			set
			{
				_mtCodeLocale = value;
				OnPropertyChanged(nameof(MTCodeLocale));
			}
		}

		public int RowNumber { get; set; }
		public int MTCodeLocaleColumnNo { get; set; }
		public int MTCodeMainColumnNo { get; set; }
		public int LanguageColumnNo { get; set; }
		public int RegionColumnNo { get; set; }
		public int TradosCodeColumnNo { get; set; }
	}
}