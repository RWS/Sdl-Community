using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Sdl.Community.XLIFF.Manager.LanguageMapping.Model
{
	public class MappedLanguage : INotifyPropertyChanged
	{
		private string _languageCode;
		private string _mappedCode;
		private string _customDisplayName;
		private string _languageDisplayName;
		private string _languageName;
		private string _languageRegion;

		public int Index { get; set; }

		public string LanguageCode
		{
			get => _languageCode;
			set
			{
				if (_languageCode == value)
				{
					return;
				}

				_languageCode = value;
				OnPropertyChanged(nameof(LanguageCode));
				OnPropertyChanged(nameof(LanguageDisplayName));
			}
		}

		public string LanguageDisplayName
		{
			get => _languageDisplayName;
			set
			{
				if (_languageDisplayName == value)
				{
					return;
				}

				_languageDisplayName = value;
				OnPropertyChanged(nameof(LanguageDisplayName));

				try
				{
					if (string.IsNullOrEmpty(LanguageName) || string.IsNullOrEmpty(LanguageRegion))
					{
						SetLanguageCountryAndRegion();
					}
				}
				catch
				{
					// catch all; ignore
				}
			}
		}

		public string LanguageName
		{
			get => _languageName;
			set
			{
				if (_languageName == value)
				{
					return;
				}

				_languageName = value;
				OnPropertyChanged(nameof(LanguageName));
			}
		}

		public string LanguageRegion
		{
			get => _languageRegion;
			set
			{
				if (_languageRegion == value)
				{
					return;
				}

				_languageRegion = value;
				OnPropertyChanged(nameof(LanguageRegion));
			}
		}

		public string MappedCode
		{
			get => _mappedCode;
			set
			{
				if (_mappedCode == value)
				{
					return;
				}

				_mappedCode = value;
				OnPropertyChanged(nameof(MappedCode));
			}
		}

		public string CustomDisplayName
		{
			get => _customDisplayName;
			set
			{
				if (_customDisplayName == value)
				{
					return;
				}

				_customDisplayName = value;
				OnPropertyChanged(nameof(CustomDisplayName));
			}
		}

		private void SetLanguageCountryAndRegion()
		{
			if (string.IsNullOrEmpty(LanguageDisplayName))
			{
				return;
			}

			var regexSplit = new Regex(@"(?<language>[^\(]*)\((?<region>[^\)]*)", RegexOptions.IgnoreCase);
			if (LanguageDisplayName.Contains("(") && LanguageDisplayName.Contains(")"))
			{
				var match = regexSplit.Match(LanguageDisplayName);
				if (match.Success)
				{
					LanguageName = match.Groups["language"].Value.Trim();
					LanguageRegion = match.Groups["region"].Value.Trim();
				}
			}
			else
			{
				LanguageName = LanguageDisplayName;
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
