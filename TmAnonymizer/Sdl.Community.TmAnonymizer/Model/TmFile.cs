using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TmFile : ModelBase
	{
		private bool _isSelected;
		private string _name;
		private string _description;
		private string _path;
		private string _cachePath;
		private bool _isTmCache;
		private bool _shouldRemove;
		private bool _isServerTm;
		private bool _isLoaded;
		private string _type;
		private int _translationUnits;
		private List<TmLanguageDirection> _tmLanguageDirections;

		public TmFile()
		{
			IsSelected = false;
			IsServerTm = false;
		}

		public Credentials Credentials { get; set; }

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Type
		{
			get => _type;
			set
			{
				_type = value;
				OnPropertyChanged(nameof(Type));
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public string CachePath
		{
			get => _cachePath;
			set
			{
				_cachePath = value;

				OnPropertyChanged(nameof(CachePath));
				OnPropertyChanged(nameof(IsTmCache));
			}
		}

		public bool IsTmCache
		{
			get { return !string.IsNullOrEmpty(_cachePath); }
		}

		public bool IsLoaded
		{

			get => _isLoaded;
			set
			{
				_isLoaded = value;
				OnPropertyChanged(nameof(IsLoaded));
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;

				if (!_isSelected && TmLanguageDirections != null)
				{
					foreach (var languageDirection in TmLanguageDirections)
					{
						languageDirection?.TranslationUnits?.Clear();
					}
				}

				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public bool ShouldRemove
		{

			get => _shouldRemove;
			set
			{
				_shouldRemove = value;
				OnPropertyChanged(nameof(ShouldRemove));
			}
		}

		public int TranslationUnits
		{
			get => _translationUnits;
			set
			{
				_translationUnits = value;
				OnPropertyChanged(nameof(TranslationUnits));
			}
		}

		public List<TmLanguageDirection> TmLanguageDirections
		{
			get => _tmLanguageDirections ?? (_tmLanguageDirections = new List<TmLanguageDirection>());
			set
			{
				_tmLanguageDirections = value;
				OnPropertyChanged(nameof(TmLanguageDirections));
			}
		}

		public bool IsServerTm
		{

			get => _isServerTm;
			set
			{
				_isServerTm = value;
				OnPropertyChanged(nameof(IsServerTm));
			}
		}
	}
}
