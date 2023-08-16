using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI
{
	/// <summary>
	/// Interaction logic for ImportGlossaryWindow.xaml
	/// </summary>
	public partial class BrowseGlossaryWindow : Window, INotifyPropertyChanged
	{
		private int _selectedIndexSourceLanguage;
		private int _selectedIndexTargetLanguage;
		private List<string> _supportedLanguages;

		public BrowseGlossaryWindow(List<string> supportedLanguages)
		{
			InitializeComponent();
			SupportedLanguages = supportedLanguages;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string Path { get; set; }

		public int SelectedIndexSourceLanguage
		{
			get => _selectedIndexSourceLanguage;
			set => SetField(ref _selectedIndexSourceLanguage, value);
		}

		public int SelectedIndexTargetLanguage
		{
			get => _selectedIndexTargetLanguage;
			set => SetField(ref _selectedIndexTargetLanguage, value);
		}

		public List<string> SupportedLanguages
		{
			get => _supportedLanguages;
			set => SetField(ref _supportedLanguages, value);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void ImportButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Escape)
				return;

			DialogResult = false;
			Close();
		}
	}
}