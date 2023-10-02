using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InterpretBank.Wrappers;
using InterpretBank.Wrappers.Interface;
using Newtonsoft.Json;

namespace InterpretBank.SettingsService.UI
{
	/// <summary>
	/// Interaction logic for ChooseFilepathControl.xaml
	/// </summary>
	public partial class ChooseFilepathControl : UserControl
	{
		public static readonly DependencyProperty AutoCompleteOptionsProperty =
			DependencyProperty.Register(nameof(AutoCompleteOptions), typeof(List<string>),
				typeof(ChooseFilepathControl), new PropertyMetadata(default(List<string>)));

		public static readonly DependencyProperty FilepathProperty =
					DependencyProperty.Register(nameof(Filepath), typeof(string), typeof(ChooseFilepathControl),
				new PropertyMetadata(string.Empty));

		private static readonly string DbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			@"Trados AppStore\InterpretBank\DatabaseList.json");

		public ChooseFilepathControl()
		{
			InitializeComponent();

			if (!File.Exists(DbListPath))
			{
				using var file = File.Create(DbListPath);
			}

			DatabaseList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(DbListPath)) ?? new List<string>();
		}

		public List<string> AutoCompleteOptions
		{
			get => (List<string>)GetValue(AutoCompleteOptionsProperty);
			set => SetValue(AutoCompleteOptionsProperty, value);
		}

		public List<string> DatabaseList { get; set; }

		public string Filepath
		{
			get => (string)GetValue(FilepathProperty);
			set
			{
				if (!File.Exists(value))
					return;

				AddToDatabaseList(value);

				SetValue(FilepathProperty, value);
			}
		}

		public int SelectedIndex { get; set; }
		private IDialog OpenFileDialog { get; } = new Dialog();

		private void AddToDatabaseList(string filepath)
		{
			if (!DatabaseList.Contains(filepath))
				DatabaseList.Add(filepath);
			File.WriteAllText(DbListPath, JsonConvert.SerializeObject(DatabaseList));
		}

		private void AutoCompleteList_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var autoCompleteOption = AutoCompleteOptions[SelectedIndex];

			if (!File.Exists(autoCompleteOption))
			{
				var confirmation =
					OpenFileDialog.Confirm("This DB no longer exists. Do you wish to remove it from this list?");
				if (confirmation)
				{
					DatabaseList.Remove(autoCompleteOption);
					AutoCompleteOptions.Remove(autoCompleteOption);
				}
			}
			else
			{
				Filepath = autoCompleteOption;
			}
			AutoCompletePopup.IsOpen = false;
		}

		private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
		{
			var filepath = OpenFileDialog.GetFilePath();

			if (string.IsNullOrWhiteSpace(filepath))
				return;
			Filepath = filepath;
		}

		private void ClearFilepathButton(object sender, RoutedEventArgs e)
		{
			FilepathTextBox.Clear();
		}

		private void FilepathTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			AutoCompleteOptions = DatabaseList?.ToList();
			AutoCompletePopup.IsOpen = !AutoCompletePopup.IsOpen;
		}

		private void FilepathTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;

				if (AutoCompletePopup.IsOpen)
					AutoCompletePopup.IsOpen = false;
				else if (!string.IsNullOrWhiteSpace(Filepath))
					FilepathTextBox.Clear();
				else
				{
					e.Handled = false;
				}
			}

			if (e.Key == Key.Enter)
			{
				if (AutoCompletePopup.IsOpen)
				{
					Filepath = AutoCompleteOptions[AutoCompleteList.SelectedIndex];
					AutoCompletePopup.IsOpen = false;
				}

				e.Handled = true;
			}

			if (e.Key == Key.Down)
			{
				AutoCompletePopup.IsOpen = true;
				if (AutoCompleteList.SelectedIndex < AutoCompleteList.Items.Count - 1)
				{
					AutoCompleteList.SelectedIndex++;
					AutoCompleteList.ScrollIntoView(AutoCompleteList.SelectedItem);
				}
				e.Handled = true;
			}
			else if (e.Key == Key.Up)
			{
				if (AutoCompleteList.SelectedIndex > 0)
				{
					AutoCompleteList.SelectedIndex--;
					AutoCompleteList.ScrollIntoView(AutoCompleteList.SelectedItem);
				}
				e.Handled = true;
			}
		}
	}
}